using System;
using System.Collections.Generic;

namespace Guardia.API.Models;

public partial class Talepler
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public long? PersonelId { get; set; }

    public string? TalepTipi { get; set; }

    public DateOnly? BaslangicTarihi { get; set; }

    public DateOnly? BitisTarihi { get; set; }

    public int? ToplamGun { get; set; }

    public string? Aciklama { get; set; }

    public string? OnayDurumu { get; set; }

    public long? OnaylayanId { get; set; }

    public string? RedNedeni { get; set; }

    public DateTimeOffset? OnayTarihi { get; set; }

    public virtual Personeller? Personel { get; set; }
}
