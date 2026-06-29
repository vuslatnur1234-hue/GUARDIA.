using System.Collections.Generic;

namespace Guardia.API.DTOs.SA
{
    public class SaIndexModeli
    {
        // 1. Üstteki Ana Kartlar
        public string AylikToplamHarcama { get; set; }
        public double TedarikSuresi { get; set; }
        public double SektorOrtalamasi { get; set; }
        public string EnCokHarcananBirim { get; set; }
        public string KalanButce { get; set; }
        public int KalanButceYuzdesi { get; set; }

        // 2. Grafik Verileri 
        public Dictionary<string, double[]> GrafikVerileri { get; set; }

        // 3. Harcama Kategorileri 
        public int HammaddeYuzdesi { get; set; }
        public int LojistikYuzdesi { get; set; }
        public int BakimYuzdesi { get; set; }
        public int DigerYuzdesi { get; set; }

        // 4. Tasarruf / Verimlilik Analizi 
        public int VerimlilikBasariYuzdesi { get; set; }
        public string ToplamTasarruf { get; set; }
        public string IslemSayisi { get; set; }

        // Modalin içindeki kalemler ve tutarları için
        public Dictionary<string, string> EnCokHarcananBirimDetaylari { get; set; }
        public List<SaIndexBildirimModeli> GelenBildirimler { get; set; }
        public int BildirimSayisi { get; set; } 
    }
}