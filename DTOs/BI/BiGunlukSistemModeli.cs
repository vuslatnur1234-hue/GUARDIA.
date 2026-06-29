using System.Text.Json.Serialization;

namespace Guardia.API.DTOs.BI
{
    public class BiGunlukSistemModeli
    {

        [JsonPropertyName("gunAdi")] 
        public string GunAdi { get; set; }

        [JsonPropertyName("aktiflikOrani")]
        public int AktiflikOrani { get; set; }

        [JsonPropertyName("detay")]
        public string Detay { get; set; }


    }
}
