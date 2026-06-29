using Guardia.API.DTOs.HK;
using Guardia.API.Models;

namespace Guardia.API.Services.Hukuk
{
    public interface IHkService
    {
        Task<HkDashboardModeli> GetAylikAnalizAsync(int yil, int ay);
        Task<List<HkTrendVeriModeli>> GetYillikTrendAsync(int yil);
        Task<DavaDashboardDto> GetDavaDashboardAsync();
        Task<bool> DavaEkleAsync(HkDavaModeli yeniDava);
        Task<bool> DavaNotGuncelleAsync(int id, string yeniNot);
        Task<bool> DavaAsamaIlerletAsync(int id);
        Task<MevzuatDashboardDto> GetMevzuatDashboardAsync();
        Task<HkMevzuatModeli> MevzuatEkleAsync(HkMevzuatModeli yeni);          
        Task<List<HkHatirlaticilar>> GetHatirlaticilarAsync();
        Task<HkHatirlaticilar> HatirlaticiEkleAsync(HkHatirlaticilar model);
        Task<bool> HatirlaticiSilAsync(int id); 
        Task<SozlesmeDashboardDto> GetDashboardAsync();
        Task<HkSozlesmeVeriModeli> AsamaIlerletAsync(long id);
        Task<HkSozlesmeVeriModeli> SozlesmeEkleAsync(HkSozlesmeVeriModeli yeniDto);
        Task<bool> SureUzatAsync(int id, string yeniSureMetni);
        Task<HkDijitalArsivModeli> ArsivEkleAsync(HkDijitalArsivModeli model);
        Task<bool> ArsivGuncelleAsync(int id, string yeniDurum, string dosyaUrl = null);
        Task<bool> DosyaImhaEtAsync(int id);
        Task<List<HkDijitalArsivModeli>> GetArsivListesiAsync();
    }
}

