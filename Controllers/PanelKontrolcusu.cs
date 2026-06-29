using Microsoft.AspNetCore.Mvc;
using Guardia.API.Services.Personel;

namespace Guardia.API.Controllers
{
    [Route("api/panel")]
    [ApiController]
    public class PanelKontrolcusu : ControllerBase
    {
        private readonly IPersonelMerkeziServis _servis;

        public PanelKontrolcusu(IPersonelMerkeziServis servis)
        {
            _servis = servis;
        }

        [HttpGet("ozet")]
        public IActionResult OzetGetir([FromQuery] string sicilNo)
        {
            if (string.IsNullOrEmpty(sicilNo)) return BadRequest(new { mesaj = "Sicil No gerekli." });

            var data = _servis.PanelBilgisiniGetir(sicilNo);
            if (data == null) return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

            return Ok(data);
        }
    }
}