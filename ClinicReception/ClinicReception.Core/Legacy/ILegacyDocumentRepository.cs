using System.Collections.Generic;

namespace ClinicReception.Core.Legacy;

public interface ILegacyDocumentRepository
{
    void Add(LegacyDocument document);

    LegacyDocument? GetById(int id);

    IReadOnlyList<LegacyDocument> GetAll();

    void SaveToFile(LegacyDocument document, string path);
}
