using Microsoft.AspNetCore.Mvc;
using Guardia.API.DTOs.PE;
using Guardia.API.Services.Personel;

namespace Guardia.API.Controllers
{
    [Route("api/pizin")]
    [ApiController]
    public class PIzinKontrolcusu : ControllerBase
    {
        private readonly IPIzinServisi _izinServisi;

        public PIzinKontrolcusu(IPIzinServisi izinServisi)
        {
            _izinServisi = izinServisi;
        }

        [HttpPost("gonder")]
        public IActionResult Gonder([FromBody] PIzinTalepModeli model)
        {
            bool sonuc = _izinServisi.IzinTalepGonder(model);
            if (sonuc)
                return Ok(new { mesaj = "İzin talebiniz iletildi." });
            else
                return BadRequest(new { mesaj = "Hata oluştu." });
        }
    }
}