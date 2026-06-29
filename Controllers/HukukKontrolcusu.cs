using Guardia.API.DTOs.HK;
using Guardia.API.Models;
using Guardia.API.Services;
using Guardia.API.Services.Hukuk;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HukukKontrolcusu : ControllerBase
    {
        private readonly IHkService _hukukServisi;

        public HukukKontrolcusu(IHkService hukukServisi)
        {
            _hukukServisi = hukukServisi;
        }

        [HttpGet("analiz")]
        public async Task<IActionResult> GetAnaliz(int yil, int ay)
        {
            var sonuc = await _hukukServisi.GetAylikAnalizAsync(yil, ay);
            return Ok(sonuc);
        }

        // Yıllık Trend Verilerini Getirir
        [HttpGet("trend")]
        public async Task<IActionResult> GetTrend(int yil)
        {
           
            var trendSonuc = await _hukukServisi.GetYillikTrendAsync(yil);

            if (trendSonuc == null || !trendSonuc.Any())
            {
                return NotFound("Seçilen yıla ait trend verisi bulunamadı.");
            }

            return Ok(trendSonuc);
        }


        // 1. Notları Listele: GET 
        [HttpGet("hatirlaticilar")]
        public async Task<IActionResult> GetHatirlaticilar()
        {
            try
            {
                
                var liste = await _hukukServisi.GetHatirlaticilarAsync();
                return Ok(liste);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Hatırlatıcılar yüklenirken bir hata oluştu: {ex.Message}");
            }
        }

        // 2. Yeni Not Ekle: POST 
        [HttpPost("hatirlatici-ekle")]
        public async Task<IActionResult> HatirlaticiEkle([FromBody] HkHatirlaticilar model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Icerik))
            {
                return BadRequest("Not içeriği boş olamaz.");
            }

            try
            {
                var yeniNot = await _hukukServisi.HatirlaticiEkleAsync(model);
                return Ok(yeniNot);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Not kaydedilirken bir hata oluştu: {ex.Message}");
            }
        }

        // 3. Notu Sil: DELETE 
        [HttpDelete("hatirlatici-sil/{id}")]
        public async Task<IActionResult> HatirlaticiSil(int id)
        {
            try
            {
                var basariliMi = await _hukukServisi.HatirlaticiSilAsync(id);
                if (!basariliMi)
                {
                    return NotFound("Silinmek istenen not veritabanında bulunamadı.");
                }

                return Ok(new { mesaj = "Not başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Not silinirken bir hata oluştu: {ex.Message}");
            }
        }

        //4. Notu Güncelle: PUT
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var data = await _hukukServisi.GetDashboardAsync();
            return Ok(data);
        }

        [HttpPost("ilerlet/{id}")]
        public async Task<IActionResult> Ilerlet(int id)
        {
            var sonuc = await _hukukServisi.AsamaIlerletAsync(id);
            return Ok(sonuc);
        }


        //5. Yeni Sözleşme Ekle: POST
        [HttpPost("ekle")]
        public async Task<IActionResult> Ekle([FromBody] HkSozlesmeVeriModeli model)
        {
            var sonuc = await _hukukServisi.SozlesmeEkleAsync(model);
            return Ok(sonuc);
        }

        //6. Dava Dashboard Verilerini Getir: GET
        [HttpGet("dava-dashboard")]
        public async Task<IActionResult> GetDavaDashboard()
        {
            var veri = await _hukukServisi.GetDavaDashboardAsync();
            return Ok(veri);
        }

        //7. Yeni Dava Ekle: POST
        [HttpPost("dava-ekle")]
        public async Task<IActionResult> DavaEkle([FromBody] HkDavaModeli model)
        {
            var sonuc = await _hukukServisi.DavaEkleAsync(model);
            return Ok(sonuc);
        }


        //8. Dava Notunu Güncelle: PUT
        [HttpPut("dava-not-guncelle/{id}")]
        public async Task<IActionResult> DavaNotGuncelle(int id, [FromBody] string yeniNot)
        {
            
            string temizNot = yeniNot?.Trim('"');

            var sonuc = await _hukukServisi.DavaNotGuncelleAsync(id, temizNot);
            if (!sonuc) return NotFound(new { mesaj = "Dava bulunamadı veya not güncellenemedi." });

            return Ok(new { mesaj = "Yönetici notu veritabanına mühürlendi." });
        }

        //9. Dava Aşamasını İlerlet: PUT
        [HttpPut("dava-asama-ilerlet/{id}")]
        public async Task<IActionResult> DavaAsamaIlerlet(int id)
        {
            var sonuc = await _hukukServisi.DavaAsamaIlerletAsync(id);
            if (!sonuc) return BadRequest(new { mesaj = "Aşama ilerletilemedi. Son aşamada olabilirsiniz." });

            return Ok(new { mesaj = "Süreç aşaması başarıyla bir adım ileri taşındı." });
        }

        //10. Mevzuat Dashboard Verilerini Getir: GET
        [HttpGet("mevzuat-dashboard")]
        public async Task<IActionResult> GetMevzuatDashboard()
        {
            var veri = await _hukukServisi.GetMevzuatDashboardAsync();
            return Ok(veri);
        }


        //11. Yeni Mevzuat Ekle: POST
        [HttpPost("mevzuat-ekle")]
        public async Task<IActionResult> MevzuatEkle([FromBody] HkMevzuatModeli model)
        {
            var sonuc = await _hukukServisi.MevzuatEkleAsync(model);
            return Ok(sonuc);
        }


        //12. Arşiv Listesini Getir: GET
        [HttpGet("arsiv-liste")]
        public async Task<IActionResult> GetArsiv()
        {
            var liste = await _hukukServisi.GetArsivListesiAsync();
            if (liste == null) return Ok(new List<HkDijitalArsivModeli>()); // Boş liste dön, boş yanıt dönme.
            return Ok(liste);
        }


        //13. Dosya İmha Et: DELETE
        [HttpDelete("imha/{id}")]
        public async Task<IActionResult> ImhaEt(int id)
        {
            var sonuc = await _hukukServisi.DosyaImhaEtAsync(id);
            if (!sonuc) return BadRequest("Dosya bulunamadı veya imha edilemedi.");
            return Ok(new { mesaj = "Dosya kalıcı olarak imha edildi." });
        }

        //14. Dosya İndir: GET
        [HttpGet("indir/{id}")]
        public async Task<IActionResult> DosyaIndir(int id)
        {
            var dosya = (await _hukukServisi.GetArsivListesiAsync()).FirstOrDefault(x => x.Id == id);
            if (dosya == null) return NotFound();

           
            var dosyaYolu = Path.Combine("wwwroot", dosya.DosyaUrl ?? "");
            if (!System.IO.File.Exists(dosyaYolu))
                return NotFound("Dijital kopya henüz mevcut değil.");

            var bytes = System.IO.File.ReadAllBytes(dosyaYolu);
            return File(bytes, "application/pdf", $"{dosya.EsasNo}.pdf");
        }

        //15. Dosya Tarayıp Arşivle: PUT
        [HttpPut("arsiv-guncelle/{id}")]
        public async Task<IActionResult> ArsivGuncelle(int id, IFormFile file)
        {
            
            if (file == null || file.Length == 0)
                return BadRequest(new { mesaj = "Lütfen geçerli bir PDF dosyası seçiniz." });

            var uzanti = Path.GetExtension(file.FileName).ToLower();
            if (uzanti != ".pdf")
                return BadRequest(new { mesaj = "Sadece PDF formatındaki dosyalar yüklenebilir." });

          
            var arsivListesi = await _hukukServisi.GetArsivListesiAsync();
            var dosya = arsivListesi.FirstOrDefault(x => x.Id == id);
            if (dosya == null)
                return NotFound(new { mesaj = "İlgili arşiv kaydı bulunamadı." });

            var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", "arsiv");
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }

           
            var benzersizDosyaAdi = $"{id}_{Guid.NewGuid()}{uzanti}";
            var tamDosyaYolu = Path.Combine(klasorYolu, benzersizDosyaAdi);

           
            using (var stream = new FileStream(tamDosyaYolu, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string yeniDosyaUrl = $"docs/arsiv/{benzersizDosyaAdi}";

            var sonuc = await _hukukServisi.ArsivGuncelleAsync(id, "Arşivlendi", yeniDosyaUrl);
            if (!sonuc)
                return NotFound(new { mesaj = "Durum güncellenirken bir hata oluştu." });

            return Ok(new { mesaj = "Dosya başarıyla tarandı ve arşivlendi.", url = yeniDosyaUrl });
        }

        //16. Dava Süresini Uzat: PUT
        [HttpPut("sure-uzat/{id}")]
        public async Task<IActionResult> SureUzat(int id, [FromBody] string yeniSure)
        {
            var sonuc = await _hukukServisi.SureUzatAsync(id, yeniSure);
            if (!sonuc) return NotFound();
            return Ok(new { mesaj = "Süre uzatıldı." });
        }


        //17. Yeni Arşiv Kaydı Ekle: POST
        [HttpPost("arsiv-ekle")]
        public async Task<IActionResult> ArsivEkle([FromBody] HkDijitalArsivModeli model)
        {
            if (model == null) return BadRequest();
            var kaydedilen = await _hukukServisi.ArsivEkleAsync(model);
            return Ok(kaydedilen);
        }
    }
}
