using System;
using System.Collections.Generic;
using ClinicReception.Core.Domain;
namespace ClinicReception.Core.Documents;

public sealed record TotalRequest(
    int DocumentId,
    Patient Client,
    IReadOnlyList<OrderLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    DocumentStatus Status,
    decimal DeliveryPrice);

public sealed record TotalResult(
    decimal Total,
    DocumentStatus Status);
