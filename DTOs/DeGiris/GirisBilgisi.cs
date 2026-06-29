using System.ComponentModel.DataAnnotations;

namespace Guardia.API.DTOs.DeGiris
{
    public class GirisBilgisi
    {
        [Required(ErrorMessage = "Sicil No zorunludur.")]
        [RegularExpression(@"^[A-Za-zÇçĞğİıÖöŞşÜü]{2}[0-9]+$", ErrorMessage = "Sicil No 2 harf prefix + rakamlardan oluşmalıdır. Örn: IK001")]
        public string adminId { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string adminPass { get; set; }
    }
}