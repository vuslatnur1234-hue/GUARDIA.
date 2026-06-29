namespace Guardia.API.DTOs.HK
{
    public class HkDijitalArsivModeli
    {
        public int Id { get; set; }
        public string Kategori { get; set; }
        public string EsasNo { get; set; } = null!;
        public string DosyaAdi { get; set; } = null!;
        public string KapanisTarihi { get; set; } = null!;
        public string SaklamaSuresi { get; set; } = null!;
        public string Durum { get; set; } = "Arşivlendi";
        public string? DosyaUrl { get; set; }
        public string Tema { get; set; } = "green";
        public bool ImhaGerekliMi { get; set; } = false;
    }
}
