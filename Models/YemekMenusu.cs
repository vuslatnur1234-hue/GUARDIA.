using System;
using System.Collections.Generic;

namespace Guardia.API.Models;

public partial class YemekMenusu
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? Gun { get; set; }

    public string? Tarih { get; set; }

    public string? Corba { get; set; }

    public string? AnaYemek { get; set; }

    public string? YanUrun { get; set; }

    public string? IcecekTatli { get; set; }

    public string? Kalori { get; set; }
}
