namespace Guardia.API.DTOs.BI
{
    public class BiAylikTalepModeli
    {
        public string AyAdi { get; set; }
        public int Cozulen { get; set; }
        public int Bekleyen { get; set; }
        public int Oran => Cozulen + Bekleyen == 0 ? 0 : Cozulen * 100 / (Cozulen + Bekleyen);
    }
}
