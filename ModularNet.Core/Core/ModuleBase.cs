using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ModularNet.Core.Attributes;
using ModularNet.Core.Enums;
using ModularNet.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModularNet.Core;

public abstract class ModuleBase : IModule
{
    public virtual void ConfigureServices(IServiceCollection services)
    {
        var moduleAttr = GetType().GetCustomAttribute<ModuleAttribute>();
        if (moduleAttr == null) return;

        // Controllers 등록
        foreach (var controller in moduleAttr.Controllers)
        {
            services.AddScoped(controller);
        }

        // Providers 등록 (서비스들)
        foreach (var provider in moduleAttr.Providers)
        {
            RegisterService(services, provider);
        }

        // Import된 모듈들 등록
        foreach (var importModule in moduleAttr.Imports)
        {
            if (Activator.CreateInstance(importModule) is IModule module)
            {
                module.ConfigureServices(services);
            }
        }
    }

    private void RegisterService(IServiceCollection services, Type providerType)
    {
        var injectableAttr = providerType.GetCustomAttribute<InjectableAttribute>();
        var scope = injectableAttr?.Scope ?? ServiceScope.Scoped;

        // 인터페이스 찾기 (System 네임스페이스 제외)
        var interfaces = providerType.GetInterfaces()
            .Where(i => i.Namespace != null && !i.Namespace.StartsWith("System"))
            .ToArray();

        // 인터페이스가 있으면 인터페이스로 등록
        if (interfaces.Length > 0)
        {
            foreach (var serviceInterface in interfaces)
            {
                RegisterServiceByScope(services, serviceInterface, providerType, scope);
            }
        }

        // 구현 타입으로도 등록
        RegisterServiceByScope(services, providerType, providerType, scope);
    }

    private void RegisterServiceByScope(IServiceCollection services, Type serviceType, Type implementationType, ServiceScope scope)
    {
        switch (scope)
        {
            case ServiceScope.Singleton:
                services.AddSingleton(serviceType, implementationType);
                break;
            case ServiceScope.Scoped:
                services.AddScoped(serviceType, implementationType);
                break;
            case ServiceScope.Transient:
                services.AddTransient(serviceType, implementationType);
                break;
        }
    }

    public virtual void ConfigureApp(IApplicationBuilder app)
    {
        // 기본 구현
    }
}