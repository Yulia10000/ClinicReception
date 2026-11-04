namespace ClinicReception.Core.Legacy;

public interface IOrderStore
{
    void Add(LegacyDocument document);

    LegacyDocument? GetById(int id);

    void SaveToFile(LegacyDocument document, string path);

    void SendEmail(string to, string body);

    string ExportCsv();

    void Backup(string folder);

    void PrintInvoice(LegacyDocument document);
}
