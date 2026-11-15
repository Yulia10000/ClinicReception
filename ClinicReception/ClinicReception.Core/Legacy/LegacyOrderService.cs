
using ClinicReception.Core.Legacy;
using ClinicReception.Core.Errors;

namespace ClinicReception.Core.Services;

public class LegacyOrderService
{
    private readonly ILegacyDocumentRepository _repository;

    public LegacyOrderService(
        ILegacyDocumentRepository repository)
    {
        _repository = repository;
    }

    // Виправлення антипатерну 1
    public LegacyDocument? Load(int documentId)
    {
        return _repository.GetById(documentId);
    }

    // Виправлення антипатернів 2 і 3
    public void Cancel(int documentId, string reason)
    {
        var document = _repository.GetById(documentId)
            ?? throw new DomainRuleException(
                "document.exists",
                $"Документ {documentId} не знайдено.");

        document.Status = 2;

        _repository.SaveToFile(
            document,
            "documents.json");
    }

    // Виправлення антипатерну 4
    public int CountValidQuantities(string[] rows)
    {
        var valid = 0;

        foreach (var row in rows)
        {
            if (int.TryParse(row, out var quantity) &&
                quantity > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
