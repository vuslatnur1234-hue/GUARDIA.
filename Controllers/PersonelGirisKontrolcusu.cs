using Guardia.API.DTOs.PeGiris;
using Guardia.API.Services.Personel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Guardia.API.Data;

namespace Guardia.API.Controllers
{
    [Route("api/personelgiris")]
    [ApiController]
    public class PersonelGirisKontrolcusu : ControllerBase
    {
        private readonly IPersonelGiris _authServisi;
        private readonly AppDbContext _context;
        private readonly ILogger<PersonelGirisKontrolcusu> _logger;

        public PersonelGirisKontrolcusu(
            IPersonelGiris authServisi,
            AppDbContext context,
            ILogger<PersonelGirisKontrolcusu> logger)
        {
            _authServisi = authServisi;
            _context = context;
            _logger = logger;
        }

        [HttpPost("giris")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(423)]
        public IActionResult Giris([FromBody] PersonelGirisBilgisi bilgi)
        {
            if (bilgi == null)
                return BadRequest("Body boş geldi.");

            _logger.LogInformation("Login denemesi: {SicilNo}", bilgi.SicilNo);

            if (string.IsNullOrEmpty(bilgi.SicilNo))
                return BadRequest("SicilNo boş.");

            if (string.IsNullOrEmpty(bilgi.Sifre))
                return BadRequest("Sifre boş.");

            var girisBilgisi = _context.PersonelGirisBilgileris
                .FirstOrDefault(g => g.SicilNo == bilgi.SicilNo);

            if (girisBilgisi == null)
                return BadRequest($"DB'de kayıt bulunamadı. Aranan: '{bilgi.SicilNo}'");

            if (girisBilgisi.HesapKilitliMi)
                return StatusCode(423, "Hesap kilitli.");

            _logger.LogInformation(
                "DB şifre: {DbSifre}, Gelen şifre: {GelenSifre}",
                girisBilgisi.Sifre,
                bilgi.Sifre
            );

            bool basarili = _authServisi.GirisYap(bilgi);

            if (basarili)
            {

                return Ok(new
                {
                    Token = "gecici_token_123",
                    Mesaj = "Giriş başarılı"
                });
            }
            else
            {
                return BadRequest("Hatalı sicil no veya şifre.");
            }
        }
    }
}