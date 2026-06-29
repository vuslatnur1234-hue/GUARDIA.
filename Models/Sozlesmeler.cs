namespace Guardia.API.Models
{
    public class Sozlesmeler
    {
        public long Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Taraf { get; set; }
        public string Tur { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Durum { get; set; }
        public string DosyaUrl { get; set; }
        public long OlusturanId { get; set; }
    }
}
