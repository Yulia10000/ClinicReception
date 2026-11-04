namespace ClinicReception.Core.Legacy;

public interface INotifier
{
    void Notify(string recipient, string subject, string body);
}
