using System;
using System.Collections.Generic;
using System.IO;
using ClinicReception.Core.Documents;
using ClinicReception.Core.Domain;
using Xunit;

namespace ClinicReception.Tests;

public class DocumentTotalCalculatorTests
{
    private static TotalRequest BuildSampleRequest()
    {
        return new TotalRequest(
            DocumentId: 1,
            Client: new Patient(1, "Іваненко", "i@ex.com")
            {
                IsRegular = true
            },
            Lines: new List<OrderLine>
            {
                new("A1", 12, 100m),
                new("B2", 1, 250m)
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: DocumentStatus.New,
            DeliveryPrice: 60m);
    }

    [Fact]
    public void Calculate_RegularClient_Returns1292()
    {
        TotalRequest request = BuildSampleRequest();

        TotalResult result =
            DocumentTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(DocumentStatus.Paid, result.Status);
    }

    [Fact]
    public void Calculate_SmallOrderAddsDelivery()
    {
        TotalRequest request = new(
            DocumentId: 2,
            Client: new Patient(2, "Петренко", "p@ex.com"),
            Lines: new List<OrderLine>
            {
                new("A1", 2, 200m)
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: "SALE10",
            Status: DocumentStatus.Paid,
            DeliveryPrice: 60m);

        TotalResult result =
            DocumentTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(DocumentStatus.Shipped, result.Status);
    }

    [Fact]
    public void CalculateNoLinesThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            DocumentGuards.EnsureLinesValid(
                new List<OrderLine>()));
    }

    [Fact]
    public void CalculateZeroQuantityThrowsRange()
    {
        OrderLine line = new("A1", 1, 10m);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OrderLine("A1", 0, 10m));
    }

    [Fact]
    public void TextOf_ContainsTotalLine()
    {
        TotalRequest request = BuildSampleRequest();

        TotalResult result =
            DocumentTotalCalculator.Calculate(request);

        string text =
            DocumentReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text);
    }

    [Fact]
    public void Print_WritesReportToOutput()
    {
        StringWriter writer = new();
        DocumentReportPrinter printer =
            new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString());
    }

    [Fact]
    public void TextOfIncludesDocumentNumberAndClient()
    {
        TotalRequest request = BuildSampleRequest();

        TotalResult result =
            DocumentTotalCalculator.Calculate(request);

        string text =
            DocumentReport.TextOf(request, result);

        Assert.Contains("Документ № 1", text);
        Assert.Contains("Пацієнт: Іваненко", text);
    }

    [Fact]
    public void TextOfIncludesOrderLines()
    {
        TotalRequest request = BuildSampleRequest();

        TotalResult result =
            DocumentTotalCalculator.Calculate(request);

        string text =
            DocumentReport.TextOf(request, result);

        Assert.Contains("A1 x 12", text);
        Assert.Contains("B2 x 1", text);
    }
}
