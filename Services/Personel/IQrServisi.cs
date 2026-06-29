using Guardia.API.DTOs.PE;

namespace Guardia.API.Services.Personel
{
    public interface IQrServisi
    {
        QrModeli QrOlustur(string sicilNo);
    }
}