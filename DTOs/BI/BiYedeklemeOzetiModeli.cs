namespace Guardia.API.DTOs.BI
{
    public class BiYedeklemeOzetiModeli
    {
        public string YerelSunucuDurum { get; set; } // "AKTİF", "KRİTİK", "DEVRE DIŞI"
        public int BulutDolulukOrani { get; set; } // 88
        public string HariciYedekDurum { get; set; } // "SENKRONİZE", "BEKLİYOR"
    }
}
