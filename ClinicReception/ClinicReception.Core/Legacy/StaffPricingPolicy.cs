namespace ClinicReception.Core.Legacy;

public sealed class StaffPricingPolicy : ILegacyPricingPolicy
{
    public decimal PriceOf(LegacyLine line)
    {
        return line.UnitPrice * line.Quantity * 0.70m;
    }
}
