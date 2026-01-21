using ModularNet.Core;
using ModularNet.Core.Attributes;
using ModularNet.Sample.Controllers;
using ModularNet.Sample.Services;

namespace ModularNet.Sample.Modules;

[Module(
    Controllers = [typeof(ProductController)],
    Providers = [typeof(ProductService)]
)]
public class ProductModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        // Product module specific configuration can go here
        services.AddScoped<IProductService, ProductService>();
    }
}
