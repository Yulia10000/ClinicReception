using System.Collections.Generic;

namespace Variant.Legacy;

public class ReportBuilder
{
    public string Build(
        int docId,
        string clientName,
        List<ServiceLine> items,
        decimal discount,
        decimal shipping,
        decimal total,
        string currency)
    {
        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + clientName + "\n";

        foreach (var item in items)
        {
            txt += item.Code + " x" + item.Qty + " = " +
                (item.Qty * item.Price).ToString("0.00") +
                " " + currency + "\n";
        }

        txt += "Знижка: " + discount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + shipping.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";

        return txt;
    }
}