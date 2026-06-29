using System;
using System.Collections.Generic;

namespace Guardia.API.Models;

public partial class Bildirimler
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public long? PersonelId { get; set; }

    public string? Baslik { get; set; }

    public string? Mesaj { get; set; }

    public string? Icon { get; set; }

    public bool? Okundu { get; set; }

    public DateTimeOffset? Tarih { get; set; }
    public virtual Personeller? Personel { get; set; }
}
