namespace Guardia.API.DTOs.IK
{
    public class IkTurnoverAnalizModeli
    {
        public int ToplamAyrilan { get; set; }
        public double YillikOran { get; set; }
        public double SektorOrtalamasi { get; set; }
        public string RiskliDepartman { get; set; }
        public double RiskOrani { get; set; }

         

        public int IstenCikarilma { get; set; }
        public int BaskaIsTeklifi { get; set; }
        public int SehirDegisikligi { get; set; }
        public int EmeklilikSaglik { get; set; }
        
    }
}
