using System;
using System.Collections.Generic;

namespace zorvanz_api.Models;

public partial class CartItem
{
    public Guid Id { get; set; }

    public int Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? UnitPrice { get; set; }

    public Guid? CartId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? ProductId { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
