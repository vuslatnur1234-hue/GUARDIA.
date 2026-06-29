using Microsoft.AspNetCore.Mvc;
using Guardia.API.DTOs;
using Guardia.API.Services.Personel;
using System; // Hata yakalama (Exception) için eklendi

namespace Guardia.API.Controllers
{
    [Route("api/personel")]
    [ApiController]
    public class PersonelController : ControllerBase
    {
        private readonly IPersonelServisi _personelServisi;

        public PersonelController(IPersonelServisi personelServisi)
        {
            _personelServisi = personelServisi;
        }

        [HttpGet("profil")]
        public IActionResult ProfilGetir([FromQuery] string sicilNo)
        {
            try
            {
                if (string.IsNullOrEmpty(sicilNo)) return BadRequest("Sicil no gönderilmedi.");

                var data = _personelServisi.ProfilBilgileriniGetir(sicilNo);

                if (data == null) return NotFound("Kullanıcı bulunamadı.");

                return Ok(data);
            }
            catch (Exception ex)
            {
           
                string hataDetayi = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, "C# ÇÖKTÜ DETAY: " + hataDetayi);
            }
        }
    }
}