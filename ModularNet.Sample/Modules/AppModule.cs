using ModularNet.Core;
using ModularNet.Core.Attributes;
using ModularNet.Sample.Controllers;
using ModularNet.Sample.Interceptors;
using ModularNet.Sample.Services;

namespace ModularNet.Sample.Modules;

[Module(
    Imports = [typeof(ProductModule), typeof(AuthModule)],
    Controllers = [typeof(WeatherController), typeof(UserController)],
    Providers = [typeof(WeatherService), typeof(LoggingInterceptor)]
)]
public class AppModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        // Application-wide configuration
        services.AddScoped<IWeatherService, WeatherService>();
    }

    public override void ConfigureApp(IApplicationBuilder app)
    {
        base.ConfigureApp(app);

        // Application-wide middleware configuration
    }
}
