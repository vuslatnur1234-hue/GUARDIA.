namespace Guardia.API.DTOs.IK
{
    public class IkDuyuruModeli
    {

        public int Id { get; set; }
        public string Baslik { get; set; }
        public string HedefKitle { get; set; } 
        public string Kategori { get; set; }  
        public string Icerik { get; set; }
        public string Yayinlayan { get; set; } 
        public DateTime YayinTarihi { get; set; }
        public string DosyaAdi { get; set; } 
        public string DuyuruIcerigi { get; set; }

        public string OkunduOnay { get; set; } 
        public string MobilBildirimGonder { get; set; }

        public string EPostaGonder { get; set; }

    }
}
