namespace Guardia.API.DTOs.PeGiris
{
    public class PSicilDogrulamaModeli
    {
        public required string SicilNo { get; set; }
    }

    public class SmsDogrulamaModeli
    {
        public required string Kod { get; set; }
    }

    public class PSifreSifirlamaModeli
    {
        public required string SicilNo { get; set; }
        public required string YeniSifre { get; set; }
        public required string YeniSifreTekrar { get; set; }
    }

    public class PSifreGuncellemeModeli
    {
        public required string SicilNo { get; set; }
        public required string MevcutSifre { get; set; }
        public required string YeniSifre { get; set; }
        public required string YeniSifreTekrar { get; set; }
    }
}