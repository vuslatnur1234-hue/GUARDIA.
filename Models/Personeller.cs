using System;
using System.Collections.Generic;

namespace Guardia.API.Models;

public partial class Personeller
{
    public long Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? AdSoyad { get; set; }
    public string? SicilNo { get; set; }
    public string? Sifre { get; set; }
    public string? Email { get; set; }
    public string? Unvan { get; set; }
    public string? TcNo { get; set; }

    public string? Departman { get; set; }

    public DateOnly? IseGirisTarihi { get; set; }
    public string? Telefon { get; set; }
    public string? AcilDurumNo { get; set; }
    public string? AvatarUrl { get; set; }
    public int? IzinBakiyesi { get; set; }
    public string? VardiyaGrubu { get; set; }
    public string? QrKodData { get; set; }
    public string? Adres { get; set; }
    public string? AcilDurumYakini { get; set; }
    public string? KanGrubu { get; set; }
    public string? YeniAdresTalebi { get; set; }
    public string? YeniTelefonTalebi { get; set; }
    public bool Durum { get; set; } = true; 
    public DateTime? AyrilmaTarihi { get; set; } 

    public string? IkGuncellemeDurumu { get; set; }
    public DateTimeOffset? MesaiBaslangic { get; set; }
    public int? DogrulamaKodu { get; set; }
    public DateTimeOffset? KodSonKullanma { get; set; }
    public DateTime? DogumTarihi { get; set; }
    public string? EskiSifreler { get; set; }

    
    public string? Iban { get; set; }
    public string? BankaAdi { get; set; }
    public string? HesapNumarasi { get; set; }
    public string? MedeniDurum { get; set; }
    public string? YakinlikDerecesi { get; set; }
    public string? MaasMiktari { get; set; }
    public string? AskerlikDurumu { get; set; }

    // İlişkiler
    public virtual ICollection<Bildirimler> Bildirimlers { get; set; } = new List<Bildirimler>();
    public virtual ICollection<Bordrolar> Bordrolars { get; set; } = new List<Bordrolar>();
    public virtual ICollection<Izinler> Izinlers { get; set; } = new List<Izinler>();
    public virtual ICollection<Maaslar> Maaslars { get; set; } = new List<Maaslar>();
    public virtual ICollection<Talepler> Taleplers { get; set; } = new List<Talepler>();
}