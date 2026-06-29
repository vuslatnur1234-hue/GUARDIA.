using System;

namespace Guardia.API.Models
{
    public class Davalar
    {
        public long Id { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string DosyaNo { get; set; }
    
        public string? Mahkeme { get; set; }      
        public string? YoneticiNotu { get; set; }  
        public string? KarsiTaraf { get; set; }    
        public string? Konu { get; set; }
        public string? Asama { get; set; }
        public string? Durum { get; set; }
        public string? DosyaUrl { get; set; }    
        public long? OlusturanId { get; set; }
        public DateTimeOffset? DurusmaTarihi { get; set; }
    }
}
