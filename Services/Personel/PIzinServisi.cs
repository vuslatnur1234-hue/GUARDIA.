using Guardia.API.Data;
using Guardia.API.DTOs.PE;
using Guardia.API.Models;

namespace Guardia.API.Services.Personel
{
    public class PIzinServisi : IPIzinServisi
    {
        private readonly AppDbContext _context;

        public PIzinServisi(AppDbContext context)
        {
            _context = context;
        }

        public bool IzinTalepGonder(PIzinTalepModeli model)
        {
            try
            {
                var personel = _context.Personellers
                    .FirstOrDefault(p => p.SicilNo == model.SicilNo);

                if (personel == null) return false;

                var izin = new Izinler
                {
                    PersonelId = personel.Id,
                    IzinTuru = model.IzinTuru,
                    Aciklama = model.Aciklama,
                    BaslangicTarihi = model.BaslangicTarihi,
                    BitisTarihi = model.BitisTarihi,
                    OnayDurumu = "BEKLEMEDE"
                };

                _context.Izinlers.Add(izin);
                _context.SaveChanges();
                return true;
            }
            catch { return false; }
        }
    }
}