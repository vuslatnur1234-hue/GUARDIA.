namespace Guardia.API.DTOs.HK
{
    public class HkDashboardModeli
    {

        public double GenelKazanmaOrani { get; set; }

        public List<RiskAnalizDto> RiskDagilimi { get; set; }
    }

 // Risk Analizi için ayrı bir DTO
    public class RiskAnalizDto
    {
        public string KategoriAd { get; set; }
        public double Yuzde { get; set; }
        public string DurumEtiketi { get; set; }
        public string RenkKodu { get; set; }
    }
}
