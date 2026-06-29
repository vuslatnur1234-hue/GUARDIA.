using Guardia.API.Data;
using Guardia.API.DTOs.IK;
using Guardia.API.Models;
using Guardia.API.Services.InsanKaynaklari;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;

namespace Guardia.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class IkIstatistikKontrolcusu : ControllerBase
    {
        private readonly IIkService _ikServisi;
        private readonly AppDbContext _context;
        public IkIstatistikKontrolcusu(IIkService ikServisi, AppDbContext context)
        {
            _ikServisi = ikServisi;
            _context = context;
        }

        // 1. Panel İstatistikleri Endpoint'i
        [HttpGet("panel")]
        public async Task<IActionResult> PanelIstatistikleri()
        {
            return Ok(await _ikServisi.PanelIstatistikleriniGetirAsync());
        }

      //2. Giriş Listesi Endpoint'i
        [HttpGet("giris-listesi")]
        public async Task<IActionResult> GirisListesi()
        {
           
            var sonucListesi = await _ikServisi.GirisListesiniGetirAsync();

            return Ok(sonucListesi);
        }

        // 3. Aktiviteler Endpoint'i
        [HttpGet("aktiviteler")]
        public async Task<IActionResult> GetAktiviteler([FromQuery] string kategori = "tumu")
        {
            
            var result = await _ikServisi.AktiviteleriGetirAsync(kategori);
            return Ok(result);
        }

        //4. Departman Dağılımı Endpoint'i
        [HttpGet("departman-dagilim")]
        public async Task<IActionResult> DepartmanDagilim([FromQuery] string tip = "tumu")
        {
            return Ok(await _ikServisi.DepartmanDagiliminiGetirAsync(tip));
        }


     
        private string GetGirisYapanBirim()
        {
            var birimClaim = User.FindFirst("Birim")?.Value;
            if (!string.IsNullOrEmpty(birimClaim)) return birimClaim.ToUpper();

            var headerBirim = Request.Headers["X-Aktif-Birim"].ToString();
            if (!string.IsNullOrEmpty(headerBirim))
            {
               
                if (headerBirim.ToUpper() == "INSAN_KAYNAKLARI")
                    return "İNSAN KAYNAKLARI";

                return headerBirim.ToUpper();
            }

            return "HUKUK";
        }

        // 5. Gelen Mesajlar: GET 
        [HttpGet("mesajlar")]
        public async Task<IActionResult> MesajlariGetir()
        {
            string aktifBirim = GetGirisYapanBirim(); 
            var gelenler = await _ikServisi.GelenMesajlariGetirAsync(aktifBirim);
            return Ok(gelenler);
        }

        // 6. Giden Mesajlar: GET 
        [HttpGet("giden-mesajlar")]
        public async Task<IActionResult> GidenMesajlariGetir()
        {
            string aktifBirim = GetGirisYapanBirim();
            var gidenler = await _ikServisi.GidenMesajlariGetirAsync(aktifBirim);
            return Ok(gidenler);
        }

        // 7. Mesaj Gönder / Cevap Yaz
        [HttpPost("mesaj-gonder")]
        public async Task<IActionResult> MesajGonder([FromBody] Guardia.API.DTOs.MesajModeli model)
        {
            // Validasyon kontrolü
            if (model == null || string.IsNullOrWhiteSpace(model.Mesaj) || string.IsNullOrWhiteSpace(model.Birim))
            {
                return BadRequest("Mesaj içeriği ve hedef birim alanları boş bırakılamaz.");
            }

            string aktifBirim = GetGirisYapanBirim(); 

          
            var sonuc = await _ikServisi.MesajGonderAsync(aktifBirim, model);

            if (!sonuc)
                return StatusCode(500, "Mesaj gönderilirken veritabanı katmanında bir hata oluştu.");

            return Ok(new { mesaj = "Mesajınız ilgili departmanların veri tablolarına başarıyla mühürlendi." });
        }

     //8. Mesaj Okundu Olarak İşaretle: POST 
        [HttpPost("mesaj-okundu/{id}")]
        public async Task<IActionResult> MesajOkunduYap(long id) 
        {
        
            var mesaj = await _context.Mesajlars.FindAsync(id);

            if (mesaj != null && !mesaj.OkunduMu)
            {
                mesaj.OkunduMu = true; 
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Mesaj okundu olarak işaretlendi." });
            }

            return Ok(); 
        }

        // 9. Memnuniyet Detayları Endpoint'i
        [HttpGet("memnuniyet-detay")]
        public IActionResult MemnuniyetDetay() => Ok(_ikServisi.MemnuniyetAnaliziGetir());
        
        // 10. Turnover Detayları Endpoint'i
        [HttpGet("turnover-detay")]
        public async Task<IActionResult> TurnoverDetay() => Ok(await _ikServisi.TurnoverAnaliziGetirAsync());

        // 11. Saha Aktiflik Detayları Endpoint'i
        [HttpGet("saha-aktiflik")]
        public async Task<IActionResult> GetSahaAktiflik() => Ok(await _ikServisi.GetSahaAktiflikAsync());

        // 12. Duyuru Yayınlama Endpoint'i
        [AllowAnonymous]
        [HttpPost("duyuru-yayinla")]
        public async Task<IActionResult> DuyuruYayinla([FromBody] IkDuyuruModeli model)
        {
           
            var sonuc = await _ikServisi.DuyuruKaydet(model);

            if (sonuc) return Ok();
            return BadRequest();
        }


        // 12. Duyuru Listesi Endpoint'i
        [AllowAnonymous]
        [HttpGet("duyuru-listesi")]
        public async Task<IActionResult> DuyuruListesi() 
        {
            
            var liste = await _ikServisi.PersoneleGoreDuyuruGetir("Tüm Personel");
            return Ok(liste);
        }

        // 13. Personel Duyuruları Endpoint'i
        [AllowAnonymous]
        [HttpGet("personel-duyurulari")]
        public async Task<IActionResult> GetPersonelDuyurulari([FromQuery] string dept) 
        {
            
            var süzülmüşDuyurular = await _ikServisi.PersoneleGoreDuyuruGetir(dept);
            return Ok(süzülmüşDuyurular);
        }
    }
}