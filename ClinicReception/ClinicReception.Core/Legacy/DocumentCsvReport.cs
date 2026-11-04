using System.Collections.Generic;
using System.Text;

namespace ClinicReception.Core.Legacy;

public sealed class DocumentCsvReport
{
    public string BuildCsv(List<LegacyDocument> documents)
    {
        StringBuilder text = new();
        text.AppendLine("id;total;status");

        foreach (LegacyDocument document in documents)
        {
            text.Append(document.Id);
            text.Append(';');
            text.Append(document.Total);
            text.Append(';');
            text.AppendLine(document.Status.ToString());
        }

        return text.ToString();
    }
}
