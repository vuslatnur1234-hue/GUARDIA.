using Guardia.API.DTOs.PE;

namespace Guardia.API.Services.Personel
{
    public interface IPIzinServisi
    {
        bool IzinTalepGonder(PIzinTalepModeli model);
    }
}