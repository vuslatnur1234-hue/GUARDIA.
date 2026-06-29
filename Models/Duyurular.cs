namespace Guardia.API.Models;

public class Duyurular
{
    public int Id { get; set; }
    public string? Baslik { get; set; }
    public string? Icerik { get; set; }
    public int? YayinlayanId { get; set; }
    public string? Kategori { get; set; }
    public DateTimeOffset? Tarih { get; set; }
    public string? HedefKitle { get; set; }
    public string? DosyaUrl { get; set; }
    public bool? BildirimGonder { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
}