namespace ClinicReception.Core.Domain;

public sealed class Patient
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public int Age { get; init; }

    public string Phone { get; init; } = string.Empty;
}
