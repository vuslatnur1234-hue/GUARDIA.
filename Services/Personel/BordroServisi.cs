/*using Guardia.API.Data;
using Guardia.API.DTOs.PE;

namespace Guardia.API.Services.Personel
{
    public class BordroServisi : IPBordroServisi
    {
        private readonly AppDbContext _context;
        public BordroServisi(AppDbContext context)
        {
            _context = context;
        }

        public List<PBordroModeli> BordrolariGetir(string sicilNo)
        {
            var personel = _context.Personellers
                .FirstOrDefault(p => p.SicilNo == sicilNo);

            if (personel == null) return new List<PBordroModeli>();

            // bordrolar yerine maaslar tablosundan okuyoruz
            return _context.Maaslars
                .Where(m => m.PersonelId == personel.Id)
                .OrderByDescending(m => m.OdemeTarihi)
                .Select(m => new PBordroModeli
                {
                    Ay = m.Donem ?? "-",
                    Tarih = m.OdemeTarihi.HasValue
                        ? m.OdemeTarihi.Value.ToString("dd.MM.yyyy")
                        : "-",
                    YeniMi = false // maaslar tablosunda yeni_mi yok, false sabit
                })
                .ToList();
        }
    }
}*/