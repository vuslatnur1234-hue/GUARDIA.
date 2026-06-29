using Guardia.API.DTOs.IK;
using Guardia.API.Services.InsanKaynaklari;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{

    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class IkPersonelKontrolcusu : ControllerBase{

        private readonly IIkService _ikServisi;


    
    public IkPersonelKontrolcusu(IIkService ikServisi)
    {
        _ikServisi = ikServisi;
    }
    
    // 1. Personel Listesi (GET)
   [HttpGet("personeller")]
public async Task<IActionResult> PersonelleriGetir() 
{
    
    var liste = await _ikServisi.PersonelListesiniGetirAsync(); 
    
    return Ok(liste);
}

        // 2. Personel Ekleme (POST)
        [HttpPost("ekle")]
        public IActionResult PersonelEkle([FromBody] IkPersonelEkleModeli model)
        {
            try
            {
                if (model == null) return BadRequest("Veri boş geldi.");

                
                _ikServisi.YeniPersonelKaydet(model);

                return Ok(new { mesaj = "Başarılı" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, ex.Message);
            }
        }

        // 3. Personel Silme (DELETE)
        [HttpDelete("sil/{sicilNo}")]
        public async Task<IActionResult> PersonelSil(string sicilNo)
        {
            try
            {
                
                bool silindiMi = await _ikServisi.PersonelSilAsync(sicilNo);

                if (silindiMi)
                {
                    
                    return Ok(new { mesaj = "Personel kaydı başarıyla pasif duruma getirildi ve işten çıkış tarihi mühürlendi." });
                }
                else
                {
                    return NotFound("Sistemde bu sicil numarasına sahip aktif bir personel bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"İşlem sırasında sunucu hatası oluştu: {ex.Message}");
            }
        }

        // 4. Personel Detayı (GET)
        [HttpGet("personel/{sicilNo}")]
        public async Task<IActionResult> PersonelDetayGetir(string sicilNo)
        {
            var pers = await _ikServisi.PersonelDetayiniGetirAsync(sicilNo); 
            if (pers == null) return NotFound(new { mesaj = "Personel bulunamadı." });
            return Ok(pers);
        }

        // 5. Personel Güncelleme (PUT)
        [HttpPut("guncelle/{sicilNo}")]
        public IActionResult PersonelGuncelle(string sicilNo, [FromBody] IkPersonelEkleModeli model)
        {
            var ok = _ikServisi.PersonelGuncelle(sicilNo, model);
            if (!ok) return NotFound("Personel bulunamadı.");

         
            _ikServisi.BekleyenTalebiSil(sicilNo);

            return Ok(new { mesaj = "Güncellendi" });
        }

        // 6. Onay Bekleyen Veriyi Getir (GET)
        [HttpGet("onay-bekleyen/{sicilNo}")]
        public IActionResult GetOnayBekleyen(string sicilNo)
        {
            var veri = _ikServisi.BekleyenVeriyiGetir(sicilNo);
            if (veri == null) return NotFound("Bekleyen talep yok.");
            return Ok(veri);
        }

        // 7. Güncelleme Talebi Gönder (POST)
        [AllowAnonymous]
        [HttpPost("talep-gonder/{sicilNo}")]
        public IActionResult TalepGonder(string sicilNo, [FromBody] Dictionary<string, string> yeniVeriler)
        {
            _ikServisi.GuncellemeTalebiOlustur(sicilNo, yeniVeriler);
            return Ok(new { mesaj = "Güncelleme talebiniz İK'ya iletildi." });
        }

        // 8. Tüm Bekleyen Talepleri Getir (Bildirim Paneli İçin)
        [HttpGet("tum-bekleyen-talepler")]
        public IActionResult TumBekleyenTalepleriGetir()
        {
            var talepler = _ikServisi.TumBekleyenVerileriGetir();
            return Ok(talepler);
        }
    }
}
