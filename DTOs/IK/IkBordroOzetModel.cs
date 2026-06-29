namespace Guardia.API.DTOs.IK
{
    public class IkBordroOzetModel
    {
        public decimal BrutToplam { get; set; }
        public decimal NetOdenen { get; set; }
        public int BekleyenCount { get; set; }
        public decimal SgkIsverenToplami { get; set; }
        public decimal SgkIsciToplami { get; set; }
        public decimal GelirVergisiToplami { get; set; }
        public decimal DamgaVergisiToplami { get; set; }
        public decimal ToplamYasalYukumluluk { get; set; }

        public decimal BankaMaasOdemeleri { get; set; }
        public decimal IkramiyeBonusOdemeleri { get; set; }
        public decimal YanHaklarYolYemek { get; set; }

        public int Ay { get; set; } 
    }
}
