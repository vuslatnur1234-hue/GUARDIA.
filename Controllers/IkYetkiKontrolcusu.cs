using Guardia.API.DTOs.DeGiris;
using Guardia.API.Services.AuthDepartman;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IkYetkiKontrolcusu : ControllerBase
    {
        private readonly IAuthService _authServisi;
        private readonly JWTService _jwtServisi;

        
        public IkYetkiKontrolcusu(IAuthService authServisi, JWTService jwtServisi)
        {
            _authServisi = authServisi;
            _jwtServisi = jwtServisi;
        }

        [AllowAnonymous]

        // Departmanlar için giriş endpoint'i
        [HttpPost("giris-yap")]
        public IActionResult GirisYap([FromBody] GirisBilgisi model)
        {
            var sonuc = _authServisi.GirisYap(model) as dynamic;
            if (sonuc == null)
                return Unauthorized(new { mesaj = "Sicil No veya Şifre Hatalı!" });

            return Ok(sonuc); 
        }


        [AllowAnonymous]
        // Şifre sıfırlama kodu gönderme endpoint'i
        [HttpPost("kod-gonder")]
        public IActionResult KodGonder([FromBody] SifreSifirlamaBilgisi bilgi)
        {
            var sonuc = _authServisi.KodGonder(bilgi);
            if (sonuc != null) return Ok(sonuc);
            return BadRequest(new { mesaj = "Geçersiz sicil no veya e-posta." });
        }


        [AllowAnonymous]
        // Gönderilen kodu doğrulama endpoint'i
        [HttpPost("kodu-onayla")]
        public IActionResult KoduDogrula([FromBody] KodDogrulamaBilgisi bilgi)
        {
            var sonuc = _authServisi.KoduDogrula(bilgi);
            if (sonuc != null) return Ok(sonuc);
            return BadRequest(new { mesaj = "Geçersiz veya süresi dolmuş kod." });
        }

        [AllowAnonymous]
        // Yeni şifre belirleme endpoint'i
        [HttpPost("sifre-guncelle")]
        public IActionResult SifreyiGuncelle([FromBody] YeniSifreBilgisi bilgi)
        {
            var sonuc = _authServisi.SifreyiGuncelle(bilgi);
            if (sonuc != null) return Ok(sonuc);
            return BadRequest(new { mesaj = "Şifre güncellenemedi. Politika kurallarını kontrol edin." });
        }

      
        [Authorize(Roles = "Admin")]
        // Kullanıcı bilgilerini döndüren endpoint
        [HttpGet("ben-kimim")]
        public IActionResult BenKimim()
        {
            return Ok(new
            {
                adminNo = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
                departman = User.FindFirst("departman")?.Value,
                adSoyad = User.FindFirst("adSoyad")?.Value
            });
        }



    }
}