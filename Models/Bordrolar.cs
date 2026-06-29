using System;
using System.Collections.Generic;

namespace Guardia.API.Models;

public partial class Bordrolar
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public long? PersonelId { get; set; }

    public string? AyYil { get; set; }

    public DateOnly? OdemeTarihi { get; set; }

    public string? Donem { get; set; }

    public decimal? NetOdeme { get; set; }

    public string? Durum { get; set; }

    public DateOnly? Tarih { get; set; }

    public string? DosyaUrl { get; set; }

    public bool? YeniMi { get; set; }

    public virtual Personeller? Personel { get; set; }
}
