using Guardia.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;
    public TestController(AppDbContext context) { _context = context; }

    [HttpGet("test-db")]
    public async Task<IActionResult> TestDb()
    {
        var count = await _context.Personellers.CountAsync();
        return Ok($"Bağlantı başarılı! Personel sayısı: {count}");
    }
}