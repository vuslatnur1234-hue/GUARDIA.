using System.Collections.Generic;
using Guardia.API.DTOs.SA;

namespace Guardia.API.Services.SatinAlma
{
    public class SatinAlmaIndexServisi : IIndexSatinAlmaServisi
    {
        public SaIndexModeli IndexVerileriniGetir()
        {
           
            return new SaIndexModeli
            {
                AylikToplamHarcama = "3M",
                TedarikSuresi = 10,
                SektorOrtalamasi = 5.1,
                EnCokHarcananBirim = "Üretim Hatti",
                KalanButce = "2M",
                KalanButceYuzdesi = 55,

                EnCokHarcananBirimDetaylari = new Dictionary<string, string>
                {
                    { "Hammadde Alımı", "₺1.200.000" },
                    { "Makine Yedek Parça", "₺850.000" },
                    { "Birim İşçilik", "₺3" }
                },

                // Grafiğin dinamik datası
                GrafikVerileri = new Dictionary<string, double[]>
                {
                    { "2024", new double[] { 1.82, 1.95, 2.08, 2.01, 2.18, 2.24, 2.11, 2.05, 2.19, 2.38, 2.31, 2.55 } },
                    { "2025", new double[] { 2.48, 2.38, 2.65, 2.58, 2.72, 2.68, 2.61, 2.85, 2.79, 2.95, 2.88, 3.05 } },
                    { "2026", new double[] { 20.0, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 } }
                },

                // Kategoriler
                HammaddeYuzdesi = 95,  
                LojistikYuzdesi = 2,   
                BakimYuzdesi = 2,      
                DigerYuzdesi = 1,      

                // Radar (Tasarruf) Rakamları
                VerimlilikBasariYuzdesi = 84,
                ToplamTasarruf = "142.500",
                IslemSayisi = "42/45 İndirim",

                // GELEN BİLDİRİMLER (YENİ EKLENEN KISIM)
                BildirimSayisi = 3,
                GelenBildirimler = new List<SaIndexBildirimModeli>
                {
                    new SaIndexBildirimModeli { 
                        GonderenBirim = "ÜRETİM", 
                        Ozet = "Hammadde stoğu kritik seviyede!", 
                        MesajDetayi = "Hammadde stoğu %10 altına düştü. Acil tedarik formu oluşturulsun mu?", 
                        ZamanTabiri = "5 dk önce", 
                        RenkKodu = "#ef4444" // Kırmızı
                    },
                    new SaIndexBildirimModeli { 
                        GonderenBirim = "İDARİ İŞLER", 
                        Ozet = "Araç lastik ihalesi açıldı.", 
                        MesajDetayi = "Araç lastik ihalesi için 3 farklı firmadan teklif toplandı, onayınıza sunuldu.", 
                        ZamanTabiri = "1 saat önce", 
                        RenkKodu = "#3b82f6" // Mavi
                    },
                    new SaIndexBildirimModeli { 
                        GonderenBirim = "gına geldi", 
                        Ozet = "Yeni nakliye teklifi geldi.", 
                        MesajDetayi = "Yeni nakliye firması sözleşme taslağı incelenmek üzere hazır.", 
                        ZamanTabiri = "3 saat önce", 
                        RenkKodu = "#f59e0b" // Sarı
                    }
                }
            };
        }
    }
}