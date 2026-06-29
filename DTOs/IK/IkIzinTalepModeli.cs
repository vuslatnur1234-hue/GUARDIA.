namespace Guardia.API.DTOs.IK
{
    public class IkIzinTalepModeli
    {
        public int Id { get; set; }
        public string PersonelAd { get; set; }
        public string Departman { get; set; }   
        public string SicilNo { get; set; }
        public string IzinTuru { get; set; }
        public string TarihAraligi { get; set; }
        public string Durum { get; set; } // "Bekliyor", "Onaylandı", "Reddedildi"
        public int Sure { get; set; }           
        public int KalanGun { get; set; }
        public string BaslangicTarihi { get; set; }
        public string BitisTarihi { get; set; }
    }
}
