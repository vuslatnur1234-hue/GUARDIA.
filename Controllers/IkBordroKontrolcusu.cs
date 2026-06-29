using Guardia.API.DTOs;
using Guardia.API.Services.InsanKaynaklari;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{
    [Authorize(Roles = "Admin, Personel")]// Sadece Admin rolündeki kullanıcılar erişebilir
    [Route("api/[controller]")]
    [ApiController]
    public class IkBordroKontrolcusu : ControllerBase
    {
        private readonly IIkService _ikServisi;

       
        public IkBordroKontrolcusu(IIkService ikServisi)
        {
            _ikServisi = ikServisi;
        }

        //1. Bordro özeti için GET endpoint'i
        [HttpGet("ozet")]
        public async Task<IActionResult> GetOzet()
        {
            
            var ozet = await _ikServisi.GetBordroOzetAsync();
            return Ok(ozet);
        }

        //2. Bordro listesi için GET endpoint'i
        [AllowAnonymous]
        [HttpGet("liste")]
        public async Task<IActionResult> GetListe()
        {
            
            var liste = await _ikServisi.GetBordroListesiAsync();
            return Ok(liste);
        }

        //3. Maaş onaylama için POST endpoint'i
        [HttpPost("onayla/{id}")]
        public async Task<IActionResult> MaasOnayla(int id)
        {
            
            var sonuc = await _ikServisi.BordroDurumGuncelleAsync(id, "Onaylandı");

            if (!sonuc) return NotFound(new { mesaj = "Personel bulunamadı." });

            return Ok(new { mesaj = "Maaş başarıyla onaylandı." });
        }

        //4. Toplu ödeme emri gönderme için POST endpoint'i
        [HttpPost("odeme-emri")]
        public async Task<IActionResult> TopluOdemeEmriGonder()
        {
           
            var sonuc = await _ikServisi.TopluOdemeEmriGonderAsync();

            if (!sonuc)
            {
               
                return BadRequest("Ödeme yapılacak onaylı personel bulunamadı.");
            }

            return Ok(new { mesaj = "Banka talimatı başarıyla iletildi." });
        }

        //5. Tüm kayıtları onaylama için POST endpoint'i
        [HttpPost("hepsini-onayla")]
        public async Task<IActionResult> HepsiniOnayla()
        {
            var sonuc = await _ikServisi.HepsiniOnaylaAsync();
            if (!sonuc) return BadRequest("Onaylanacak kayıt bulunamadı.");
            return Ok(new { mesaj = "Tüm kayıtlar onaylandı." });
        }

       
    }
}