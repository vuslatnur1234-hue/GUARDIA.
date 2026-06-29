using System;

namespace Guardia.API.Models
{
    public partial class Arizalar
    {
        public long Id { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public long? PersonelId { get; set; }
        public string? Baslik { get; set; }
        public string? Kategori { get; set; }
        public string? Oncelik { get; set; }
        public string? Aciklama { get; set; }
        public string? AtananKisi { get; set; }
        public string? Durum { get; set; }
        public string? Lokasyon { get; set; }
        public string? TakipNo { get; set; }

        public virtual Personeller? Personel { get; set; }
    }
}