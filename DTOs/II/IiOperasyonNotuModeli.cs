namespace Guardia.API.DTOs.II
{
    public class IiOperasyonNotuModeli
    {
        public int Id { get; set; }

      
        public int KullaniciId { get; set; }

        public string Metin { get; set; } = string.Empty;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}
