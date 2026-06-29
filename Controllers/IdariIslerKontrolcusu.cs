
using Guardia.API.DTOs.II;
using Guardia.API.Services.IdariIsler;
using Microsoft.AspNetCore.Mvc;


namespace Guardia.API.Controllers
{
   
    [Route("api/IdariIsler")]
    [ApiController]
    public class IdariIslerKontrolcusu : ControllerBase
    {
        private readonly IiiDashboardServisi _dashboardServisi;

        public IdariIslerKontrolcusu(IiiDashboardServisi dashboardServisi)
        {
            _dashboardServisi = dashboardServisi;
        }

        
        [HttpGet("Sayfa")]
        public IActionResult Index()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ii-index.html");
            return PhysicalFile(path, "text/html");
        }

       
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var veri = await _dashboardServisi.DashboardVerisiniGetirAsync();
            return Ok(veri);
        }

        
        [HttpGet("Stok/{ayIndex:int}")]
        public async Task<IActionResult> StokDagilimi(int ayIndex)
        {
            if (ayIndex < 0 || ayIndex > 11)
                return BadRequest(new { mesaj = "Geçersiz ay indeksi." });

            var veri = await _dashboardServisi.StokDagiliminiGetirAsync(ayIndex);
            return Ok(veri);
        }

        
        [HttpGet("Araclar")]
        public async Task<IActionResult> AracHareketleri([FromQuery] string? q)
        {
            var liste = await _dashboardServisi.AracHareketleriniGetirAsync(q);
            return Ok(liste);
        }

       
        [HttpGet("Notlar")]
        public async Task<IActionResult> NotlariGetir()
        {
            
            int kullaniciId = 1;
            var notlar = await _dashboardServisi.NotlariGetirAsync(kullaniciId);
            return Ok(notlar);
        }

    
        [HttpPost("Notlar")]
        public async Task<IActionResult> NotEkle([FromBody] IiOperasyonNotuModeli not)
        {
            if (string.IsNullOrWhiteSpace(not.Metin))
                return BadRequest(new { mesaj = "Not metni boş olamaz." });

            
            not.KullaniciId = 1; 
            not.OlusturmaTarihi = DateTime.Now;

            int yeniId = await _dashboardServisi.NotEkleAsync(not);
            return Ok(new { id = yeniId, mesaj = "Not eklendi." });
        }

       
        [HttpDelete("Notlar")]
        public async Task<IActionResult> NotlariSil([FromBody] List<int> notIdleri)
        {
            if (notIdleri == null || notIdleri.Count == 0)
                return BadRequest(new { mesaj = "Silinecek not seçilmedi." });

            int silinenSayi = await _dashboardServisi.NotlariSilAsync(notIdleri);
            return Ok(new { silinenSayi, mesaj = $"{silinenSayi} not silindi." });
        }
    }
}