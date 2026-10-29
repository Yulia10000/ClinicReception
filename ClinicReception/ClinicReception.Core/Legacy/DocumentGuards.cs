using System;
using System.Collections.Generic;

namespace ClinicReception.Core.Legacy;

/// <summary>
/// Перевіряє правильність вхідних даних перед обробкою.
/// </summary>
public static class DocumentGuards
{
    /// <summary>
    /// Перевіряє основні дані документа.
    /// </summary>
    public static void EnsureHeaderValid(
        int documentId,
        string clientName,
        string clientEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientEmail);

        if (!clientEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @.",
                nameof(clientEmail));
        }
    }

    /// <summary>
    /// Перевіряє одну позицію документа.
    /// </summary>
    public static void EnsureLineValid(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.Sku);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            line.Quantity,
            nameof(line.Quantity));

        ArgumentOutOfRangeException.ThrowIfNegative(
            line.UnitPrice,
            nameof(line.UnitPrice));
    }

    /// <summary>
    /// Перевіряє список позицій документа.
    /// </summary>
    public static void EnsureLinesValid(
        IReadOnlyList<OrderLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        if (lines.Count == 0)
        {
            throw new ArgumentException(
                "Документ не містить жодної позиції.",
                nameof(lines));
        }

        foreach (OrderLine line in lines)
        {
            EnsureLineValid(line);
        }
    }
}
