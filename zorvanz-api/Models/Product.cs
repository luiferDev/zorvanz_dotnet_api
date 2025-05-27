using System;
using System.Collections.Generic;

namespace zorvanz_api.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Name { get; set; }

    public double? Popularity { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public int? CategoryId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
