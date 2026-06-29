namespace Guardia.API.DTOs.II
{
    public class IiAracHareketiModeli
    {
        public int Id { get; set; }

       
        public string Plaka { get; set; } = string.Empty;

      
        public string SurucuAdi { get; set; } = string.Empty;

     
        public string Saat { get; set; } = string.Empty;

     
        public string Durum { get; set; } = string.Empty;

      
        public string DurumCss
        {
            get => Durum switch
            {
                "Yolda" => "durum-yolda",
                "Dönüşte" => "durum-donuste",
                "Beklemede" => "durum-beklemede",
                _ => ""
            };
        }
    }
}
