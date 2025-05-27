using System.ComponentModel.DataAnnotations;

namespace zorvanz_api.Models.DTO;

public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }
    public int? CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }
    public double Popularity { get; set; }
}