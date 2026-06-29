namespace Guardia.API.Models
{
    public class PersonelGecis
    {
        public long Id { get; set; }
        public long PersonelId { get; set; }
        public string? SicilNo { get; set; }
        public DateTimeOffset GecisZamani { get; set; }
        public string? GecisYonu { get; set; }
        public virtual Personeller? Personel { get; set; }
    }
}