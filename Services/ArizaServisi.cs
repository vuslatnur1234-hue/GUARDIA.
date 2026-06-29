using Guardia.API.Data;
using Guardia.API.DTOs;
using Guardia.API.Models;
using Guardia.API.Services.Interfaces;

namespace Guardia.API.Services
{
    public class ArizaServisi : IArizaServisi
    {
        private readonly AppDbContext _context;

        public ArizaServisi(AppDbContext context)
        {
            _context = context;
        }

        public bool ArizaKaydet(ArizaKayitModeli ariza)
        {
            try
            {
                var personel = _context.Personellers
                    .FirstOrDefault(p => p.SicilNo == ariza.SicilNo);

                if (personel == null) return false;

                var yeniAriza = new Arizalar
                {
                    PersonelId = personel.Id,
                    Baslik = ariza.Makine,
                    Kategori = ariza.Tip,
                    Oncelik = ariza.Aciliyet,
                    Aciklama = ariza.Detay,
                    Durum = "BEKLEMEDE",
                    TakipNo = "#ARZ-2026-" + new Random().Next(1000, 9999)
                };

                _context.Arizalars.Add(yeniAriza);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
                return false;
            }
        }
    }
}