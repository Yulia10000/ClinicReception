using System;
using System.Collections.Generic;

namespace ClinicReception.Core.Domain;

public sealed class Document
{
    private readonly List<OrderLine> _lines = new();

    public Document(int id, int patientId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(patientId);

        Id = id;
        PatientId = patientId;
        CreatedAt = createdAt;
        Status = DocumentStatus.New;
    }

    public int Id { get; }
    public int PatientId { get; }
    public DateOnly CreatedAt { get; }
    public DocumentStatus Status { get; private set; }

    public IReadOnlyList<OrderLine> Lines => _lines;

    public void AddLine(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != DocumentStatus.New)
        {
            throw new InvalidOperationException(
                "Додавати позиції можна лише до нового документа.");
        }

        _lines.Add(line);
    }

    public void Pay()
    {
        if (Status != DocumentStatus.New)
        {
            throw new InvalidOperationException(
                "Оплатити можна лише новий документ.");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Не можна оплатити порожній документ.");
        }

        Status = DocumentStatus.Paid;
    }

    public void Ship()
    {
        if (Status != DocumentStatus.Paid)
        {
            throw new InvalidOperationException(
                "Відправити можна лише оплачений документ.");
        }

        Status = DocumentStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status != DocumentStatus.New &&
            Status != DocumentStatus.Paid)
        {
            throw new InvalidOperationException(
                "Скасувати цей документ неможливо.");
        }

        Status = DocumentStatus.Cancelled;
    }
}
