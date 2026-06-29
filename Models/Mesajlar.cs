namespace Guardia.API.Models
{
    public class Mesajlar
    {
        public long Id { get; set; }
        public string GonderenBirim { get; set; }
        public string AliciBirim { get; set; }
        public string MesajIcerigi { get; set; }
        public string GonderimSaati { get; set; }
        public bool OkunduMu { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

    }
}

