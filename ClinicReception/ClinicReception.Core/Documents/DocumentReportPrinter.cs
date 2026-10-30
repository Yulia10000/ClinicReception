using System;
using System.IO;

namespace ClinicReception.Core.Documents;

public sealed class DocumentReportPrinter
{
    private readonly TextWriter _output;

    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
