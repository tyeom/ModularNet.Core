using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModularNet.Core.Attributes;
using ModularNet.Core.Binding;
using ModularNet.Core.Context;
using ModularNet.Core.Interfaces;
using ModularNet.Core.Middleware;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace ModularNet.Core;

public class ModularAppFactory
{
    public static WebApplication CreateApp<TRootModule>(string[] args)
        where TRootModule : IModule, new()
    {
        var builder = WebApplication.CreateBuilder(args);

        // 루트 모듈 인스턴스 생성
        var rootModule = new TRootModule();

        // 모듈의 서비스 등록
        rootModule.ConfigureServices(builder.Services);

        // 컨트롤러 리플렉션 등록
        RegisterControllers<TRootModule>(builder.Services);

        var app = builder.Build();

        // 전역 예외 처리 미들웨어 등록
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // 모듈의 미들웨어 구성
        rootModule.ConfigureApp(app);

        // 컨트롤러 라우팅 매핑
        MapControllers(app);

        return app;
    }

    private static void RegisterControllers<TRootModule>(IServiceCollection services)
        where TRootModule : IModule
    {
        // 모든 모듈에서 컨트롤러 찾아서 등록
        var moduleType = typeof(TRootModule);
        var moduleAttr = moduleType.GetCustomAttribute<ModuleAttribute>();

        if (moduleAttr != null)
        {
            foreach (var controller in moduleAttr.Controllers)
            {
                services.AddScoped(controller);
            }
        }
    }

    private static void MapControllers(WebApplication app)
    {
        // 리플렉션으로 컨트롤러 메서드를 라우트에 매핑
        var serviceProvider = app.Services;
        var controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<ControllerAttribute>() != null);

        foreach (var controllerType in controllerTypes)
        {
            var controllerAttr = controllerType.GetCustomAttribute<ControllerAttribute>();
            var basePath = controllerAttr.Route;

            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<GetAttribute>() != null ||
                           m.GetCustomAttribute<PostAttribute>() != null ||
                           m.GetCustomAttribute<PutAttribute>() != null ||
                           m.GetCustomAttribute<DeleteAttribute>() != null ||
                           m.GetCustomAttribute<PatchAttribute>() != null);

            foreach (var method in methods)
            {
                MapMethod(app, serviceProvider, controllerType, method, basePath);
            }
        }
    }

    private static void MapMethod(WebApplication app, IServiceProvider serviceProvider,
        Type controllerType, MethodInfo method, string basePath)
    {
        var getAttr = method.GetCustomAttribute<GetAttribute>();
        var postAttr = method.GetCustomAttribute<PostAttribute>();
        var putAttr = method.GetCustomAttribute<PutAttribute>();
        var deleteAttr = method.GetCustomAttribute<DeleteAttribute>();
        var patchAttr = method.GetCustomAttribute<PatchAttribute>();

        string httpMethod;
        string path;

        if (getAttr != null)
        {
            httpMethod = "GET";
            path = getAttr.Path;
        }
        else if (postAttr != null)
        {
            httpMethod = "POST";
            path = postAttr.Path;
        }
        else if (putAttr != null)
        {
            httpMethod = "PUT";
            path = putAttr.Path;
        }
        else if (deleteAttr != null)
        {
            httpMethod = "DELETE";
            path = deleteAttr.Path;
        }
        else if (patchAttr != null)
        {
            httpMethod = "PATCH";
            path = patchAttr.Path;
        }
        else
        {
            return;
        }

        string fullPath = $"/{basePath}/{path}".Replace("//", "/");

        // 인터셉터 수집 (클래스 레벨 + 메서드 레벨)
        var classInterceptors = controllerType.GetCustomAttributes<UseInterceptorsAttribute>()
            .SelectMany(a => a.InterceptorTypes);
        var methodInterceptors = method.GetCustomAttributes<UseInterceptorsAttribute>()
            .SelectMany(a => a.InterceptorTypes);
        var allInterceptorTypes = classInterceptors.Concat(methodInterceptors).ToArray();

        // HTTP 메서드별로 라우트 등록
        Func<HttpContext, Task<object?>> handler = async (HttpContext context) =>
        {
            using var scope = serviceProvider.CreateScope();
            var controller = scope.ServiceProvider.GetRequiredService(controllerType);

            var arguments = await ParameterBinder.BindParametersAsync(context, method, scope.ServiceProvider);

            var execContext = new Context.ExecutionContext
            {
                HttpContext = context,
                ControllerType = controllerType,
                Method = method,
                Arguments = arguments
            };

            var result = await ExecuteWithInterceptors(
                scope.ServiceProvider,
                allInterceptorTypes,
                execContext,
                () => Task.FromResult(method.Invoke(controller, execContext.Arguments))
            );

            return result;
        };

        switch (httpMethod)
        {
            case "GET":
                app.MapGet(fullPath, handler);
                break;
            case "POST":
                app.MapPost(fullPath, handler);
                break;
            case "PUT":
                app.MapPut(fullPath, handler);
                break;
            case "DELETE":
                app.MapDelete(fullPath, handler);
                break;
            case "PATCH":
                app.MapPatch(fullPath, handler);
                break;
        }
    }

    private static async Task<object?> ExecuteWithInterceptors(
        IServiceProvider serviceProvider,
        Type[] interceptorTypes,
        Context.ExecutionContext context,
        Func<Task<object?>> finalHandler)
    {
        if (interceptorTypes.Length == 0)
        {
            return await finalHandler();
        }

        // 인터셉터 체인 빌드 (역순으로)
        Func<Task<object?>> currentHandler = finalHandler;

        for (int i = interceptorTypes.Length - 1; i >= 0; i--)
        {
            var interceptorType = interceptorTypes[i];
            var capturedHandler = currentHandler;

            currentHandler = async () =>
            {
                var interceptor = (IInterceptor)serviceProvider.GetRequiredService(interceptorType);
                var callHandler = new CallHandler(capturedHandler);
                return await interceptor.InterceptAsync(context, callHandler);
            };
        }

        return await currentHandler();
    }
}
