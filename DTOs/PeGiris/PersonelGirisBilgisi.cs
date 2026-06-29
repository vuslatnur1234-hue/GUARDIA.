using System.Text.Json.Serialization;

namespace Guardia.API.DTOs.PeGiris
{
    public class PersonelGirisBilgisi
    {
        [JsonPropertyName("sicilNo")]  
        public string? SicilNo { get; set; }

        [JsonPropertyName("sifre")]
        public string? Sifre { get; set; }
    }
}