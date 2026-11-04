namespace ClinicReception.Core.Legacy;

public sealed class RegularPricingPolicy : ILegacyPricingPolicy
{
    public decimal PriceOf(LegacyLine line)
    {
        return line.UnitPrice * line.Quantity * 0.95m;
    }
}
