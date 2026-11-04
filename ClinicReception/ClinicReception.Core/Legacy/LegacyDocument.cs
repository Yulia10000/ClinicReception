using System.Collections.Generic;

namespace ClinicReception.Core.Legacy;

public class LegacyLine
{
    public string Sku { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class LegacyDocument
{
    public int Id { get; set; }
    public int Status { get; set; }
    public decimal Total { get; set; }
    public string Email { get; set; } = "";
    public List<LegacyLine> Lines { get; set; } = new();

    public void AddLine(LegacyLine line)
    {
        Lines.Add(line);
    }
}
