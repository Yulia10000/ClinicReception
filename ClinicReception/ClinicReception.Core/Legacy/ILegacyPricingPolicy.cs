namespace ClinicReception.Core.Legacy;

public interface ILegacyPricingPolicy
{
    decimal PriceOf(LegacyLine line);
}
