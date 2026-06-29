using Guardia.API.DTOs;
using System.Threading.Tasks;

namespace Guardia.API.Services.ai
{
    public interface IAiServisi
    {
        Task<CevapModeli> SoruCevapla(SoruModeli istek);
    }
}