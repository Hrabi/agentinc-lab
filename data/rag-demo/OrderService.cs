using System.ComponentModel.DataAnnotations;

namespace Contoso.Orders.Domain;

/// <summary>
/// Repository for managing customer orders with validation and business rules.
/// </summary>
public class OrderRepository
{
    private readonly Dictionary<string, Order> _orders = new(StringComparer.OrdinalIgnoreCase);
    private int _nextSequence = 1;

    /// <summary>
    /// Creates a new order after validating all business rules.
    /// </summary>
    /// <exception cref="ValidationException">Thrown when order data is invalid.</exception>
    public Order CreateOrder(string customerId, List<OrderLine> lines, Address shippingAddress, string? discountCode = null)
    {
        // Validate customer ID
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ValidationException("Customer ID is required.");

        // Validate at least one line item
        if (lines is not { Count: > 0 })
            throw new ValidationException("Order must contain at least one line item.");

        // Validate each line item
        foreach (var line in lines)
        {
            if (line.Quantity < 1 || line.Quantity > 100)
                throw new ValidationException($"Invalid quantity {line.Quantity} for SKU {line.Sku}. Must be 1-100.");

            if (line.UnitPrice <= 0)
                throw new ValidationException($"Invalid price {line.UnitPrice} for SKU {line.Sku}. Must be positive.");
        }

        // Validate shipping address
        ValidateAddress(shippingAddress);

        // Validate discount code format (if provided)
        if (discountCode is not null && !IsValidDiscountCode(discountCode))
            throw new ValidationException($"Invalid discount code format: {discountCode}. Expected format: CONTOSO-XXXX-XXXX.");

        var orderId = $"ORD-{DateTimeOffset.UtcNow:yyyy}-{_nextSequence++:D5}";

        var order = new Order
        {
            OrderId = orderId,
            CustomerId = customerId,
            OrderDate = DateTimeOffset.UtcNow,
            Lines = lines,
            ShippingAddress = shippingAddress,
            DiscountCode = discountCode
        };

        _orders[orderId] = order;
        return order;
    }

    /// <summary>
    /// Retrieves an order by ID. Returns null if not found.
    /// </summary>
    public Order? GetOrder(string orderId) =>
        _orders.GetValueOrDefault(orderId);

    /// <summary>
    /// Gets all orders for a specific customer, ordered by date descending.
    /// </summary>
    public IReadOnlyList<Order> GetOrdersByCustomer(string customerId) =>
        _orders.Values
            .Where(o => o.CustomerId.Equals(customerId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(o => o.OrderDate)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Cancels an order. Only Pending and Confirmed orders can be cancelled.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the order cannot be cancelled.</exception>
    public void CancelOrder(string orderId)
    {
        var order = GetOrder(orderId)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
            throw new InvalidOperationException(
                $"Cannot cancel order {orderId} — current status is {order.Status}. " +
                "Only Pending or Confirmed orders can be cancelled.");

        order.Status = OrderStatus.Cancelled;
    }

    /// <summary>
    /// Initiates a return for a delivered order within the 30-day return window.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when return conditions are not met.</exception>
    public void RequestReturn(string orderId)
    {
        var order = GetOrder(orderId)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        if (order.Status != OrderStatus.Delivered)
            throw new InvalidOperationException(
                $"Cannot return order {orderId} — status is {order.Status}. Only Delivered orders can be returned.");

        // Check 30-day return window
        var daysSinceOrder = (DateTimeOffset.UtcNow - order.OrderDate).TotalDays;
        if (daysSinceOrder > 30)
            throw new InvalidOperationException(
                $"Return window expired for order {orderId}. " +
                $"Orders must be returned within 30 days (ordered {daysSinceOrder:F0} days ago).");

        order.Status = OrderStatus.ReturnRequested;
    }

    /// <summary>
    /// Gets summary statistics for all orders.
    /// </summary>
    public OrderStatistics GetStatistics()
    {
        var orders = _orders.Values.ToList();
        return new OrderStatistics
        {
            TotalOrders = orders.Count,
            TotalRevenue = orders.Where(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Refunded).Sum(o => o.Total),
            AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.Subtotal) : 0m,
            OrdersByStatus = orders.GroupBy(o => o.Status).ToDictionary(g => g.Key, g => g.Count()),
            TopProducts = orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.Lines)
                .GroupBy(l => l.Sku)
                .Select(g => new ProductSales { Sku = g.Key, ProductName = g.First().ProductName, TotalQuantity = g.Sum(l => l.Quantity) })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(5)
                .ToList()
        };
    }

    private static void ValidateAddress(Address address)
    {
        if (string.IsNullOrWhiteSpace(address.Street))
            throw new ValidationException("Street is required.");
        if (string.IsNullOrWhiteSpace(address.City))
            throw new ValidationException("City is required.");
        if (string.IsNullOrWhiteSpace(address.PostalCode))
            throw new ValidationException("Postal code is required.");
        if (string.IsNullOrWhiteSpace(address.Country))
            throw new ValidationException("Country is required.");
    }

    private static bool IsValidDiscountCode(string code) =>
        System.Text.RegularExpressions.Regex.IsMatch(code, @"^CONTOSO-[A-Z0-9]{4}-[A-Z0-9]{4}$");
}

/// <summary>
/// Aggregate statistics for the order system.
/// </summary>
public class OrderStatistics
{
    public int TotalOrders { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal AverageOrderValue { get; init; }
    public required Dictionary<OrderStatus, int> OrdersByStatus { get; init; }
    public required List<ProductSales> TopProducts { get; init; }
}

/// <summary>
/// Sales data for a single product.
/// </summary>
public class ProductSales
{
    public required string Sku { get; init; }
    public required string ProductName { get; init; }
    public int TotalQuantity { get; init; }
}
