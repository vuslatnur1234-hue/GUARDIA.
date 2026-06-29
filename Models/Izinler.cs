using System;
using System.Collections.Generic;

namespace Guardia.API.Models;

public partial class Izinler
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public long? PersonelId { get; set; }

    public DateOnly? BaslangicTarihi { get; set; }

    public DateOnly? BitisTarihi { get; set; }

    public string? IzinTuru { get; set; }

    public string? Aciklama { get; set; }

    public string? OnayDurumu { get; set; }

    public virtual Personeller? Personel { get; set; }
}
