using System;

namespace Guardia.API.Models;

public partial class PersonelGirisBilgileri
{
    public long Id { get; set; }
    public long? PersonelId { get; set; }
    public string? SicilNo { get; set; }
    public string? Sifre { get; set; }
    public int HataliDenemeSayisi { get; set; }
    public bool HesapKilitliMi { get; set; }
    public int? DogrulamaKodu { get; set; }
    public DateTimeOffset? KodSonKullanma { get; set; }
    public virtual Personeller? Personel { get; set; }
}