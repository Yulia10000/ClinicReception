namespace ClinicReception.Core.Domain;

public sealed class Appointment
{
    private readonly List<VisitItem> _items = new();

    public int Id { get; init; }

    public int PatientId { get; init; }

    public int DoctorId { get; init; }

    public VisitStatus Status { get; set; } = VisitStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<VisitItem> Items => _items;

    public void AddItem(VisitItem item)
    {
        _items.Add(item);
    }

    public decimal Total()
    {
        decimal sum = 0m;

        foreach (VisitItem item in _items)
        {
            sum += item.Amount;
        }

        return sum;
    }
}
