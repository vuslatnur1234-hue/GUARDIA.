using Guardia.API.DTOs.DeGiris;

namespace Guardia.API.Services.AuthDepartman
{
    public interface IAuthService
    {

        ServiceResult GirisYap(GirisBilgisi bilgi);
        ServiceResult KodGonder(SifreSifirlamaBilgisi bilgi);
        ServiceResult KoduDogrula(KodDogrulamaBilgisi bilgi);
        ServiceResult SifreyiGuncelle(YeniSifreBilgisi bilgi);
    }
}
