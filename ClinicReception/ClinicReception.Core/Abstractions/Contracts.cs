using System.Collections.Generic;
using ClinicReception.Core.Domain;

namespace ClinicReception.Core.Abstractions;

public interface IDocumentRepository
{
    void Add(Document document);

    Document? GetById(int id);

    IReadOnlyList<Document> GetAll();
}

public interface IPricingPolicy
{
    decimal PriceOf(OrderLine line);
}
