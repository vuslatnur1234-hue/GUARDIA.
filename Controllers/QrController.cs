using Guardia.API.Data;
using Guardia.API.Models;
using Guardia.API.Services.Personel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Guardia.API.Controllers
{
    [Route("api/qr")]
    [ApiController]
    public class QrController : ControllerBase
    {
        private readonly IQrServisi _qrServisi;
        private readonly AppDbContext _context;

        // Ofis koordinatları - nevada


        private const double MERKEZ_LAT = 41.020266;
        private const double MERKEZ_LNG = 29.008589;

        // Dudullu OSB, Nato Yolu Cd No:265, Ümraniye/İstanbul

        // private const double MERKEZ_LAT = 41.001717;
        //private const double MERKEZ_LNG = 29.176089;

        private const double IZIN_VERILEN_METRE = 100;

        public QrController(IQrServisi qrServisi, AppDbContext context)
        {
            _qrServisi = qrServisi;
            _context = context;
        }

        [HttpGet("getir/{sicilNo}")]
        public IActionResult Getir(string sicilNo, [FromQuery] double lat, [FromQuery] double lng)
        {
            // Konum kontrolü
            double mesafe = HesaplaMesafe(lat, lng, MERKEZ_LAT, MERKEZ_LNG);

            if (mesafe > IZIN_VERILEN_METRE)
            {
                return Unauthorized(new
                {
                    mesaj = "Konum dışındasınız.",
                    mesafeMetre = (int)mesafe
                });
            }

            var data = _qrServisi.QrOlustur(sicilNo);
            return Ok(data);
        }

        private double HesaplaMesafe(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLng = (lng2 - lng1) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        // GecisYap metodu 
        [HttpPost("gecis-yap")]
        public IActionResult GecisYap([FromBody] GecisIstekModeli istek)
        {
            if (istek == null || string.IsNullOrEmpty(istek.QrData))
                return BadRequest("Geçersiz istek: QR veri boş olamaz.");

            var personel = _context.Personellers
                .FirstOrDefault(p => p.QrKodData == istek.QrData);

            if (personel == null)
                return BadRequest("Geçersiz veya sisteme kayıtsız QR kod.");

            var bugunBaslangic = DateTimeOffset.Now.Date;

            var sonGecis = _context.PersonelGecisler
                .Where(g => g.PersonelId == personel.Id && g.GecisZamani >= bugunBaslangic)
                .OrderByDescending(g => g.GecisZamani)
                .FirstOrDefault();

            string yon = (sonGecis == null || sonGecis.GecisYonu == "ÇIKIŞ") ? "GİRİŞ" : "ÇIKIŞ";

            _context.PersonelGecisler.Add(new PersonelGecis
            {
                PersonelId = personel.Id,
                SicilNo = personel.SicilNo,
                GecisZamani = DateTimeOffset.Now,
                GecisYonu = yon
            });

            _context.SaveChanges();

            return Ok(new
            {
                durum = "basarili",
                mesaj = $"Turnike açıldı — {yon} onaylandı.",
                gecisYonu = yon,
                personelAd = personel.AdSoyad
            });
        }
    }

    public class GecisIstekModeli
    {
        public string QrData { get; set; }
        public string? PersonelAd { get; set; }
    }
}