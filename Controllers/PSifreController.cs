using Guardia.API.DTOs.PeGiris;
using Guardia.API.Services;
using Guardia.API.Services.Personel;
using Microsoft.AspNetCore.Mvc;

namespace Guardia.API.Controllers
{
    [Route("api/sifre")]
    [ApiController]
    public class SifreController : ControllerBase
    {
        
        private readonly IPSifreUnuttumServisi _sifreUnuttumServisi;

        public SifreController(IPSifreUnuttumServisi sifreUnuttumServisi)
        {
            _sifreUnuttumServisi = sifreUnuttumServisi;
        }

        [HttpPost("guncelle")]
        public IActionResult Guncelle([FromBody] PSifreGuncellemeModeli model)
        {
            var sonuc = _sifreUnuttumServisi.SifreGuncelle(model);

            if (sonuc.Basarili)
                return Ok(new { mesaj = sonuc.Mesaj });
            else
                return BadRequest(new { mesaj = sonuc.Mesaj });
        }
    }
}