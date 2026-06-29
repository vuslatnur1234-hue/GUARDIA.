

namespace Guardia.API.Models
{
    public class Admin
    {
        public long id { get; set; }
        public DateTimeOffset? created_at { get; set; }

        public string admin_no { get; set; } = string.Empty;
        public string sifre { get; set; } = string.Empty;
        public string ad_soyad { get; set; } = string.Empty;
        public string departman { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public int? yetki_seviyesi { get; set; }
       

        // Şifremi unuttum akışı için
        public string? dogrulama_kodu { get; set; }
        public DateTimeOffset? kod_son_kullanma { get; set; }
    }
}