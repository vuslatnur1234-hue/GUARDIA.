namespace Guardia.API.Models
{
    public class Arsiv
    {
        public long Id { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
        public string DosyaAdi { get; set; } 
        public string Kategori { get; set; }
        public DateTime? YuklemeTarihi { get; set; } 
        public DateTime? ImhaTarihi { get; set; } 
        public string Durum { get; set; }
        public string DosyaUrl { get; set; }
        public long OlusturanId { get; set; } 
    }
}
