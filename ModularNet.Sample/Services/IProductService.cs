using ModularNet.Sample.Models;

namespace ModularNet.Sample.Services;

public interface IProductService
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    Product Create(CreateProductDto dto);
    Product? Update(int id, UpdateProductDto dto);
    bool Delete(int id);
}
