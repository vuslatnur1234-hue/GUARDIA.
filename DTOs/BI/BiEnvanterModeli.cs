namespace Guardia.API.DTOs.BI
{
    public class BiEnvanterModeli
    {
        public int Id { get; set; } 
        public string CihazTuru { get; set; }
        public string Durum { get; set; }// kullanımda,servis dışı,stokta, hurda
                                      
        public string CihazModeli { get; set; }

        public string SeriNo { get; set; } //benzersiz ıd

         public string ZimmetliKisi { get; set; } // cihazın kime zimmetli olduğunu gösterir

        public string Lokasyon { get; set; } // cihazın fiziksel olarak nerede olduğunu gösterir.
    }
}
