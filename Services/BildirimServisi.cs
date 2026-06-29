/*using Guardia.API.Data;
using Guardia.API.DTOs;
using Guardia.API.Services.Interfaces;

namespace Guardia.API.Services
{
    public class BildirimServisi : IBildirimServisi
    {
        private readonly AppDbContext _context;

        public BildirimServisi(AppDbContext context)
        {
            _context = context;
        }

        public List<BildirimModeli> BildirimleriGetir(string sicilNo)
        {
            var personel = _context.Personellers
                .FirstOrDefault(p => p.SicilNo == sicilNo);

            if (personel == null) return new List<BildirimModeli>();

            return _context.Bildirimlers
                .Where(b => b.PersonelId == personel.Id)
                .OrderByDescending(b => b.Tarih)
                .Select(b => new BildirimModeli
                {
                    Baslik = b.Baslik ?? "",
                    Metin = b.Mesaj ?? "",
                    Ikon = "fa-solid fa-bell",
                    Zaman = b.Tarih.HasValue ? b.Tarih.Value.ToString("dd.MM.yyyy HH:mm") : "",
                    Okunmadi = b.Okundu == false
                })
                .ToList();
        }
    }
}*/