using ClinicReception.Core.Legacy;

namespace ClinicReception.Core.Services;

public class LegacyOrderService
{
    private readonly ILegacyDocumentRepository _repository;

    public LegacyOrderService(
        ILegacyDocumentRepository repository)
    {
        _repository = repository;
    }

    // Антипатерн 1: проковтнутий виняток і повернення null
    public LegacyDocument? Load(int documentId)
    {
        try
        {
            return _repository.GetById(documentId);
        }
        catch (Exception)
        {
        }

        return null;
    }

    // Антипатерни 2 і 3: широкий catch та throw ex
    public void Cancel(int documentId, string reason)
    {
        try
        {
            var document = _repository.GetById(documentId);

            document!.Status = 2;

            _repository.SaveToFile(
                document,
                "documents.json");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    // Антипатерн 4: виняток замість TryParse
    public int CountValidQuantities(string[] rows)
    {
        var valid = 0;

        foreach (var row in rows)
        {
            try
            {
                var quantity = int.Parse(row);

                if (quantity > 0)
                {
                    valid++;
                }
            }
            catch (FormatException)
            {
                // Неправильне значення пропускається
            }
        }

        return valid;
    }
}
