using System;
using System.Collections.Generic;

namespace zorvanz_api.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public DateTime? Date { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public Guid? CustomerId { get; set; }

    public string? PaymentMethod { get; set; }

    public Guid? CartId { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
