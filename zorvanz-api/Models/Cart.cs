using System;
using System.Collections.Generic;

namespace zorvanz_api.Models;

public partial class Cart
{
    public Guid Id { get; set; }

    public decimal? TotalAmount { get; set; }

    public Guid? CustomerId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
