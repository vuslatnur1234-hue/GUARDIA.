using Guardia.API.Data;
using Guardia.API.DTOs;
using System.Linq;

namespace Guardia.API.Services.Personel
{
    // Arayüzü güncelliyoruz
    public interface IPersonelServisi
    {
        PersonelModeli ProfilBilgileriniGetir(string sicilNo);
    }

    public class PersonelServisi : IPersonelServisi
    {
        private readonly AppDbContext _context;

        // Veritabanını çağırıyoruz
        public PersonelServisi(AppDbContext context)
        {
            _context = context;
        }

        public PersonelModeli ProfilBilgileriniGetir(string sicilNo)
        {
            // Veritabanından o sicile ait kişiyi bul
            var personel = _context.Personellers.FirstOrDefault(p => p.SicilNo == sicilNo);

            if (personel == null) return null!;

            // Bulunan kişinin bilgilerini DTO'ya doldurup gönder
            return new PersonelModeli
            {
                SicilNo = personel.SicilNo ?? "",
                AdSoyad = personel.AdSoyad ?? "Bilinmeyen Kullanıcı",
                Unvan = personel.Unvan ?? "Unvan Yok",
                ProfilFoto = personel.AvatarUrl ?? "img/ayse_nur_celik_profil.jpg.png"
            };
        }
    }
}