using Microsoft.AspNetCore.Mvc;
using Guardia.API.DTOs;
using Guardia.API.Services.Interfaces;
using Guardia.API.Data; // AppDbContext için
using Guardia.API.Models; // Mesajlar modeli için
using System;
using System.Threading.Tasks;

namespace Guardia.API.Controllers
{
    [Route("api/ariza")]
    [ApiController]
    public class ArizaController : ControllerBase
    {
        private readonly IArizaServisi _arizaServisi;
        private readonly AppDbContext _context; 

        public ArizaController(IArizaServisi arizaServisi, AppDbContext context)
        {
            _arizaServisi = arizaServisi;
            _context = context;
        }

        [HttpPost("kaydet")]
        public async Task<IActionResult> Kaydet([FromBody] ArizaKayitModeli gelenVeri)
        {
           
            bool basarili = _arizaServisi.ArizaKaydet(gelenVeri);

            if (basarili)
            {
                try
                {
                   
                    var sistemBildirimi = new Mesajlar
                    {
                        GonderenBirim = "SİSTEM - ARIZA",

                   
                        AliciBirim = "İNSAN KAYNAKLARI",

                        
                        MesajIcerigi = $"Yeni Arıza Talebi!\nMakine: {gelenVeri.Makine}\nTip: {gelenVeri.Tip}\nAciliyet: {gelenVeri.Aciliyet}\nDetay: {gelenVeri.Detay}",

                        GonderimSaati = DateTime.Now.ToString("HH:mm"),
                        OkunduMu = false, 
                        CreatedAt = DateTime.Now
                    };

                    _context.Mesajlars.Add(sistemBildirimi);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("Arıza kaydedildi ancak İK bildirimi gönderilemedi: " + ex.Message);
                }

                return Ok(new { mesaj = "Arıza başarıyla sunucuya ulaştı, teşekkürler!" });
            }
            else
            {
                return BadRequest(new { mesaj = "Veri kaydedilirken bir sorun çıktı." });
            }
        }
    }
}