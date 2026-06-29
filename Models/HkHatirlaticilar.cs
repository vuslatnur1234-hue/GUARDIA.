namespace Guardia.API.Models
{
    public class HkHatirlaticilar
    {
        public int Id { get; set; } 
        public string Icerik { get; set; }
        public string TarihEtiketi { get; set; } 
        public bool IsTamamlandi { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
    }
}
