using Guardia.API.DTOs;
using Guardia.API.DTOs.IK;
                                                                                                                                                                                                                                                                                                                                                                             using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guardia.Tests
{
    internal interface IIkTestService
    {
        // Panel & İstatistikler
        IkPanelIstatistikleri PanelIstatistikleriniGetir();
        List<IkPersonelGirisModeli> GirisListesiniGetir();
        List<IkAktiviteModeli> AktiviteleriGetir();
        List<IkDepartmanDagilimModeli> DepartmanDagiliminiGetir(string tip);
        List<MesajModeli> GelenMesajlariGetir();
        IkMemnuniyetAnalizModeli MemnuniyetAnaliziGetir();
        IkTurnoverAnalizModeli TurnoverAnaliziGetir();
        List<IkSahaAktiflikModeli> GetSahaAktiflik();

        // Personel Yönetimi (CRUD)
        List<IkPersonelDetayModeli> PersonelListesiniGetir();
        IkPersonelDetayModeli PersonelDetayGetir(string sicilNo);
        void YeniPersonelKaydet(IkPersonelEkleModeli model);
        bool PersonelGuncelle(string sicilNo, IkPersonelEkleModeli model);
        bool PersonelSil(string sicilNo);

        // İzin Yönetimi
        List<IkIzinTalepModeli> IzinListesiniGetir();
        bool IzinOnayla(int talepId);
        bool IzinReddet(int talepId, string neden);
        IkIzinIstatistikModeli IzinIstatistikleriniGetir();

        // Duyuru Sistemi
        List<IkDuyuruModeli> SonDuyurulariGetir();
        bool DuyuruKaydet(IkDuyuruModeli yeniDuyuru);
        List<IkDuyuruModeli> PersoneleGoreDuyuruGetir(string dept);

        // Talep / Zil Hafızası (Dictionary)
        object BekleyenVeriyiGetir(string sicilNo);
        void GuncellemeTalebiOlustur(string sicilNo, object yeniVeriler);
        void BekleyenTalebiSil(string sicilNo);

        // Asenkron Bordro Sistemleri
        Task<List<IkBordroListeModel>> GetBordroListesiAsync();
        Task<IkBordroOzetModel> GetBordroOzetAsync();
        Task<bool> BordroDurumGuncelleAsync(int id, string yeniDurum);
        Task<bool> TopluOdemeEmriGonderAsync();
        Task<bool> HepsiniOnaylaAsync();

    }
}
                                                                                                               