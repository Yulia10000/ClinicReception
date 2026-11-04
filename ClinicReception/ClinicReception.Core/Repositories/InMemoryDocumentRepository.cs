using System;
using System.Collections.Generic;
using ClinicReception.Core.Abstractions;
using ClinicReception.Core.Domain;

namespace ClinicReception.Core.Repositories;

public sealed class InMemoryDocumentRepository : IDocumentRepository
{
    private readonly Dictionary<int, Document> _documents = new();

    public void Add(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (_documents.ContainsKey(document.Id))
        {
            throw new InvalidOperationException(
                "Документ з таким номером вже існує.");
        }

        _documents.Add(document.Id, document);
    }

    public Document? GetById(int id)
    {
        return _documents.TryGetValue(id, out Document? document)
            ? document
            : null;
    }

    public IReadOnlyList<Document> GetAll()
    {
        return new List<Document>(_documents.Values);
    }
}
