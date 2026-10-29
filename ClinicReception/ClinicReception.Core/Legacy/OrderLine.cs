
namespace ClinicReception.Core.Legacy;

/// <summary>
/// Представляє позицію документа з кількістю та ціною.
/// </summary>
public sealed class OrderLine
{
    /// <summary>Артикул або назва позиції.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>Кількість одиниць.</summary>
    public int Quantity { get; set; }

    /// <summary>Ціна однієї одиниці.</summary>
    public decimal UnitPrice { get; set; }
}
