using System;
using System.Collections.Generic;
using System.Globalization;
using Variant.Legacy;
using Xunit;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // Фіксуємо формат чисел
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private static List<ServiceLine> TwoLines() => new()
    {
        new ServiceLine { Code = "A-1", Qty = 2, Price = 150m },
        new ServiceLine { Code = "B-2", Qty = 1, Price = 400m }
    };

    // Перевірка чотирьох помилкових ситуацій
    [Theory]
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, string state, string mail,
        string expected)
    {
        var items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<ServiceLine>(),
            _ => null
        };

        var sut = new LegacyAppointmentProcessor();

        var actual = sut.Handle(
            1001, 7, "Іван", mail,
            "regular", 0, items, state, "UAH",
            new DateTime(2026, 3, 1), false);

        Assert.Equal(expected, actual);
    }

    // Перевірка повного тексту звіту
    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyAppointmentProcessor();

        var report = sut.Handle(
            1001, 7, "Іван", "a@b.c",
            "vip", 3, TwoLines(), "new", "UAH",
            new DateTime(2026, 3, 1), false);

        var expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "A-1 x2 = 300.00 UAH\n" +
            "B-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";

        Assert.Equal(expected, report);
    }

    // Перевірка різних видів знижок
    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    public void Preview_ByKind_ReturnsTotal(
        string kind, int done, decimal expected)
    {
        var sut = new LegacyAppointmentProcessor();

        var total = sut.Preview(kind, done, TwoLines());

        Assert.Equal(expected, total);
    }

    // Перевірка опису клієнта
    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var patient = new Patient
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = "vip",
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1)
        };

        var years = DateTime.Now.Year - 2020;

        var expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] " +
                       "<ivan@mail.com> стаж " + years;

        var actual = new LegacyAppointmentProcessor()
            .DescribeClient(patient);

        Assert.Equal(expected, actual);
    }

    // Перевірка межі безкоштовної доставки
    [Fact]
    public void Preview_AmountExactly1000_Returns1000()
    {
        var items = new List<ServiceLine>
        {
            new ServiceLine
            {
                Code = "C-1",
                Qty = 2,
                Price = 500m
            }
        };

        var actual = new LegacyAppointmentProcessor()
            .Preview("regular", 0, items);

        Assert.Equal(1000m, actual);
    }

    // Перевірка стану документа paid
    [Fact]
    public void Handle_PaidState_AcceptsDocument()
    {
        var actual = new LegacyAppointmentProcessor().Handle(
            1002, 7, "Іван", "a@b.c",
            "regular", 0, TwoLines(), "paid", "UAH",
            new DateTime(2026, 3, 1), false);

        Assert.StartsWith("Документ #1002\n", actual);
    }
}
