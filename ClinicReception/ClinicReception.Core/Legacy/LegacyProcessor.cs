
using System;
using System.Collections.Generic;
using ClinicReception.Core.Domain;
using ClinicReception.Core.Documents;

namespace ClinicReception.Core.Legacy;

public static class LegacyProcessor
{
    public static decimal Process(
        int docId,
        string clientName,
        string clientEmail,
        bool isRegular,
        IReadOnlyList<OrderLine> lines,
        DateOnly createdAt,
        string couponCode,
        DocumentStatus status,
        bool printToConsole,
        decimal deliveryPrice,
        out DocumentStatus newStatus)
    {
        DocumentGuards.EnsureHeaderValid(
            docId, clientName, clientEmail);

        decimal total = DocumentTotalCalculator.SubtotalOf(lines);
        total -= DocumentTotalCalculator.ClientDiscountOf(
            total, isRegular);
        total -= DocumentTotalCalculator.VolumeDiscountOf(total);
        total -= DocumentTotalCalculator.CouponDiscountOf(
            total, couponCode);
        total -= DocumentTotalCalculator.WeekendDiscountOf(
            total, createdAt);
        total += DocumentTotalCalculator.DeliveryFeeOf(
            total, deliveryPrice);

        total = status == DocumentStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);

        newStatus = DocumentTotalCalculator.NextStatusOf(
            status, total);

        if (printToConsole)
        {
            Console.WriteLine("Документ № " + docId);
            Console.WriteLine("Клієнт: " + clientName);

            foreach (OrderLine line in lines)
            {
                Console.WriteLine(
                    line.Sku + " x " + line.Quantity);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}

