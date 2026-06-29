namespace Guardia.API.DTOs.HK
{
    public class HkMevzuatModeli
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string Kategori { get; set; } 
        public string Ozet { get; set; }
        public string OnemDerecesi { get;  set; }  // Acil Aksiyon, Bilgi, Güncelleme
        public string Tarih { get; set; }
        public string KaynakUrl { get; set; }
    }

    public class MevzuatDashboardDto
    {
        public List<HkMevzuatModeli> Mevzuatlar { get; set; }
    }
}
