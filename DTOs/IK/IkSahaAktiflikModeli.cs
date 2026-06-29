namespace Guardia.API.DTOs.IK
{
    public class IkSahaAktiflikModeli
    {
        public string BirimAdi { get; set; } = null!;
        public string AltBilgi { get; set; } = null!;
        public int MevcutPersonel { get; set; }
        public int ToplamKapasite { get; set; }
        public string MetrikAdi { get; set; } = null!; 
        public int MetrikYuzde { get; set; }
        public string DurumEtiketi { get; set; } = null!;
        public string Tema { get; set; } = null!; 
    }
}
