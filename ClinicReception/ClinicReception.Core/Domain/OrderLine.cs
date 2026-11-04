namespace ClinicReception.Core.Domain;

public sealed record OrderLine
{
    public string Sku { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public decimal Amount => Quantity * UnitPrice;

    public OrderLine(string sku, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);

        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
