using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ClinicReception.Core.Legacy;

public class FileOrderStore : ILegacyDocumentRepository
{
    private readonly List<LegacyDocument> _documents = new();

    public void Add(LegacyDocument document)
    {
        _documents.Add(document);
    }

    public LegacyDocument? GetById(int id)
    {
        return _documents.Find(document => document.Id == id);
    }

    public IReadOnlyList<LegacyDocument> GetAll()
    {
        return _documents;
    }

    public void SaveToFile(LegacyDocument document, string path)
    {
        string json = JsonSerializer.Serialize(document);
        File.WriteAllText(path, json);
    }
}
