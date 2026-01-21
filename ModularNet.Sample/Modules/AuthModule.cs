using ModularNet.Core;
using ModularNet.Core.Attributes;
using ModularNet.Sample.Interceptors;

namespace ModularNet.Sample.Modules;

[Module(
    Providers = [typeof(AuthInterceptor), typeof(CachingInterceptor)]
)]
public class AuthModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        // Auth module specific configuration
        // In a real application, you might configure:
        // - JWT authentication
        // - OAuth providers
        // - Role-based authorization
        // - API key validation service
    }
}
