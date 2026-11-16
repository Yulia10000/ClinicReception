
using System;
using ClinicReception.Core.Domain;
using ClinicReception.Core.Policies;
using ClinicReception.Core.Repositories;
using ClinicReception.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ClinicReception.Tests;

public class DocumentSolidTests
{
    [Fact]
    public void OrderLine_WithZeroQuantity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OrderLine("A1", 0, 100m));
    }

    [Fact]
    public void Document_AddLineWhenPaid_ThrowsInvalidOperationException()
    {
        Document document = new(
            1,
            1,
            new DateOnly(2026, 3, 10));

        document.AddLine(new OrderLine("A1", 2, 100m));
        document.Pay();

        Assert.Throws<InvalidOperationException>(() =>
            document.AddLine(new OrderLine("B2", 1, 50m)));
    }

    [Fact]
    public void DocumentService_TotalOfWithInjectedDependencies_ReturnsCorrectTotal()
    {
        InMemoryDocumentRepository repository = new();
        DiscountPricingPolicy pricing = new(0.10m);

        DocumentService service = new(
            repository,
            pricing,
            NullLogger<DocumentService>.Instance);

        Document document = new(
            1,
            1,
            new DateOnly(2026, 3, 10));

        document.AddLine(new OrderLine("A1", 2, 100m));
        document.AddLine(new OrderLine("B2", 1, 50m));

        service.Place(document);

        decimal total = service.TotalOf(document);

        Assert.Equal(225m, total);
        Assert.Same(document, repository.GetById(1));
    }
}
