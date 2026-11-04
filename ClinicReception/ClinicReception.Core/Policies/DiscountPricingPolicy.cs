using System;
using ClinicReception.Core.Abstractions;
using ClinicReception.Core.Domain;

namespace ClinicReception.Core.Policies;

public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _discountRate;

    public DiscountPricingPolicy(decimal discountRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(discountRate);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
            discountRate,
            1m);

        _discountRate = discountRate;
    }

    public decimal PriceOf(OrderLine line)
    {
        return line.Amount * (1m - _discountRate);
    }
}
