// Partial class extension for Order to add GHN shipping IDs
// This is separate from the auto-generated file to avoid conflicts
#nullable enable
using System;

namespace DAL.Models;

public partial class Order
{
    /// <summary>
    /// GHN Province ID for shipping destination (from GHN master data API)
    /// </summary>
    public int? ShippingProvinceId { get; set; }

    /// <summary>
    /// GHN District ID for shipping destination (from GHN master data API)
    /// Required for creating GHN shipping orders
    /// </summary>
    public int? ShippingDistrictId { get; set; }

    /// <summary>
    /// GHN Ward Code for shipping destination (from GHN master data API)
    /// Format: "21211" (5-digit string code)
    /// Helps improve delivery accuracy
    /// </summary>
    public string? ShippingWardCode { get; set; }
}
