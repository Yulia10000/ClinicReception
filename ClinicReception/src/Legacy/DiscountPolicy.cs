namespace Variant.Legacy;

public class DiscountPolicy
{
    public decimal Calculate(
        decimal sum, ClientKind clientKind, int clientDone)
    {
        decimal discount = 0m;

        if (clientKind == ClientKind.Vip)
        {
            discount = sum * PricingRules.VipRate;
        }
        else if (clientKind == ClientKind.Staff)
        {
            discount = sum * PricingRules.StaffRate;
        }
        else if (clientDone > PricingRules.LoyalOrders)
        {
            discount = sum * PricingRules.LoyalRate;
        }

        if (discount > PricingRules.MaxDiscount)
            discount = PricingRules.MaxDiscount;

        return discount;
    }
}