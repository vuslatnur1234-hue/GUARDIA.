namespace Guardia.API.DTOs.HK
{
    public class HkDavaModeli
    {
        public int Id { get; set; }
        public string DosyaNo { get; set; }      
        public string Mahkeme { get; set; }    
        public string KarsiTaraf { get; set; }   
        public string Konu { get; set; }         
        public string Asama { get; set; }        
        public string DurusmaTarihi { get; set; }
        public string Durum { get; set; }      
        public string YoneticiNotu { get; set; }
    }

  
    public class DavaDashboardDto
    {
        public int DerdestCount { get; set; }
        public int AktifDavaCount { get; set; }
        public int TemyizCount { get; set; }
        public List<HkDavaModeli> Davalar { get; set; }
    }
}
