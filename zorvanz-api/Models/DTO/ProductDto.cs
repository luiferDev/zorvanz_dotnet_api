namespace zorvanz_api.Models.DTO;

public class ProductDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public double? Popularity { get; set; }

    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}