using System;
using System.Collections.Generic;

namespace Guardia.API.Models;

public partial class Maaslar
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public long? PersonelId { get; set; }

    public string? Donem { get; set; }

    public decimal? NetMaas { get; set; }

    public decimal? Prim { get; set; }

    public decimal? Kesinti { get; set; }

    public DateOnly? OdemeTarihi { get; set; }

    public string? Durum { get; set; }

    public virtual Personeller? Personel { get; set; }

    public decimal? SgkPayi { get; set; }
    public decimal? GelirVergisi { get; set; }
}
