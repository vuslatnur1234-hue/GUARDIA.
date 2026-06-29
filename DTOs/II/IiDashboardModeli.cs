namespace Guardia.API.DTOs.II
{
    public class IiDashboardModeli
    {
        public IiStokDagilimModeli StokDagilimi { get; set; } = new();
        public List<IiZiyaretciYogunlukModeli> ZiyaretciYogunlugu { get; set; } = new();
        public List<IiAracHareketiModeli> SonAracHareketleri { get; set; } = new();
    }
}
