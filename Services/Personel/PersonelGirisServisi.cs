using System.Linq;
using Guardia.API.Data;
using Guardia.API.DTOs.PeGiris;
using Guardia.API.Models;

namespace Guardia.API.Services.Personel
{
    public class PersonelGirisServisi : IPersonelGiris
    {
        private readonly AppDbContext _context;

        public PersonelGirisServisi(AppDbContext context)
        {
            _context = context;
        }

        public bool GirisYap(PersonelGirisBilgisi bilgi)
        {
            var personel = _context.Personellers
                .FirstOrDefault(p => p.SicilNo == bilgi.SicilNo);

            if (personel == null)
                return false;

            var girisBilgisi = _context.PersonelGirisBilgileris
                .FirstOrDefault(g => g.SicilNo == bilgi.SicilNo);

            if (girisBilgisi == null)
                return false;

            if (girisBilgisi.HesapKilitliMi)
                return false;

            if (BCrypt.Net.BCrypt.Verify(bilgi.Sifre, girisBilgisi.Sifre))
            {
                girisBilgisi.HataliDenemeSayisi = 0;
                _context.SaveChanges();
                return true;
            }
            else
            {
                girisBilgisi.HataliDenemeSayisi++;

                if (girisBilgisi.HataliDenemeSayisi >= 3)
                    girisBilgisi.HesapKilitliMi = true;

                _context.SaveChanges();
                return false;
            }
        }
    }
}