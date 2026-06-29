using Guardia.API.Data;
using Guardia.API.DTOs.PE;

namespace Guardia.API.Services.Personel
{
    public class QrServisi : IQrServisi
    {
        private readonly AppDbContext _context;

        public QrServisi(AppDbContext context)
        {
            _context = context;
        }

        public QrModeli QrOlustur(string sicilNo)
        {
            var personel = _context.Personellers
                .FirstOrDefault(p => p.SicilNo == sicilNo);

            if (personel == null) return new QrModeli
            {
                QrData = $"GUARDIA_{sicilNo}_{DateTime.Now.Ticks}",
                PersonelAd = "Bilinmiyor"
            };

            return new QrModeli
            {
                QrData = $"GUARDIA_{personel.AdSoyad?.Replace(" ", "_")}_{sicilNo}_{DateTime.Now.Ticks}",
                PersonelAd = personel.AdSoyad ?? ""
            };
        }
    }
}