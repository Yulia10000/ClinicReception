
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

    public string Handle(OrderRequest request)
    {
        if (request.Items == null)
            return "ERR: null";

        if (request.Items.Count == 0)
            return "ERR: empty";

        if (request.State != DocState.New &&
            request.State != DocState.Paid)
            return "ERR: state";

        if (request.ClientMail == null ||
            !request.ClientMail.Contains("@"))
            return "ERR: mail";

        _log.Add("ok " + request.DocId);

        _tmpSum = Subtotal(request.Items);
        _tmpDiscount = _discountPolicy.Calculate(
            _tmpSum, request.ClientKind, request.ClientDone);

        decimal ship = _shippingPolicy.Calculate(
            _tmpSum, _tmpDiscount);

        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = _reportBuilder.Build(
            request.DocId,
            request.ClientName,
            request.Items,
            _tmpDiscount,
            ship,
            total,
            request.Currency);

        if (request.SendMail)
        {
            _log.Add("mail -> " + request.ClientMail);
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



