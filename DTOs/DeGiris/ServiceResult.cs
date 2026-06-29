namespace Guardia.API.DTOs.DeGiris
{
    public class ServiceResult
    {
        public bool Basarili { get; set; }
        public string HataMesaji { get; set; }
        public object Veri { get; set; }

        public static ServiceResult Success(object veri = null) => new ServiceResult { Basarili = true, Veri = veri };
        public static ServiceResult Failure(string mesaj) => new ServiceResult { Basarili = false, HataMesaji = mesaj };
    }
}
