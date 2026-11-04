namespace ClinicReception.Core.Legacy;

public class SmtpMailer : INotifier
{
    public void Notify(string recipient, string subject, string body)
    {
        // Тут може бути реалізоване надсилання повідомлення.
    }
}
