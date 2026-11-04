namespace ClinicReception.Core.Domain;

public sealed record Patient
{
    public int Id { get; }
    public string Name { get; }
    public string Email { get; }
    public bool IsRegular { get; init; }

    public Patient(int id, string name, string email)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (!email.Contains('@'))
        {
            throw new ArgumentException(
                "Електронна пошта має містити символ @.",
                nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
    }
}
