
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ClinicReception.Lab02.Domain
{
    /// <summary>
    /// Представляє заявку пацієнта на медичні послуги.
    /// </summary>
    public class AppointmentRequest
    {
        public string Id { get; set; }

        public string PatientName { get; set; }

        public List<string[]> Services { get; set; } = new List<string[]>();

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Створює заявку пацієнта.
        /// </summary>
        /// <param name="id">Ідентифікатор заявки.</param>
        /// <param name="patientName">Ім'я пацієнта.</param>
        public AppointmentRequest(string id, string patientName)
        {
            Id = id;
            PatientName = patientName;
            CreatedAt = DateTime.Now;
        }

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

                total = total + quantity * unitPrice;
                lineCount = lineCount + 1;
            }

            if (isRegularPatient && total > 1000)
            {
                total = total * 0.9m;
            }
            else if (total > 5000)
            {
                total = total * 0.85m;
            }

            if (lineCount > 10)
            {
                total = total - 100;
            }

            if (total < 0)
            {
                total = 0;
            }

            total = total + total * 0.2m;

            return Math.Round(total, 2);
        }

        /// <summary>
        /// Намагається змінити статус заявки.
        /// </summary>
        /// <param name="newStatus">Новий статус заявки.</param>
        /// <returns>True, якщо статус змінено успішно.</returns>
        public bool TryChangeStatus(int newStatus)
        {
            if (Status == 0 && newStatus == 1)
            {
                Status = 1;
                return true;
            }

            if (Status == 1 && newStatus == 2)
            {
                Status = 2;
                return true;
            }

            if (Status == 0 && newStatus == 3)
            {
                Status = 3;
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
                PatientName.Length > 2 &&
                Services.Count > 0 &&
                Services.Count < 100 &&
                Status >= 0 &&
                Status <= 3)
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

                report = report +
                    "Послуга: " + Services[i][0] +
                    "; кількість: " + Services[i][1] +
                    "; ціна: " + Services[i][2] +
                    "; сума: " + serviceTotal +
                    "\n";
            }

            report = report + "Разом: " + CalculateTotal(false) + "\n";

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
}



