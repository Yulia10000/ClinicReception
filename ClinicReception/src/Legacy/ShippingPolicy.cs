namespace Variant.Legacy;

public class ShippingPolicy
{
    public decimal Calculate(decimal sum, decimal discount)
    {
        if (sum - discount < PricingRules.FreeShipFrom)
            return PricingRules.ShipCost;

        return 0m;
    }
}