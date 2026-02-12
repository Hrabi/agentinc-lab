using System.ComponentModel.DataAnnotations;

namespace Contoso.Orders.Domain;

/// <summary>
/// Represents a customer order in the Contoso Electronics system.
/// </summary>
public class Order
{
    /// <summary>
    /// Unique order identifier (e.g., "ORD-2026-00001").
    /// </summary>
    public required string OrderId { get; init; }

    /// <summary>
    /// The customer who placed the order.
    /// </summary>
    public required string CustomerId { get; init; }

    /// <summary>
    /// Date and time the order was placed (UTC).
    /// </summary>
    public required DateTimeOffset OrderDate { get; init; }

    /// <summary>
    /// Current status of the order.
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Line items in the order.
    /// </summary>
    public required List<OrderLine> Lines { get; init; }

    /// <summary>
    /// Shipping address for the order.
    /// </summary>
    public required Address ShippingAddress { get; init; }

    /// <summary>
    /// Optional discount code applied to the order.
    /// </summary>
    public string? DiscountCode { get; init; }

    /// <summary>
    /// Calculated total before tax and shipping.
    /// </summary>
    public decimal Subtotal => Lines.Sum(l => l.Quantity * l.UnitPrice);

    /// <summary>
    /// Tax amount (21% VAT for EU orders).
    /// </summary>
    public decimal Tax => Subtotal * 0.21m;

    /// <summary>
    /// Shipping cost. Free for orders over €75.
    /// </summary>
    public decimal ShippingCost => Subtotal >= 75m ? 0m : 5.99m;

    /// <summary>
    /// Grand total including tax and shipping.
    /// </summary>
    public decimal Total => Subtotal + Tax + ShippingCost;
}

/// <summary>
/// A single line item in an order.
/// </summary>
public class OrderLine
{
    /// <summary>
    /// Product SKU (e.g., "SH-HUB-400").
    /// </summary>
    public required string Sku { get; init; }

    /// <summary>
    /// Product name at time of order (denormalized for history).
    /// </summary>
    public required string ProductName { get; init; }

    /// <summary>
    /// Quantity ordered.
    /// </summary>
    [Range(1, 100)]
    public required int Quantity { get; init; }

    /// <summary>
    /// Unit price at time of order (EUR).
    /// </summary>
    public required decimal UnitPrice { get; init; }

    /// <summary>
    /// Line total (Quantity × UnitPrice).
    /// </summary>
    public decimal LineTotal => Quantity * UnitPrice;
}

/// <summary>
/// Shipping or billing address.
/// </summary>
public class Address
{
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Country { get; init; }
    public string? State { get; init; }
}

/// <summary>
/// Order lifecycle states.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order received, awaiting payment confirmation.</summary>
    Pending,

    /// <summary>Payment confirmed, order is being prepared.</summary>
    Confirmed,

    /// <summary>Order has been handed to the carrier.</summary>
    Shipped,

    /// <summary>Order delivered to the customer.</summary>
    Delivered,

    /// <summary>Order cancelled by customer or system.</summary>
    Cancelled,

    /// <summary>Return initiated by the customer.</summary>
    ReturnRequested,

    /// <summary>Returned items received and refund processed.</summary>
    Refunded
}
