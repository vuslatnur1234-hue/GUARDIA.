using Guardia.API.Services.Personel;
using Microsoft.AspNetCore.Mvc;


namespace Guardia.API.Controllers
{
    [Route("api/pyemek")]
    [ApiController]
    public class PYemekKontrolcusu : ControllerBase
    {
        private readonly IPYemekServisi _yemekServisi;

        public PYemekKontrolcusu(IPYemekServisi yemekServisi)
        {
            _yemekServisi = yemekServisi;
        }

        [HttpGet("menu")]
        public IActionResult MenuGetir()
        {
            var menu = _yemekServisi.HaftaMenusuGetir();
            return Ok(menu);
        }
    }
}