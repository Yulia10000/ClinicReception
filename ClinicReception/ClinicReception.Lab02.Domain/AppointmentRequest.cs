
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ClinicReception.Lab02.Domain;

/// <summary>
/// Представляє заявку пацієнта на медичні послуги.
/// </summary>
public class AppointmentRequest
{
    private const decimal VatRate = 0.2m;
    private const decimal RegularPatientDiscount = 0.1m;
    private const decimal LargeOrderDiscount = 0.15m;
    private const decimal MinimumRegularOrder = 1000m;
    private const decimal MinimumLargeOrder = 5000m;
    private const decimal BulkDiscount = 100m;
    private const int BulkLineCount = 10;
    private const int MinimumPatientNameLength = 2;
    private const int MaximumServiceCount = 100;

    private const int NewStatus = 0;
    private const int ConfirmedStatus = 1;
    private const int CompletedStatus = 2;
    private const int CancelledStatus = 3;

    public AppointmentRequest(string id, string patientName)
    {
        Id = id;
        PatientName = patientName;
        CreatedAt = DateTime.Now;
    }

    public string Id { get; set; }

    public string PatientName { get; set; }

    public List<string[]> Services { get; set; } = new List<string[]>();

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Додає послугу до заявки.
    /// </summary>
    /// <param name="serviceName">Назва послуги.</param>
    /// <param name="quantity">Кількість послуг.</param>
    /// <param name="unitPrice">Ціна за одиницю.</param>
    public void Add(string serviceName, int quantity, decimal unitPrice)
    {
        string[] serviceData = new string[3];

        serviceData[0] = serviceName;
        serviceData[1] = quantity.ToString(CultureInfo.InvariantCulture);
        serviceData[2] = unitPrice.ToString(CultureInfo.InvariantCulture);

        Services.Add(serviceData);
    }

    /// <summary>
    /// Обчислює загальну вартість послуг у заявці.
    /// </summary>
    /// <param name="isRegularPatient">Вказує, чи є пацієнт постійним.</param>
    /// <returns>Загальна вартість послуг.</returns>
    public decimal CalculateTotal(bool isRegularPatient)
    {
        decimal total = 0;
        int lineCount = 0;

        for (int i = 0; i < Services.Count; i++)
        {
            int quantity = int.Parse(
                Services[i][1],
                CultureInfo.InvariantCulture);

            decimal unitPrice = decimal.Parse(
                Services[i][2],
                CultureInfo.InvariantCulture);

            total += quantity * unitPrice;
            lineCount++;
        }

        if (isRegularPatient && total > MinimumRegularOrder)
        {
            total *= 1 - RegularPatientDiscount;
        }
        else if (total > MinimumLargeOrder)
        {
            total *= 1 - LargeOrderDiscount;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscount;
        }

        if (total < 0)
        {
            total = 0;
        }

        total += total * VatRate;

        return Math.Round(total, 2);
    }

    /// <summary>
    /// Намагається змінити статус заявки.
    /// </summary>
    /// <param name="newStatus">Новий статус заявки.</param>
    /// <returns>True, якщо статус змінено; інакше false.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == NewStatus && newStatus == ConfirmedStatus)
        {
            Status = ConfirmedStatus;
            return true;
        }

        if (Status == ConfirmedStatus && newStatus == CompletedStatus)
        {
            Status = CompletedStatus;
            return true;
        }

        if (Status == NewStatus && newStatus == CancelledStatus)
        {
            Status = CancelledStatus;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє коректність заявки.
    /// </summary>
    /// <returns>True, якщо заявка відповідає умовам перевірки.</returns>
    public bool IsValid()
    {
        if (Id != null &&
            Id != "" &&
            PatientName != null &&
            PatientName.Length > MinimumPatientNameLength &&
            Services.Count > 0 &&
            Services.Count < MaximumServiceCount &&
            Status >= NewStatus &&
            Status <= CancelledStatus)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Створює текстовий звіт про послуги заявки.
    /// </summary>
    /// <returns>Текст звіту.</returns>
    public string CreateReport()
    {
        string report = "";

        for (int i = 0; i < Services.Count; i++)
        {
            int quantity = int.Parse(
                Services[i][1],
                CultureInfo.InvariantCulture);

            decimal unitPrice = decimal.Parse(
                Services[i][2],
                CultureInfo.InvariantCulture);

            decimal serviceTotal = quantity * unitPrice;

            report +=
                "Послуга: " + Services[i][0] +
                "; кількість: " + Services[i][1] +
                "; ціна: " + Services[i][2] +
                "; сума: " + serviceTotal +
                "\n";
        }

        report += "Разом: " + CalculateTotal(false) + "\n";

        return report;
    }

    /// <summary>
    /// Знаходить заявку за ідентифікатором.
    /// </summary>
    /// <param name="requests">Список заявок для пошуку.</param>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <returns>Знайдена заявка або null.</returns>
    public static AppointmentRequest? FindById(
        List<AppointmentRequest> requests,
        string id)
    {
        for (int i = 0; i < requests.Count; i++)
        {
            if (requests[i].Id == id)
            {
                return requests[i];
            }
        }

        return null;
    }
}



