using System;
using System.Collections.Generic;

namespace zorvanz_api.Models;

public partial class Invoice
{
    public Guid Id { get; set; }

    public decimal? Amount { get; set; }

    public Guid? CustomerId { get; set; }

    public DateTime? Date { get; set; }

    public Guid? OrderId { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Order? Order { get; set; }
}
