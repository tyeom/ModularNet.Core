using ModularNet.Core.Attributes;
using ModularNet.Core.Exceptions;
using ModularNet.Core.Pipes;
using ModularNet.Sample.Interceptors;
using ModularNet.Sample.Models;
using ModularNet.Sample.Services;

namespace ModularNet.Sample.Controllers;

[Controller("products")]
[UseInterceptors(typeof(AuthInterceptor))]
public class ProductController
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [Get]
    public IEnumerable<Product> GetAllProducts()
    {
        _logger.LogInformation("Fetching all products");
        return _productService.GetAll();
    }

    [Get("{id}")]
    public Product GetProductById([Pipe(typeof(ParseIntPipe))] int id)
    {
        var product = _productService.GetById(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }
        return product;
    }

    [Post]
    public Product CreateProduct([Pipe(typeof(ValidationPipe))] CreateProductDto dto)
    {
        _logger.LogInformation("Creating new product: {ProductName}", dto.Name);
        return _productService.Create(dto);
    }

    [Put("{id}")]
    public Product UpdateProduct(
        [Pipe(typeof(ParseIntPipe))] int id,
        [Pipe(typeof(ValidationPipe))] UpdateProductDto dto)
    {
        var product = _productService.Update(id, dto);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }

        _logger.LogInformation("Updated product: {ProductId}", id);
        return product;
    }

    [Delete("{id}")]
    public object DeleteProduct([Pipe(typeof(ParseIntPipe))] int id)
    {
        var success = _productService.Delete(id);
        if (!success)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }

        _logger.LogInformation("Deleted product: {ProductId}", id);
        return new { message = $"Product {id} deleted successfully" };
    }
}
