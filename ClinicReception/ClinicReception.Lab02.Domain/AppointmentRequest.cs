

using System;
using System.Collections.Generic;
using System.Globalization;

namespace ClinicReception.Lab02.Domain;

/// <summary>
/// Представляє заявку на медичний прийом із переліком послуг.
/// </summary>
public class AppointmentRequest
{
    private const int StatusNew = 0;
    private const int StatusConfirmed = 1;
    private const int StatusCompleted = 2;
    private const int StatusCancelled = 3;

    private const decimal VatRate = 0.2m;
    private const decimal RegularDiscountThreshold = 1000m;
    private const decimal RegularDiscountRate = 0.10m;
    private const decimal LargeRequestThreshold = 5000m;
    private const decimal LargeRequestDiscountRate = 0.15m;
    private const int BulkLineCount = 10;
    private const decimal BulkDiscountAmount = 100m;
    private const int MaxLineCount = 100;
    private const int MinPatientNameLength = 2;

    private readonly List<string[]> _services = new();

    /// <summary>
    /// Ініціалізує нову заявку на прийом.
    /// </summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="patientName">Ім'я пацієнта.</param>
    public AppointmentRequest(string id, string patientName)
    {
        Id = id;
        PatientName = patientName;
        CreatedAt = DateTime.Now;
        Status = StatusNew;
    }

    /// <summary>
    /// Повертає ідентифікатор заявки.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Повертає ім'я пацієнта.
    /// </summary>
    public string PatientName { get; private set; }

    /// <summary>
    /// Повертає дату й час створення заявки.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Повертає поточний числовий статус заявки.
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// Додає послугу до заявки.
    /// </summary>
    /// <param name="serviceName">Назва медичної послуги.</param>
    /// <param name="quantity">Кількість послуг.</param>
    /// <param name="unitPrice">Вартість однієї послуги.</param>
    public void AddService(string serviceName, int quantity, decimal unitPrice)
    {
        string[] serviceData = new string[3];
        serviceData[0] = serviceName;
        serviceData[1] = quantity.ToString(CultureInfo.CurrentCulture);
        serviceData[2] = unitPrice.ToString(CultureInfo.CurrentCulture);

        _services.Add(serviceData);
    }

    /// <summary>
    /// Обчислює підсумкову вартість послуг із урахуванням знижок і ПДВ.
    /// </summary>
    /// <param name="isRegularPatient">Ознака постійного пацієнта.</param>
    /// <returns>Підсумкова вартість послуг.</returns>
    public decimal CalculateTotal(bool isRegularPatient)
    {
        decimal total = 0;
        int lineCount = 0;

        foreach (string[] service in _services)
        {
            int quantity = int.Parse(
                service[1],
                CultureInfo.CurrentCulture);

            decimal unitPrice = decimal.Parse(
                service[2],
                CultureInfo.CurrentCulture);

            total += quantity * unitPrice;
            lineCount++;
        }

        if (isRegularPatient && total > RegularDiscountThreshold)
        {
            total *= 1m - RegularDiscountRate;
        }
        else if (total > LargeRequestThreshold)
        {
            total *= 1m - LargeRequestDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму після застосування знижок.
        total += total * VatRate;

        return Math.Round(total, 2);
    }

    /// <summary>
    /// Намагається змінити статус заявки.
    /// </summary>
    /// <param name="newStatus">Новий статус заявки.</param>
    /// <returns>
    /// true, якщо перехід дозволений; інакше false.
    /// </returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusConfirmed)
        {
            Status = StatusConfirmed;
            return true;
        }

        if (Status == StatusConfirmed && newStatus == StatusCompleted)
        {
            Status = StatusCompleted;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє коректність основних даних заявки.
    /// </summary>
    /// <returns>true, якщо дані відповідають заданим умовам.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Id)
            && !string.IsNullOrEmpty(PatientName)
            && PatientName.Length > MinPatientNameLength
            && _services.Count > 0
            && _services.Count < MaxLineCount
            && Status >= StatusNew
            && Status <= StatusCancelled;
    }

    /// <summary>
    /// Формує текстовий звіт про послуги заявки.
    /// </summary>
    /// <returns>Текстовий звіт із сумою.</returns>
    public string CreateReport()
    {
        string report = string.Empty;

        foreach (string[] service in _services)
        {
            int quantity = int.Parse(
                service[1],
                CultureInfo.CurrentCulture);

            decimal unitPrice = decimal.Parse(
                service[2],
                CultureInfo.CurrentCulture);

            decimal serviceTotal = quantity * unitPrice;

            report +=
                $"Послуга: {service[0]}; кількість: {service[1]}; "
                + $"ціна: {service[2]}; сума: {serviceTotal}\n";
        }

        report += $"Разом: {CalculateTotal(false)}\n";

        return report;
    }

    /// <summary>
    /// Знаходить заявку за ідентифікатором у переданому списку.
    /// </summary>
    /// <param name="requests">Список заявок для пошуку.</param>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <returns>Знайдена заявка або null, якщо її немає.</returns>
    public static AppointmentRequest? FindById(
        List<AppointmentRequest> requests,
        string id)
    {
        foreach (AppointmentRequest request in requests)
        {
            if (request.Id == id)
            {
                return request;
            }
        }

        return null;
    }
}



