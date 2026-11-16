
using System;
using ClinicReception.Core.Abstractions;
using ClinicReception.Core.Domain;
using Microsoft.Extensions.Logging;

namespace ClinicReception.Core.Services;

public sealed class DocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly ILogger<DocumentService> _log;

    public DocumentService(
        IDocumentRepository repository,
        IPricingPolicy pricing,
        ILogger<DocumentService> log)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));

        _pricing = pricing
            ?? throw new ArgumentNullException(nameof(pricing));

        _log = log
            ?? throw new ArgumentNullException(nameof(log));
    }

    public void Place(Document document)
    {
        if (document is null)
        {
            var exception = new ArgumentNullException(nameof(document));
            _log.LogError(exception, "Не передано документ для збереження");
            throw exception;
        }

        using var scope = _log.BeginScope(
            "Document:{DocumentId}", document.Id);

        _log.LogDebug(
            "Початок збереження документа {DocumentId}",
            document.Id);

        _repository.Add(document);

        _log.LogInformation(
            "Документ {DocumentId} успішно збережено",
            document.Id);
    }

    public decimal TotalOf(Document document)
    {
        if (document is null)
        {
            var exception = new ArgumentNullException(nameof(document));
            _log.LogError(exception, "Не передано документ для розрахунку");
            throw exception;
        }

        using var scope = _log.BeginScope(
            "Document:{DocumentId}", document.Id);

        _log.LogDebug(
            "Початок розрахунку суми документа {DocumentId}",
            document.Id);

        decimal total = 0m;

        foreach (OrderLine line in document.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        if (total <= 0)
        {
            _log.LogWarning(
                "Сума документа {DocumentId} дорівнює нулю або менша",
                document.Id);
        }

        _log.LogInformation(
            "Розрахунок документа {DocumentId} завершено. Сума: {Total}",
            document.Id,
            total);

        return total;
    }
}
