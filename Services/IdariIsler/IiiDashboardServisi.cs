using Guardia.API.DTOs.II;

namespace Guardia.API.Services.IdariIsler
{
    public interface IiiDashboardServisi
    {
       
       
        Task<IiDashboardModeli> DashboardVerisiniGetirAsync();
        Task<IiStokDagilimModeli> StokDagiliminiGetirAsync(int ayIndex);
        Task<List<IiZiyaretciYogunlukModeli>> ZiyaretciYogunlugunuGetirAsync();
        Task<List<IiAracHareketiModeli>> AracHareketleriniGetirAsync(string? aramaMetni = null);    
        Task<List<IiOperasyonNotuModeli>> NotlariGetirAsync(int kullaniciId);
        Task<int> NotEkleAsync(IiOperasyonNotuModeli not);  
        Task<int> NotlariSilAsync(List<int> notIdleri);
    }
}
