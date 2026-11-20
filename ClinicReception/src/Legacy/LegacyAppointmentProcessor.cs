


using System;
using System.Collections.Generic;

namespace Variant.Legacy;

public enum ClientKind
{
    Regular,
    Vip,
    Staff
}

public enum DocState
{
    Draft,
    New,
    Paid,
    Closed
}

public class Patient
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public ClientKind Kind = ClientKind.Regular;
    public int DoneCount;
    public DateTime SinceUtc;
}

public class ServiceLine
{
    public string Code = "";
    public int Qty;
    public decimal Price;
    public string Group = "";
}

public class LegacyAppointmentProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly List<string> _log = new();

    public string Handle(int docId, int clientId,
        string clientName, string? clientMail,
        ClientKind clientKind, int clientDone,
        List<ServiceLine>? items, DocState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (items == null)
            return "ERR: null";

        if (items.Count == 0)
            return "ERR: empty";

        if (state != DocState.New && state != DocState.Paid)
            return "ERR: state";

        if (clientMail == null || !clientMail.Contains("@"))
            return "ERR: mail";

        _log.Add("ok " + docId);

        _tmpSum = Subtotal(items);
        _tmpDiscount = DiscountOf(_tmpSum, clientKind, clientDone);
        decimal ship = ShippingOf(_tmpSum, _tmpDiscount);

        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + clientName + "\n";

        for (int i = 0; i < items.Count; i++)
        {
            txt += items[i].Code + " x" + items[i].Qty + " = " +
                (items[i].Qty * items[i].Price)
                    .ToString("0.00") + " " + currency + "\n";
        }

        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";

        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
        }

        return txt;
    }

    private decimal Subtotal(List<ServiceLine> items)
    {
        decimal sum = 0m;

        foreach (var item in items)
        {
            sum += item.Qty * item.Price;
        }

        return sum;
    }

    private decimal DiscountOf(
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

    private decimal ShippingOf(decimal sum, decimal discount)
    {
        if (sum - discount < PricingRules.FreeShipFrom)
            return PricingRules.ShipCost;

        return 0m;
    }

    public decimal Preview(ClientKind clientKind, int clientDone,
        List<ServiceLine> items)
    {
        decimal sum = Subtotal(items);
        decimal discount = DiscountOf(sum, clientKind, clientDone);
        decimal shipping = ShippingOf(sum, discount);

        return sum - discount + shipping;
    }

    public string DescribeClient(Patient c)
    {
        string s = c.Name.Trim().ToUpper();

        if (c.Kind == ClientKind.Vip)
            s += " [VIP]";

        if (c.DoneCount > PricingRules.LoyalOrders)
            s += " [ЛОЯЛЬНИЙ]";

        s += " <" + c.Email.ToLower() + ">";

        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;

        return s;
    }

    public string DumpLog()
    {
        string r = "";

        foreach (var l in _log)
            r += l + "\n";

        return r;
    }
}


