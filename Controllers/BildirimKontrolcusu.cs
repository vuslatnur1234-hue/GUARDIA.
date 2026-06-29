/*using Guardia.API.Services.Personel;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{
    [Route("api/pbildirim")]
    [ApiController]
    public class BildirimKontrolcusu : ControllerBase
    {
        private readonly IPBildirimServisi _bildirimServisi;

        public BildirimKontrolcusu(IPBildirimServisi bildirimServisi)
        {
            _bildirimServisi = bildirimServisi;
        }

        // GET: api/pbildirim/liste?sicilNo=SIC001
        [HttpGet("liste")]
        public async Task<IActionResult> ListeGetir([FromQuery] string sicilNo)
        {
            if (string.IsNullOrEmpty(sicilNo))
                return BadRequest("Sicil numarası gereklidir.");

            var liste = await _bildirimServisi.PersonelBildirimleriniGetirAsync(sicilNo);
            return Ok(liste);
        }
    }
}*/