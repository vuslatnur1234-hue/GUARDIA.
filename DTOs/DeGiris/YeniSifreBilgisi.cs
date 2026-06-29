namespace Guardia.API.DTOs.DeGiris
{
    public class YeniSifreBilgisi
    {
        public string SicilNo { get; set; } 
        public string Email { get; set; } = null!;
        public string YeniSifre { get; set; } = null;
        public string YeniSifreTekrar { get; set; } = null;
    }
}
