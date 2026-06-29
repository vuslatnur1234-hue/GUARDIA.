using Guardia.API.DTOs;
using Guardia.API.DTOs.IK;
using Guardia.API.Models;


namespace Guardia.API.Services.InsanKaynaklari
{
    public interface IIkService
    {

        Task<IkPanelIstatistikleri> PanelIstatistikleriniGetirAsync();
        Task<List<IkPersonelGirisModeli>> GirisListesiniGetirAsync();
        Task<List<IkAktiviteModeli>> AktiviteleriGetirAsync(string kategori = "tumu");
        Task<List<IkDepartmanDagilimModeli>> DepartmanDagiliminiGetirAsync(string tip);
 
        IkMemnuniyetAnalizModeli MemnuniyetAnaliziGetir();
        Task<IkTurnoverAnalizModeli> TurnoverAnaliziGetirAsync();
        Task<List<IkSahaAktiflikModeli>> GetSahaAktiflikAsync();
        Task<IkPersonelDetayModeli> PersonelDetayiniGetirAsync(string sicilNo);
        Task<List<IkPersonelDetayModeli>> PersonelListesiniGetirAsync();
        void YeniPersonelKaydet(IkPersonelEkleModeli model);
        object BekleyenVeriyiGetir(string sicilNo);
        void GuncellemeTalebiOlustur(string sicilNo, Dictionary<string, string> veriler);
        void BekleyenTalebiSil(string sicilNo);
        bool PersonelGuncelle(string sicilNo, IkPersonelEkleModeli model);         
        Task<IkBordroOzetModel> GetBordroOzetAsync();
        Task<List<IkBordroListeModel>> GetBordroListesiAsync();
        Task<bool> BordroDurumGuncelleAsync(int id, string yeniDurum);
        Task<bool> TopluOdemeEmriGonderAsync();
        Task<bool> HepsiniOnaylaAsync();           
        List<IkDuyuruModeli> SonDuyurulariGetir();          
        Task<List<IkDuyuruModeli>> PersoneleGoreDuyuruGetir(string dept);
        Task<bool> PersonelSilAsync(string sicilNo);
        Task<bool> DuyuruKaydet(IkDuyuruModeli model);
        Task<List<IkIzinTalepModeli>> IzinListesiniGetirAsync();
        Task<IkIzinIstatistikModeli> IzinIstatistikleriniGetirAsync();
        Task<bool> IzinOnaylaAsync(int talepId);
        Task<bool> IzinReddetAsync(int talepId, string neden);
        Task<List<MesajModeli>> GelenMesajlariGetirAsync(string aktifBirim);
        Task<List<MesajModeli>> GidenMesajlariGetirAsync(string aktifBirim);
        Task<bool> MesajGonderAsync(string gonderenBirim, MesajModeli model);
        Dictionary<string, object> TumBekleyenVerileriGetir();

    }

}
        
