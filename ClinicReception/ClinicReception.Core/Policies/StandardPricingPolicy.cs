using ClinicReception.Core.Abstractions;
using ClinicReception.Core.Domain;

namespace ClinicReception.Core.Policies;

public sealed class StandardPricingPolicy : IPricingPolicy
{
    public decimal PriceOf(OrderLine line)
    {
        return line.Amount;
    }
}
