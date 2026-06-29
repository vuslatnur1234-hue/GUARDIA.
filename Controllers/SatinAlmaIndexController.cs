using Microsoft.AspNetCore.Mvc;
using Guardia.API.DTOs.SA;
using Guardia.API.Services.SatinAlma;

namespace Guardia.API.Controllers
{
    [Route("api/saindex")]
    [ApiController]
    public class SatinAlmaIndexController : ControllerBase
    {
        private readonly IIndexSatinAlmaServisi _satinAlmaServisi;

        public SatinAlmaIndexController(IIndexSatinAlmaServisi satinAlmaServisi)
        {
            _satinAlmaServisi = satinAlmaServisi;
        }

        [HttpGet("veriler")]
        public IActionResult GetVeriler()
        {
            var data = _satinAlmaServisi.IndexVerileriniGetir();
            return Ok(data);
        }
        [HttpGet("rapor-indir")]
        public IActionResult RaporIndir()
        {
           
            byte[] dosyaIcerigi = System.Text.Encoding.UTF8.GetBytes("Guardia Harcama Raporu Test İçeriği...");

            return File(dosyaIcerigi, "text/plain", "Aylik_Harcama_Raporu.txt");
        }

        [HttpPost("mesaj-gonder")]
        public IActionResult MesajGonder([FromBody] SaIndexNotModeli istek)
        {
            if (istek == null || string.IsNullOrWhiteSpace(istek.Mesaj))
            {
                return BadRequest("Mesaj boş olamaz!");
            }

          
            Console.WriteLine("=== YENİ MESAJ GELDİ ===");
            Console.WriteLine("Hedefler: " + string.Join(", ", istek.HedefBirimler));
            Console.WriteLine("Mesaj: " + istek.Mesaj);
            Console.WriteLine("========================");

            return Ok(new { basari = true, bilgi = "Mesaj C# tarafına ulaştı!" });
        }
    }
}