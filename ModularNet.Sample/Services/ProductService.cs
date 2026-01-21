using ModularNet.Core.Attributes;
using ModularNet.Core.Enums;
using ModularNet.Sample.Models;

namespace ModularNet.Sample.Services;

[Injectable(ServiceScope.Singleton)]
public class ProductService : IProductService
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public ProductService()
    {
        // Seed some initial data
        _products.AddRange(new[]
        {
            new Product
            {
                Id = _nextId++,
                Name = "Laptop",
                Description = "High-performance laptop for developers",
                Price = 1299.99m,
                Stock = 10,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = _nextId++,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse",
                Price = 29.99m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = _nextId++,
                Name = "Mechanical Keyboard",
                Description = "RGB mechanical keyboard with blue switches",
                Price = 149.99m,
                Stock = 25,
                CreatedAt = DateTime.UtcNow
            }
        });
    }

    public IEnumerable<Product> GetAll()
    {
        return _products.OrderBy(p => p.Id);
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public Product Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = _nextId++,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _products.Add(product);
        return product;
    }

    public Product? Update(int id, UpdateProductDto dto)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null) return null;

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;

        product.UpdatedAt = DateTime.UtcNow;

        return product;
    }

    public bool Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null) return false;

        _products.Remove(product);
        return true;
    }
}
