using System;
using System.Collections.Generic;

namespace Variant.Legacy;

public class OrderRequest
{
    public int DocId { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = "";
    public string? ClientMail { get; set; }
    public ClientKind ClientKind { get; set; }
    public int ClientDone { get; set; }
    public List<ServiceLine>? Items { get; set; }
    public DocState State { get; set; }
    public string Currency { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool SendMail { get; set; }
}