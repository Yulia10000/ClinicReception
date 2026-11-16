
using System;
using ClinicReception.App.Parsing;
using ClinicReception.Core.Abstractions;
using ClinicReception.Core.Domain;
using ClinicReception.Core.Documents;
using ClinicReception.Core.Legacy;
using ClinicReception.Core.Policies;
using ClinicReception.Core.Repositories;
using ClinicReception.Core.Services;
using Microsoft.Extensions.Logging;

// Налаштування логування
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });

    builder.SetMinimumLevel(LogLevel.Debug);
});

// Створення основного сервісу
IDocumentRepository repository =
    new InMemoryDocumentRepository();

IPricingPolicy pricing =
    new DiscountPricingPolicy(0.05m);

ILogger<DocumentService> logger =
    loggerFactory.CreateLogger<DocumentService>();

DocumentService service =
    new DocumentService(repository, pricing, logger);

// Створення документа
Document document = new(
    1,
    100,
    DateOnly.FromDateTime(DateTime.Today));

document.AddLine(new OrderLine("A1", 2, 250m));

service.Place(document);

Console.WriteLine(
    $"Сума документа: {service.TotalOf(document):0.00}");
// Перевірка рівня Warning
Document emptyDocument = new(
    3,
    100,
    DateOnly.FromDateTime(DateTime.Today));

service.TotalOf(emptyDocument);

// Перевірка рівня Error
try
{
    service.Place(null!);
}
catch (ArgumentNullException)
{
    Console.WriteLine("Перевірку рівня Error виконано.");
}

// Налаштування старого прикладу
ILegacyDocumentRepository legacyRepository =
    new FileOrderStore();

ILegacyPricingPolicy legacyPricing =
    new VipPricingPolicy();

INotifier notifier =
    new SmtpMailer();

OrderManager legacyService = new(
    legacyRepository,
    legacyPricing,
    notifier);

// Створення старого документа
LegacyDocument legacyDocument = new()
{
    Id = 2,
    Email = "client@example.com"
};

legacyDocument.AddLine(new LegacyLine
{
    Sku = "B1",
    Quantity = 2,
    UnitPrice = 500m
});

decimal legacyTotal = legacyService.Place(legacyDocument);

Console.WriteLine(
    $"Сума документа VIP-клієнта: {legacyTotal:0.00}");

// Введення даних із повторною перевіркою
while (true)
{
    Console.WriteLine("\nВведення даних документа:");

    Console.Write("Номер документа: ");
    string documentId = Console.ReadLine() ?? "";

    Console.Write("Номер клієнта: ");
    string patientId = Console.ReadLine() ?? "";

    Console.Write("Ім'я клієнта: ");
    string patientName = Console.ReadLine() ?? "";

    Console.Write("Електронна пошта: ");
    string patientEmail = Console.ReadLine() ?? "";

    Console.Write("Назва позиції: ");
    string sku = Console.ReadLine() ?? "";

    Console.Write("Кількість: ");
    string quantity = Console.ReadLine() ?? "";

    if (!int.TryParse(quantity, out int quantityValue) ||
        quantityValue <= 0)
    {
        Console.WriteLine(
            "Помилка вводу: Кількість — ціле число більше 0");

        Console.WriteLine("Спробуйте ще раз.");
        continue;
    }

    Console.Write("Ціна: ");
    string price = Console.ReadLine() ?? "";

    Console.Write("Дата (дд.мм.рррр): ");
    string date = Console.ReadLine() ?? "";

    var parseResult = TotalRequestParser.Parse(
        documentId,
        patientId,
        patientName,
        patientEmail,
        sku,
        quantity,
        price,
        date);

    if (!parseResult.IsSuccess)
    {
        Console.WriteLine(
            $"Помилка вводу: {parseResult.Error}");

        Console.WriteLine("Спробуйте ще раз.");
        continue;
    }

    if (parseResult.Value != null)
    {
        TotalResult result =
            DocumentTotalCalculator.Calculate(parseResult.Value);

        Console.WriteLine(
            $"Розрахована сума: {result.Total:0.00}");
    }

    break;
}
