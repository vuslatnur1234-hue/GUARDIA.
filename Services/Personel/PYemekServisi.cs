using Guardia.API.Data;
using Guardia.API.DTOs.PE;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Guardia.API.Services.Personel
{
    public class PYemekServisi : IPYemekServisi
    {
        private readonly AppDbContext _context;

        public PYemekServisi(AppDbContext context)
        {
            _context = context;
        }

        public List<PYemekMenuModeli> HaftaMenusuGetir()
        {
            var gunler = new[] { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma" };
            var dbMenuler = _context.YemekMenusus.ToList()
                .GroupBy(m => m.Gun)
                .Select(g => g.First())
                .Where(m => gunler.Contains(m.Gun))
                .OrderBy(m => Array.IndexOf(gunler, m.Gun))
                .ToList();

            string bugunGun = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("tr-TR"));

            var menuListesi = dbMenuler.Select(m =>
            {
                
                string gelenVeri = m.IcecekTatli ?? "";
                string icecek = "";
                string tatli = "";

              
                if (gelenVeri.Contains("Ayran") || gelenVeri.Contains("Suyu") || gelenVeri.Contains("Limonata") || gelenVeri.Contains("Kola"))
                {
                    icecek = gelenVeri;
                }
                else
                {
                    tatli = gelenVeri;
                }

                return new PYemekMenuModeli
                {
                    Gun = m.Gun ?? "",
                    Corba = m.Corba ?? "",
                    AnaYemek = m.AnaYemek ?? "",
                    Esantiyon = m.YanUrun ?? "",      
                    Tatli = tatli,                    
                    Icerek = icecek,                
                    Meyve = "-",                       
                    BugunMu = (m.Gun != null && m.Gun.Equals(bugunGun, StringComparison.OrdinalIgnoreCase))
                };
            }).ToList();

            return menuListesi;
        }
    }
}