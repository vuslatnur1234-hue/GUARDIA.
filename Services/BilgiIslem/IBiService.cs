using Guardia.API.DTOs.BI;

namespace Guardia.API.Services.BilgiIslem
{
    public interface IBiService
    {
        
        Task<List<BiTeknikImkanModeli>> TumImkanlariListeleAsync();
        Task<List<BiTeknikImkanModeli>> DepartmanBazliImkanlarAsync(string departman);
        Task<BiEnvanterOzetiModeli> GetEnvanterOzetiAsync();
        Task<int> GetSon24SaatZimmetSayisiAsync();
        Task<List<BiAylikTalepModeli>> GetAylikTalepIstatisikleriAsync();
        Task<List<BiGunlukSistemModeli>> GetHaftalikSistemCalismaSuresiAsync();     
        Task<BiYedeklemeOzetiModeli> GetYedeklemeOzetiAsync();
        Task<List<BiEnvanterModeli>> GetTumEnvanterAsync();
        Task<bool> EnvanterEkleAsync(BiEnvanterModeli yeniCihaz);
        Task<bool> EnvanterGuncelleAsync(int id, BiEnvanterModeli guncelCihaz);
        Task<bool> EnvanterSilAsync(int id);
        Task<BiEnvanterModeli> GetByIdAsync(int id);

    }
}
