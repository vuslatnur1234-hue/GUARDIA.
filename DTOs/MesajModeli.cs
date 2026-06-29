namespace Guardia.API.DTOs
{
    public class MesajModeli
    {
        public long Id { get; set; }
        public string Birim { get; set; }
        public string Mesaj { get; set; }
        public string Saat { get; set; }
        public bool OkunduMu { get; set; }
    }
}
