using Guardia.API.DTOs.PE;
using Guardia.API.Services.ai;
using Guardia.API.Services.InsanKaynaklari;
using Guardia.API.Services.Personel;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{
    [Route("api/pprofil")]
    [ApiController]
    public class PProfilController : ControllerBase
    {
        private readonly IPersonelMerkeziServis _servis;
        private readonly IIkService _ikServisi;

        public PProfilController(IPersonelMerkeziServis servis, IIkService ikServisi)
        {
            _servis = servis;
            _ikServisi = ikServisi;
        }

        [HttpGet("getir")]
        public IActionResult Getir([FromQuery] string sicilNo)
        {
            if (string.IsNullOrEmpty(sicilNo)) return BadRequest(new { mesaj = "Sicil No gerekli." });

            var data = _servis.ProfilBilgileriniGetir(sicilNo);
            if (data == null) return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

            return Ok(data);
        }

        // PProfilKontrolcusu.cs
        [HttpPost("guncelle")]
        public IActionResult ProfilGuncelle([FromBody] PersonelProfilModeli model)
        {
            if (string.IsNullOrEmpty(model.SicilNo))
                return BadRequest("Sicil no eksik.");

            var veriler = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(model.Telefon))
                veriler["telefon"] = model.Telefon;

            if (!string.IsNullOrEmpty(model.Adres))
                veriler["adres"] = model.Adres;

            if (!string.IsNullOrEmpty(model.AcilDurumYakini))
                veriler["yakinAdiSoyadi"] = model.AcilDurumYakini;

    
            _ikServisi.GuncellemeTalebiOlustur(model.SicilNo, veriler);

            return Ok(new { mesaj = "Talebiniz İK onayına iletildi." });
        }
    }
}