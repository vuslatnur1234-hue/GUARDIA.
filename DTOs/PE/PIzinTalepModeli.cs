namespace Guardia.API.DTOs.PE
{
    public class PIzinTalepModeli
    {
        public string SicilNo { get; set; }
        public string IzinTuru { get; set; }
        public DateOnly BaslangicTarihi { get; set; }
        public DateOnly BitisTarihi { get; set; }
        public string Aciklama { get; set; }
    }
}