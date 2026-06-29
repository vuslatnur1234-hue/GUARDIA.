using Guardia.API.DTOs;
using Guardia.API.Services.InsanKaynaklari;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{

    [Authorize(Roles = "Admin")]
    [Route("api/IkIzinKontrolcusu")]
    [ApiController]
    public class IkIzinKontrolcusu : ControllerBase
    { 
        private readonly IIkService _ikServisi;

        public IkIzinKontrolcusu(IIkService ikServisi)
        {
            _ikServisi = ikServisi;
        }


       //İzin istatistiklerini getirir
        [HttpGet("izin-istatistikleri")]
        public async Task<IActionResult> IzinIstatistikleri()
        {
            var istatistikler = await _ikServisi.IzinIstatistikleriniGetirAsync();
            return Ok(istatistikler);
        }

        // Belirli bir izin talebini onaylar
        [HttpPost("izin-onayla/{id}")]
        public async Task<IActionResult> IzinOnayla(int id)
        {
            var sonuc = await _ikServisi.IzinOnaylaAsync(id);
            if (sonuc)
            {
                return Ok(new { mesaj = "İzin başarıyla onaylandı." });
            }
            return BadRequest(new { mesaj = "Onay işlemi başarısız oldu." });
        }

        // Belirli bir izin talebini reddeder
        [HttpPost("izin-reddet/{id}")]
        public async Task<IActionResult> IzinReddet(int id, [FromBody] string neden)
        {
            var sonuc = await _ikServisi.IzinReddetAsync(id, neden);
            if (sonuc)
            {
                return Ok(new { mesaj = "İzin talebi reddedildi." });
            }
            return BadRequest(new { mesaj = "Red işlemi başarısız oldu." });
        }


        // İzin taleplerinin listesini getirir
        [HttpGet("izin-listesi-getir")] 
        public async Task<IActionResult> IzinListesi()
        {
            var liste = await _ikServisi.IzinListesiniGetirAsync();
            return Ok(liste);
        }
    }
}
