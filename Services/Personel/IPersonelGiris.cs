using Guardia.API.DTOs.PeGiris;

namespace Guardia.API.Services.Personel
{
    public interface IPersonelGiris
    {
        bool GirisYap(PersonelGirisBilgisi bilgi);
    }
}