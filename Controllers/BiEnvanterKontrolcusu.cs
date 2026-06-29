using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Guardia.API.DTOs.BI;
using Guardia.API.Services.BilgiIslem;

namespace Guardia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiEnvanterKontrolcusu : ControllerBase
    {

        private readonly IBiService _teknikImkanServisi;

        public BiEnvanterKontrolcusu(IBiService teknikImkanServisi)
        {
            _teknikImkanServisi = teknikImkanServisi;
        }

        [HttpGet("baglantilar")]
        public async Task<IActionResult> GetBaglantilar(string? departman)
        {
            if (!string.IsNullOrEmpty(departman))
            {
                return Ok(await _teknikImkanServisi.DepartmanBazliImkanlarAsync(departman));
            }
            return Ok(await _teknikImkanServisi.TumImkanlariListeleAsync());
        }


        [HttpGet("ozet")]
        public async Task<IActionResult> GetEnvanterOzeti()
        {
            return Ok(await _teknikImkanServisi.GetEnvanterOzetiAsync());
        }


        [HttpGet("son24saat-zimmet-sayisi")]
        public async Task<IActionResult> GetSon24SaatZimmetSayisi()
        {
            return Ok(await _teknikImkanServisi.GetSon24SaatZimmetSayisiAsync());
        }


        [HttpGet("talep-istatistikleri")]
        public async Task<IActionResult> GetTalepIstatistikleri()
        {
            return Ok(await _teknikImkanServisi.GetAylikTalepIstatisikleriAsync());
        }

        [HttpGet("sistem-calisma-suresi")]
        public async Task<IActionResult> GetSistemCalismaSuresi()
        {
            return Ok(await _teknikImkanServisi.GetHaftalikSistemCalismaSuresiAsync());
        }

        [HttpGet("yedekleme-ozeti")]
        public async Task<IActionResult> GetYedeklemeOzeti()
        {
            return Ok(await _teknikImkanServisi.GetYedeklemeOzetiAsync());
        }



        // 1. Tüm Envanteri Listele
  [HttpGet("liste")]
  public async Task<IActionResult> GetEnvanter()
        {
            var liste = await _teknikImkanServisi.GetTumEnvanterAsync(); // ID göndermene gerek yok
            return Ok(liste);
        }

        // 2. Yeni Cihaz Ekle
        [HttpPost("ekle")]
        public async Task<IActionResult> CihazEkle([FromBody] BiEnvanterModeli model)
        {
            if (model == null) return BadRequest("Cihaz bilgileri boş olamaz.");

            var sonuc = await _teknikImkanServisi.EnvanterEkleAsync(model);
            return Ok(new { mesaj = "Cihaz başarıyla eklendi!", data = sonuc });
        }

        // 3. Cihaz Güncelle
        [HttpPut("guncelle/{id}")]
        public async Task<IActionResult> CihazGuncelle(int id, [FromBody] BiEnvanterModeli model)
        {
            
            var mevcutCihaz = await _teknikImkanServisi.GetByIdAsync(id);
            if (mevcutCihaz == null) return NotFound("Güncellenecek cihaz bulunamadı.");

            await _teknikImkanServisi.EnvanterGuncelleAsync(id, model);
            return Ok(new { mesaj = "Cihaz bilgileri güncellendi." });
        }

        // 4. Cihaz Sil
        [HttpDelete("sil/{id}")]
        public async Task<IActionResult> CihazSil(int id)
        {
           
            var kontrol = await _teknikImkanServisi.GetByIdAsync(id);
            if (kontrol == null) return NotFound("Silinecek cihaz bulunamadı.");

            await _teknikImkanServisi.EnvanterSilAsync(id);
            return Ok(new { mesaj = "Cihaz envanterden silindi." });
        }
    }
}
