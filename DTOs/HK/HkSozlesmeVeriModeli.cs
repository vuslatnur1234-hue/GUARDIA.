namespace Guardia.API.DTOs.HK
{
    public class HkSozlesmeVeriModeli
    {
        public int Id { get; set; } 
        public string Baslik { get; set; }      
        public string Taraf { get; set; }    
        public string BitisTarihi { get; set; }  
        public int MevcutAsama { get; set; }      // 1-5 arası (Taslak'tan Aktif'e)
        public string AsamaDurumu { get; set; }   // YÜRÜRLÜKTE, İÇ ONAY vb.
        public string DurumBilgisi { get; set; }
        public string Kategori { get; set; }      
    }

    public class SozlesmeDashboardDto
    {
        // Üstteki 3 ana kart için veriler
        public int SuresiDolanCount { get; set; }
        public int KritikCount { get; set; }
        public int YururlukteCount { get; set; }
        public List<HkSozlesmeVeriModeli> Sozlesmeler { get; set; }
    }

}
