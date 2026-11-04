using System;
using ClinicReception.Core.Abstractions;
using ClinicReception.Core.Domain;

namespace ClinicReception.Core.Services;

public sealed class DocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IPricingPolicy _pricing;

    public DocumentService(
        IDocumentRepository repository,
        IPricingPolicy pricing)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));

        _pricing = pricing
            ?? throw new ArgumentNullException(nameof(pricing));
    }

    public void Place(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        _repository.Add(document);
    }

    public decimal TotalOf(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        decimal total = 0m;

        foreach (OrderLine line in document.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }
}
