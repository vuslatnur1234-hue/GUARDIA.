using Guardia.API.DTOs.II;
using Guardia.API.Services.IdariIsler;
using static Guardia.API.Services.IdariIsler.IiDashboardServisi;
// using Guardia.Data; // → DbContext'inizi buraya ekleyin (örn. AppDbContext)

namespace Guardia.API.Services.IdariIsler
{
    public class IiDashboardServisi : IiiDashboardServisi
    {
        

            public async Task<IiDashboardModeli> DashboardVerisiniGetirAsync()
            {
                
                var stokTask = StokDagiliminiGetirAsync(2); // varsayılan: Mart
                var ziyaretciTask = ZiyaretciYogunlugunuGetirAsync();
                var aracTask = AracHareketleriniGetirAsync();

                await Task.WhenAll(stokTask, ziyaretciTask, aracTask);

                return new IiDashboardModeli
                {
                    StokDagilimi = stokTask.Result,
                    ZiyaretciYogunlugu = ziyaretciTask.Result,
                    SonAracHareketleri = aracTask.Result
                };
            }

         

            public async Task<IiStokDagilimModeli> StokDagiliminiGetirAsync(int ayIndex)
            {
               
                var aylar = new[] { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

              
                var ornekVeri = new Dictionary<int, (int Ofis, int Hijyen, int Mutfak)>
            {
                { 0, (40, 55, 20) },
                { 1, (50, 45, 35) },
                { 2, (45, 60, 30) }
            };

            (int Ofis, int Hijyen, int Mutfak) veri = ornekVeri.GetValueOrDefault(ayIndex, (0, 0, 0));
            int yil = DateTime.Now.Year;

                return await Task.FromResult(new IiStokDagilimModeli
                {
                    AyEtiketi = $"{aylar[ayIndex]} {yil}",
                    AyIndex = ayIndex,
                    OfisAdeti = veri.Ofis,
                    HijyenAdeti = veri.Hijyen,
                    MutfakAdeti = veri.Mutfak
                });
            }

          

            public async Task<List<IiZiyaretciYogunlukModeli>> ZiyaretciYogunlugunuGetirAsync()
            {         

                return await Task.FromResult(new List<IiZiyaretciYogunlukModeli>
            {
                new() { HaftaEtiketi = "1. Hafta", ZiyaretciSayisi = 45 },
                new() { HaftaEtiketi = "2. Hafta", ZiyaretciSayisi = 62 },
                new() { HaftaEtiketi = "3. Hafta", ZiyaretciSayisi = 38 },
                new() { HaftaEtiketi = "4. Hafta", ZiyaretciSayisi = 55 }
            });
            }

            
            public async Task<List<IiAracHareketiModeli>> AracHareketleriniGetirAsync(string? aramaMetni = null)
            {
               

                var liste = new List<IiAracHareketiModeli>
            {
                new() { Id = 1, Plaka = "34 ABC 111", SurucuAdi = "Caner Öz",   Saat = "09:45", Durum = "Yolda"     },
                new() { Id = 2, Plaka = "06 GIA 55",  SurucuAdi = "Selin A.",   Saat = "10:20", Durum = "Dönüşte"   },
                new() { Id = 3, Plaka = "35 KRT 99",  SurucuAdi = "Mehmet V.",  Saat = "11:05", Durum = "Beklemede" },
                new() { Id = 4, Plaka = "34 DEF 456", SurucuAdi = "Ayşe K.",    Saat = "12:30", Durum = "Yolda"     }
            };

                if (!string.IsNullOrWhiteSpace(aramaMetni))
                {
                    var filtre = aramaMetni.ToLower();
                    liste = liste.Where(a =>
                        a.Plaka.ToLower().Contains(filtre) ||
                        a.SurucuAdi.ToLower().Contains(filtre)).ToList();
                }

                return await Task.FromResult(liste);
            }

         

            public async Task<List<IiOperasyonNotuModeli>> NotlariGetirAsync(int kullaniciId)
            {
             
                return await Task.FromResult(new List<IiOperasyonNotuModeli>());
            }

            public async Task<int> NotEkleAsync(IiOperasyonNotuModeli not)
            {
                not.Id = new Random().Next(1000, 9999); 
                return await Task.FromResult(not.Id);
            }

            public async Task<int> NotlariSilAsync(List<int> notIdleri)
            {        

                return await Task.FromResult(notIdleri.Count);
            }
        }
}
