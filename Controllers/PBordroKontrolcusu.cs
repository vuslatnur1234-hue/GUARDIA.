/*using Guardia.API.Services;
using Guardia.API.Services.Personel;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{
    [Route("api/pbordro")]
    [ApiController]
    public class PBordroKontrolcusu : ControllerBase
    {
        private readonly IPBordroServisi _bordroServisi;

        public PBordroKontrolcusu(IPBordroServisi bordroServisi)
        {
            _bordroServisi = bordroServisi;
        }

        [HttpGet("liste")]
        public IActionResult ListeGetir([FromQuery] string sicilNo)
        {
            var liste = _bordroServisi.BordrolariGetir(sicilNo);
            return Ok(liste);
        }

        [HttpGet("indir/{ay}")]
        public IActionResult Indir(string ay)
        {
            var icerik = System.Text.Encoding.UTF8.GetBytes($"{ay} bordrosu - Guardia");
            return File(icerik, "application/pdf", $"Bordro_{ay}.pdf");
        }
    }
}*/