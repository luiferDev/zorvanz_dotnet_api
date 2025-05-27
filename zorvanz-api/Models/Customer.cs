using System;
using System.Collections.Generic;

namespace zorvanz_api.Models;

public partial class Customer
{
    public Guid Id { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
