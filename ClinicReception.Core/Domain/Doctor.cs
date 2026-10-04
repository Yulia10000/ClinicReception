namespace ClinicReception.Core.Domain;

public sealed class Doctor
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Specialization { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;
}
