using System.ComponentModel.DataAnnotations;

namespace ModularNet.Sample.Models;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }
}
