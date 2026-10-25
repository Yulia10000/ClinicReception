namespace ClinicReception.Core.Domain;

public sealed class VisitItem
{
    public string ServiceName { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int DurationMinutes { get; init; }

    public decimal Amount => Price;
}
