namespace Guardia.API.DTOs
{
    public class LoginRequestDTO
    {
        //Yönetici Sicil No
        public string SicilNo
        {
            get; set;
        }

        //Yönetici Şifre
        public string Sifre
        {
            get; set;
        }
    }
}
