namespace Guardia.API.DTOs.BI
{
    public class BiTeknikImkanModeli
    {
        public string PersonelAdSoyad { get; set; }
        public string Departman { get; set; }
        public string CihazVeyaYetki { get; set; } // Örn: MacBook Pro, VPN Erişimi, Adobe Lisansı
        public string BaglantiDurumu { get; set; } // Örn: Aktif, Pasif, Beklemede
        public string SonErisimTarihi { get; set; }
    }
}
