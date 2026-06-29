namespace Guardia.API.DTOs.IK
{
    public class IkBordroListeModel
    {
        private string durum;

        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string SicilNo { get; set; }
        public decimal BrutMaas { get; set; }
        public decimal SgkPayi { get; set; }
        public decimal GelirVergisi { get; set; }
        public decimal Kesintiler { get; set; }
        public decimal NetMaas { get; set; } 
        public string Durum { get ; set ; } // "Beklemede", "Onaylandı"
        public string Departman { get; set; }

        public decimal VergiOraniGosterge { get; set; }
        public int PersonelId { get; set; }
        public string Donem { get; set; }


    }
}
