namespace Guardia.API.DTOs.IK
{
    public class IkMemnuniyetAnalizModeli
    {
        public int Hijyen { get; set; }
        public int Yemekhane { get; set; }
        public int Iletisim { get; set; } 
        public int SosyalHaklar { get; set; }
        public int Katilim { get; set; }
        public int YanitSayisi { get; set; }

        public double AylikTrend { get; set; }
    }
}
