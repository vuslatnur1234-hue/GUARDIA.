using Guardia.API.Data;
using Guardia.API.DTOs.PE;
using System.Linq;

namespace Guardia.API.Services.Personel
{
    // interface
    public interface IPersonelMerkeziServis
    {
        PersonelPanelModeli PanelBilgisiniGetir(string sicilNo);
        PersonelProfilModeli ProfilBilgileriniGetir(string sicilNo);
        bool ProfilGuncelle(PersonelProfilModeli model);
    }

    // ortak servis sınıfı
    public class PersonelMerkeziServis : IPersonelMerkeziServis
    {
        private readonly AppDbContext _context;

        public PersonelMerkeziServis(AppDbContext context)
        {
            _context = context;
        }

        // panel
        public PersonelPanelModeli PanelBilgisiniGetir(string sicilNo)
        {
            var personel = _context.Personellers.FirstOrDefault(p => p.SicilNo == sicilNo);
            if (personel == null) return null!;

            return new PersonelPanelModeli
            {
                AdSoyad = personel.AdSoyad ?? "Bilinmiyor",
                Unvan = personel.Unvan ?? "Unvan Yok",
                FotoUrl = string.IsNullOrEmpty(personel.AvatarUrl) ? null : personel.AvatarUrl,
                VardiyaBitis = "18:00",
                BildirimVarMi = _context.Bildirimlers.Any(b => b.PersonelId == personel.Id && b.Okundu == false),
                OkunmamisBildirimSayisi = _context.Bildirimlers.Count(b => b.PersonelId == personel.Id && b.Okundu == false)
            };
        }

        // profil getirme işlemleri
        public PersonelProfilModeli ProfilBilgileriniGetir(string sicilNo)
        {
            var personel = _context.Personellers.FirstOrDefault(p => p.SicilNo == sicilNo);
            if (personel == null) return null!;

            return new PersonelProfilModeli
            {
                SicilNo = personel.SicilNo,
                Telefon = personel.Telefon,
                AcilDurumYakini = personel.AcilDurumYakini,
                AcilDurumTelefon = personel.AcilDurumNo,
                Adres = personel.Adres,
                AdSoyad = personel.AdSoyad,
                Unvan = personel.Unvan,
                FotoUrl = personel.AvatarUrl

            };
        }

        // PersonelMerkeziServis.cs - ProfilGuncelle metodu
        public bool ProfilGuncelle(PersonelProfilModeli model)
        {
            if (string.IsNullOrEmpty(model.SicilNo)) return false;

            var personel = _context.Personellers
                .FirstOrDefault(p => p.SicilNo == model.SicilNo);
            if (personel == null) return false;

            personel.YeniTelefonTalebi = model.Telefon;

            string adresTalebi = model.Adres ?? "";
            if (!string.IsNullOrEmpty(model.AcilDurumYakini))
                adresTalebi += " | YAKIN: " + model.AcilDurumYakini;
            if (!string.IsNullOrEmpty(model.AcilDurumTelefon))
                adresTalebi += " | YAKIN_TEL: " + model.AcilDurumTelefon;

            personel.YeniAdresTalebi = adresTalebi;
            personel.IkGuncellemeDurumu = "BEKLEMEDE";

            _context.SaveChanges();
            return true;
        }
    }
}