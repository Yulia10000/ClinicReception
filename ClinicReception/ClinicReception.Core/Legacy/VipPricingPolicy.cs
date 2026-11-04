namespace ClinicReception.Core.Legacy;

public sealed class VipPricingPolicy : ILegacyPricingPolicy
{
    public decimal PriceOf(LegacyLine line)
    {
        return line.UnitPrice * line.Quantity * 0.90m;
    }
}
