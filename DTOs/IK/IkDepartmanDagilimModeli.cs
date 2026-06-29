namespace Guardia.API.DTOs.IK
{
    public class IkDepartmanDagilimModeli
    {
        public string DepartmanAdi { get; set; } = null!;
        public int KisiSayisi { get; set; }
        public string Tip { get; set; } = null!;   // idari | teknik | saha
    } 
}
