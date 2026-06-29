using Microsoft.AspNetCore.Mvc;
using Guardia.API.DTOs.PE;
using Guardia.API.DTOs.PeGiris;
using Guardia.API.Services.Personel;

namespace Guardia.API.Controllers
{
    [Route("api/psifreunuttum")]
    [ApiController]
    public class PSifreUnuttumKontrolcusu : ControllerBase
    {
        private readonly IPSifreUnuttumServisi _servisi;

        public PSifreUnuttumKontrolcusu(IPSifreUnuttumServisi servisi)
        {
            _servisi = servisi;
        }

        [HttpPost("sicildogrula")]
        public IActionResult SicilDogrula([FromBody] PSicilDogrulamaModeli model)
        {
            var sonuc = _servisi.SicilDogrula(model.SicilNo);
            if (sonuc.Basarili)
                return Ok(new { mesaj = "SMS gönderildi.", telefon = sonuc.Telefon, smsKodu = sonuc.SmsKodu });
            return BadRequest(new { mesaj = "Sicil numarası bulunamadı." });
        }

        [HttpPost("smsDogrula")]
        public IActionResult SmsDogrula([FromBody] SmsDogrulamaModeli model)
        {
            var sonuc = _servisi.SmsDogrula(model.Kod);
            if (sonuc)
                return Ok(new { mesaj = "Kod doğrulandı." });
            return BadRequest(new { mesaj = "Kod hatalı." });
        }

        [HttpPost("sifresifirla")]
        public IActionResult SifreSifirla([FromBody] PSifreSifirlamaModeli model)
        {
            var sonuc = _servisi.SifreSifirla(model);
            if (sonuc.Basarili)
                return Ok(new { mesaj = sonuc.Mesaj });
            return BadRequest(new { mesaj = sonuc.Mesaj });
        }
    }
}