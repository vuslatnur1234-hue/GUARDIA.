using Guardia.API.DTOs;
using Guardia.API.DTOs.PE;

namespace Guardia.API.Services.Personel
{
    namespace Guardia.API.Services.Interfaces
    {
        public interface PersonelIPanelServisi
        {
            // İçine string sicilNo eklendi
            PersonelPanelModeli PanelBilgisiniGetir(string sicilNo);
        }

        public interface IPProfilServisi
        {
            // İçine string sicilNo eklendi
            PersonelProfilModeli ProfilBilgileriniGetir(string sicilNo);
            bool ProfilGuncelle(PersonelProfilModeli model);
        }
    }
}