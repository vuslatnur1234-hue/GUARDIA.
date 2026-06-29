using Microsoft.AspNetCore.Mvc;
using Guardia.API.DTOs;
using Guardia.API.Services.ai;

namespace Guardia.API.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class AiKontrolcusu : ControllerBase
    {
        private readonly IAiServisi _aiServisi;

        public AiKontrolcusu(IAiServisi aiServisi)
        {
            _aiServisi = aiServisi;
        }

        [HttpPost("soru-sor")]
        public async Task<IActionResult> SoruCevapla([FromBody] SoruModeli istek)
        {
            var cevap = await _aiServisi.SoruCevapla(istek);
            return Ok(cevap);
        }
    }
}