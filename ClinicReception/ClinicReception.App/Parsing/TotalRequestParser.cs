
using System.Globalization;
using ClinicReception.Core.Domain;
using ClinicReception.Core.Documents;
using ClinicReception.Core.Errors;

namespace ClinicReception.App.Parsing;

public static class TotalRequestParser
{
    public static Result<TotalRequest> Parse(
        string documentIdText,
        string patientIdText,
        string patientName,
        string patientEmail,
        string sku,
        string quantityText,
        string priceText,
        string dateText)
    {
        if (!int.TryParse(documentIdText, out int documentId) ||
            documentId <= 0)
        {
            return Result<TotalRequest>.Failure(
                "Неправильний номер документа.");
        }

        if (!int.TryParse(patientIdText, out int patientId) ||
            patientId <= 0)
        {
            return Result<TotalRequest>.Failure(
                "Неправильний номер клієнта.");
        }

        if (!int.TryParse(quantityText, out int quantity) ||
            quantity <= 0)
        {
            return Result<TotalRequest>.Failure(
                "Кількість повинна бути більшою за нуль.");
        }

        if (!decimal.TryParse(
                priceText,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal price) || price < 0)
        {
            return Result<TotalRequest>.Failure(
                "Неправильна ціна.");
        }

        if (!DateOnly.TryParse(dateText, out DateOnly date))
        {
            return Result<TotalRequest>.Failure(
                "Неправильний формат дати.");
        }

        if (string.IsNullOrWhiteSpace(patientName) ||
            string.IsNullOrWhiteSpace(patientEmail) ||
            !patientEmail.Contains('@'))
        {
            return Result<TotalRequest>.Failure(
                "Перевірте ім'я та електронну пошту клієнта.");
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            return Result<TotalRequest>.Failure(
                "Введіть назву позиції.");
        }

        Patient patient = new(
            patientId,
            patientName,
            patientEmail);

        OrderLine line = new(
            sku,
            quantity,
            price);

        TotalRequest request = new(
            documentId,
            patient,
            new List<OrderLine> { line },
            date,
            null,
            DocumentStatus.New,
            0m);

        return Result<TotalRequest>.Success(request);
    }
}
