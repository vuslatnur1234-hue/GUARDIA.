namespace Guardia.API.DTOs.PE
{
    public class PBildirimModeli
    {
        public int Id { get; set; }
        public string? Baslik { get; set; }
        public string? Mesaj { get; set; }
        public string? Icon { get; set; }
        public bool OkunduMu { get; set; }
        public string? Tarih { get; set; }
    }
}