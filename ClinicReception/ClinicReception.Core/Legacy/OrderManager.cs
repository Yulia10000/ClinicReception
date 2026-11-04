using System.Globalization;

namespace ClinicReception.Core.Legacy;

public class OrderManager
{
    private readonly ILegacyDocumentRepository _repository;
    private readonly ILegacyPricingPolicy _pricingPolicy;
    private readonly INotifier _notifier;

    public OrderManager(
        ILegacyDocumentRepository repository,
        ILegacyPricingPolicy pricingPolicy,
        INotifier notifier)
    {
        _repository = repository;
        _pricingPolicy = pricingPolicy;
        _notifier = notifier;
    }

    public decimal Place(LegacyDocument document)
    {
        decimal total = 0m;

        foreach (LegacyLine line in document.Lines)
        {
            total += _pricingPolicy.PriceOf(line);
        }

        if (total > 1000m)
        {
            total -= 50m;
        }

        document.Total = total;
        document.Status = 1;

        _repository.SaveToFile(document, "documents.json");

        _notifier.Notify(
            document.Email,
            "Document " + document.Id,
            total.ToString(CultureInfo.InvariantCulture));

        return total;
    }
}
