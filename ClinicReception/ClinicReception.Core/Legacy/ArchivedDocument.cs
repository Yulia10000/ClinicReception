using System;
using System.Collections.Generic;

namespace ClinicReception.Core.Legacy;

public sealed class ArchivedDocument
{
    private readonly LegacyDocument _document;

    public ArchivedDocument(
        LegacyDocument document,
        DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(document);

        _document = document;
        ArchivedAt = archivedAt;
    }

    public DateOnly ArchivedAt { get; }

    public int Id => _document.Id;

    public int Status => _document.Status;

    public IReadOnlyList<LegacyLine> Lines => _document.Lines;
}
