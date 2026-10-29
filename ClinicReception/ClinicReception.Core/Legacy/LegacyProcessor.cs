

using System;
using System.Collections.Generic;

namespace ClinicReception.Core.Legacy;

/// <summary>
/// Містить початковий метод обробки документа.
/// </summary>
public static class LegacyProcessor
{
    /// <summary>
    /// Обробляє документ, розраховує суму та визначає наступний стан.
    /// </summary>
    public static decimal Process(
        int docId,
        string clientName,
        string clientEmail,
        bool isRegular,
        List<OrderLine> lines,
        DateOnly createdAt,
        string couponCode,
        string status,
        bool printToConsole,
        decimal deliveryPrice,
        out string newStatus)
    {
        DocumentGuards.EnsureHeaderValid(
            docId,
            clientName,
            clientEmail);

        DocumentGuards.EnsureLinesValid(lines);

        if (status != "New" &&
            status != "Paid" &&
            status != "Shipped" &&
            status != "Cancelled")
        {
            throw new ArgumentException(
                "Невідомий стан документа.",
                nameof(status));
        }

        newStatus = status;
        decimal total = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            decimal sum =
                lines[i].Quantity * lines[i].UnitPrice;

            if (lines[i].Quantity >= 10)
            {
                sum = sum * 0.95m;
            }

            total = total + sum;
        }

        if (isRegular)
        {
            total = total * 0.93m;
        }

        if (total > 5000)
        {
            total = total * 0.9m;
        }
        else if (total > 2000)
        {
            total = total * 0.95m;
        }

        if (couponCode == "SALE10")
        {
            total = total - total * 0.1m;
        }
        else if (couponCode == "MINUS200" && total > 1000)
        {
            total = total - 200;
        }

        if (createdAt.DayOfWeek == DayOfWeek.Sunday)
        {
            total = total * 0.98m;
        }

        if (total < 1000)
        {
            total = total + deliveryPrice;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = Math.Round(total, 2);

        if (status == "New" && total > 0)
        {
            newStatus = "Paid";
        }
        else if (status == "Paid")
        {
            newStatus = "Shipped";
        }
        else if (status == "Cancelled")
        {
            newStatus = "Cancelled";
            total = 0;
        }

        if (printToConsole)
        {
            Console.WriteLine("Документ № " + docId);
            Console.WriteLine("Клієнт: " + clientName);
            Console.WriteLine("Пошта: " + clientEmail);

            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(
                    lines[i].Sku + " x " + lines[i].Quantity);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
