namespace Guardia.API.Models
{
    public class Mevzuatlar
    {
        public long Id { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
        public string Baslik { get; set; }
        public string Kategori { get; set; }
        public string Ozet { get; set; }
        public string OnemDerecesi { get; set; } 
        public string DosyaUrl { get; set; }
        public long OlusturanId { get; set; }
        public DateTime YayinTarihi { get; set; }
    }
}
