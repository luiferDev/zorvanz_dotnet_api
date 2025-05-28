using System.ComponentModel.DataAnnotations;

namespace zorvanz_api.Models.DTO;

public class CreateProductDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public required string ImageUrl { get; set; }
    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    public double? Popularity { get; set; }
}