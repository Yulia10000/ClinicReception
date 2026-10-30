
using System;
using System.Globalization;
using System.Text;
using ClinicReception.Core.Legacy;

namespace ClinicReception.Core.Documents;

public static class DocumentReport
{
    public static string TextOf(
        TotalRequest request,
        TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);

        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();

        text.AppendLine(culture, $"Документ № {request.DocumentId}");
        text.AppendLine(culture, $"Клієнт: {request.Client.Name}");

        foreach (OrderLine line in request.Lines)
        {
            text.AppendLine(
                culture,
                $"{line.Sku} x {line.Quantity}");
        }

        text.AppendLine(string.Format(
            culture,
            "Разом: {0:F2}",
            result.Total));

        text.AppendLine(culture, $"Стан: {result.Status}");

        return text.ToString();
    }
}
