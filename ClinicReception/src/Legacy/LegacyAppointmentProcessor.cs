


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

    private readonly DiscountPolicy _discountPolicy = new();
    private readonly ShippingPolicy _shippingPolicy = new();
    private readonly ReportBuilder _reportBuilder = new();

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
        _tmpDiscount = _discountPolicy.Calculate(
            _tmpSum, clientKind, clientDone);

        decimal ship = _shippingPolicy.Calculate(
            _tmpSum, _tmpDiscount);

        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = _reportBuilder.Build(
            docId, clientName, items,
            _tmpDiscount, ship, total, currency);

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

    public decimal Preview(ClientKind clientKind, int clientDone,
        List<ServiceLine> items)
    {
        decimal sum = Subtotal(items);
        decimal discount = _discountPolicy.Calculate(
            sum, clientKind, clientDone);
        decimal shipping = _shippingPolicy.Calculate(sum, discount);

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



