using System;
using System.Collections.Generic;
using System.Globalization;
using Variant.Legacy;
using Xunit;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private static List<ServiceLine> TwoLines() => new()
    {
        new ServiceLine { Code = "A-1", Qty = 2, Price = 150m },
        new ServiceLine { Code = "B-2", Qty = 1, Price = 400m }
    };

    private static OrderRequest CreateRequest(
        List<ServiceLine>? items,
        DocState state,
        string mail,
        ClientKind kind = ClientKind.Regular)
    {
        return new OrderRequest
        {
            DocId = 1001,
            ClientId = 7,
            ClientName = "Іван",
            ClientMail = mail,
            ClientKind = kind,
            ClientDone = 0,
            Items = items,
            State = state,
            Currency = "UAH",
            CreatedAt = new DateTime(2026, 3, 1),
            SendMail = false
        };
    }

    [Theory]
    [InlineData(null, DocState.New, "a@b.c", "ERR: null")]
    [InlineData("empty", DocState.New, "a@b.c", "ERR: empty")]
    [InlineData("two", DocState.Draft, "a@b.c", "ERR: state")]
    [InlineData("two", DocState.New, "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, DocState state, string mail,
        string expected)
    {
        var items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<ServiceLine>(),
            _ => null
        };

        var sut = new LegacyAppointmentProcessor();
        var request = CreateRequest(items, state, mail);

        var actual = sut.Handle(request);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyAppointmentProcessor();

        var request = CreateRequest(
            TwoLines(), DocState.New, "a@b.c", ClientKind.Vip);

        request.ClientDone = 3;

        var report = sut.Handle(request);

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

    [Theory]
    [InlineData(ClientKind.Regular, 0, 760)]
    [InlineData(ClientKind.Vip, 0, 655)]
    [InlineData(ClientKind.Staff, 0, 550)]
    [InlineData(ClientKind.Regular, 11, 725)]
    public void Preview_ByKind_ReturnsTotal(
        ClientKind kind, int done, decimal expected)
    {
        var sut = new LegacyAppointmentProcessor();

        var total = sut.Preview(kind, done, TwoLines());

        Assert.Equal(expected, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var patient = new Patient
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = ClientKind.Vip,
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
            .Preview(ClientKind.Regular, 0, items);

        Assert.Equal(1000m, actual);
    }

    [Fact]
    public void Handle_PaidState_AcceptsDocument()
    {
        var request = CreateRequest(
            TwoLines(), DocState.Paid, "a@b.c");

        request.DocId = 1002;

        var actual = new LegacyAppointmentProcessor().Handle(request);

        Assert.StartsWith("Документ #1002\n", actual);
    }
}
