using System;
using System.Collections.Generic;

namespace zorvanz_api.Models;

public partial class OrderDetail
{
    public Guid Id { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? UnitPrice { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? ProductId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
