using Guardia.API.Data;
using Guardia.API.DTOs.HK;
using Guardia.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Guardia.API.Services.Hukuk
{
    public class HkService : IHkService
    {
        private readonly AppDbContext _context;

        public HkService(AppDbContext context)
        {
            _context = context;
        }

     
        private static List<HkHatirlaticiModeli> _hatirlaticilar = new List<HkHatirlaticiModeli>
{
    new HkHatirlaticiModeli { Id = 1, Icerik = "Esma Yılmaz Duruşma Hazırlığı", TarihEtiketi = "Yarın 10:30", IsTamamlandi = false },
    new HkHatirlaticiModeli { Id = 2, Icerik = "Tedarikçi A.Ş. Sözleşme Revizyonu", TarihEtiketi = "25 Mart", IsTamamlandi = false }
};
        
        // hatırlatıcılar veritabanından çekiliyor.
        public async Task<List<HkHatirlaticilar>> GetHatirlaticilarAsync()
        {
           
            return await _context.HkHatirlaticilars
                .Where(x => !x.IsTamamlandi)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

     
        // yeni hatırlatıcı ekleniyor.
        public async Task<HkHatirlaticilar> HatirlaticiEkleAsync(HkHatirlaticilar model)
        {
            if (model == null) return null;

            
            if (string.IsNullOrWhiteSpace(model.TarihEtiketi))
            {
                model.TarihEtiketi = DateTime.Now.ToString("dd MMMM", new System.Globalization.CultureInfo("tr-TR")); 
            }

            model.IsTamamlandi = false;
            model.CreatedAt = DateTimeOffset.UtcNow;

            _context.HkHatirlaticilars.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }

 
        // hatırlatıcı tamamlandı olarak işaretleniyor.
        public async Task<bool> HatirlaticiSilAsync(int id)
        {
            var not = await _context.HkHatirlaticilars.FindAsync(id);
            if (not == null)
            {
                return false; 
            }
            _context.HkHatirlaticilars.Remove(not);
            await _context.SaveChangesAsync();

            return true; 
        }


        // Aylık analiz verileri hazırlanıyor.
        public async Task<HkDashboardModeli> GetAylikAnalizAsync(int yil, int ay)
        {
            var oAitDavalar = await _context.Davalars
      .Where(x => x.DurusmaTarihi != null && x.DurusmaTarihi.Value.Year == yil && x.DurusmaTarihi.Value.Month == ay)
      .ToListAsync();
            
            double toplamKapananDava = oAitDavalar
                .Count(x => x.Durum != "Devam Ediyor" && x.Durum != "Yeni");

          
            double kazanilanDava = oAitDavalar
                .Count(x => x.Durum == "Kazanıldı" || x.Durum == "Başarılı");

         
            double hesaplananKazanmaOrani = toplamKapananDava > 0
                ? Math.Round(kazanilanDava / toplamKapananDava * 100, 1)
                : 0;

       
            int toplamHacim = oAitDavalar.Count;

          
            var analizListesi = await GenerateRiskDagilimi(yil, ay);

            return new HkDashboardModeli
            {
                GenelKazanmaOrani = hesaplananKazanmaOrani,
                RiskDagilimi = analizListesi
            };
        }

        // Risk dağılımı verileri hazırlanıyor.
        private async Task<List<RiskAnalizDto>> GenerateRiskDagilimi(int yil, int ay)
        {
            
            var oAitDavalar = await _context.Davalars
    .Where(x => x.DurusmaTarihi != null && x.DurusmaTarihi.Value.Year == yil && x.DurusmaTarihi.Value.Month == ay)
    .ToListAsync();

            int toplamDavaSayisi = oAitDavalar.Count;

        
            if (toplamDavaSayisi == 0)
            {
                return new List<RiskAnalizDto>();
            }

          
            var dagilim = oAitDavalar
                .GroupBy(x => x.Durum)
                .Select(g => {
                    int davaSayisi = g.Count();
           
                    double yuzde = Math.Round((double)davaSayisi / toplamDavaSayisi * 100, 1);

            
                    string durumEtiketi = "TAKİPTE";
                    string renkKodu = "#eff6ff"; 

                    if (g.Key == "Kritik" || g.Key == "Acil")
                    {
                        durumEtiketi = "ACİL";
                        renkKodu = "#fee2e2"; 
                    }
                    else if (g.Key == "Devam Ediyor" || g.Key == "Yeni")
                    {
                        durumEtiketi = "DİKKAT";
                        renkKodu = "#fef3c7";
                    }
                    else if (g.Key == "Kazanıldı" || g.Key == "Sonuçlandı")
                    {
                        durumEtiketi = "GÜVENLİ";
                        renkKodu = "#dcfce7"; 
                    }

                    return new RiskAnalizDto
                    {
                        KategoriAd = g.Key ?? "Belirtilmemiş", 
                        Yuzde = yuzde,
                        DurumEtiketi = durumEtiketi,
                        RenkKodu = renkKodu
                    };
                })
                .OrderByDescending(x => x.Yuzde) 
                .ToList();

            return dagilim;
        }


        // Yıllık trend verileri hazırlanıyor.
        public async Task<List<HkTrendVeriModeli>> GetYillikTrendAsync(int yil)
        {
           
            var yillikDavalar = await _context.Davalars
     .Where(x => x.DurusmaTarihi != null &&
                 x.DurusmaTarihi!.Value.Year == yil)
     .AsNoTracking()
     .ToListAsync();

          
            var bugun = DateTime.Now;
            int mevcutYil = bugun.Year;
            int mevcutAy = bugun.Month;

       
            string[] aylar = { "OCA", "ŞUB", "MAR", "NİS", "MAY", "HAZ", "TEM", "AĞU", "EYL", "EKİ", "KAS", "ARA" };

            var trendListesi = new List<HkTrendVeriModeli>();

          
            for (int i = 1; i <= 12; i++)
            {
             
                int oAitDosyaSayisi = yillikDavalar.Count(x => x.DurusmaTarihi != null && x.DurusmaTarihi.Value.Month == i);

                
                bool isAktifAy = (yil == mevcutYil && i == mevcutAy);

                trendListesi.Add(new HkTrendVeriModeli
                {
                    AyAd = aylar[i - 1],       
                    DosyaSayisi = oAitDosyaSayisi,
                    IsAktif = isAktifAy
                });
            }

            return trendListesi;
        }


     
        // Sözleşme dashboard verileri hazırlanıyor.
        public async Task<SozlesmeDashboardDto> GetDashboardAsync()
        {
            var dbSozlesmeler = await _context.Sozlesmeler.ToListAsync();

            var frontendModelListesi = dbSozlesmeler.Select(x => new HkSozlesmeVeriModeli
            {
                Id = (int)x.Id, 
                Taraf = x.Taraf,
                BitisTarihi = x.BitisTarihi.ToString("dd.MM.yyyy"),
                Baslik = x.Tur,
                Kategori = x.Tur,
                AsamaDurumu = x.Durum,
                DurumBilgisi = x.Durum,
                MevcutAsama = x.Durum switch
                {
                    "TASLAK" => 1,
                    "İÇ ONAY" => 2,
                    "REVİZYON" => 3,
                    "İMZA BEKLİYOR" => 4,
                    "YÜRÜRLÜKTE" => 5,
                    _ => 1
                }
            }).ToList();

            return new SozlesmeDashboardDto
            {
                SuresiDolanCount = dbSozlesmeler.Count(x => x.Durum == "SÜRESİ DOLAN"),
                KritikCount = dbSozlesmeler.Count(x => x.Durum == "KRİTİK"),
                YururlukteCount = dbSozlesmeler.Count(x => x.Durum == "YÜRÜRLÜKTE"),
                Sozlesmeler = frontendModelListesi
            };
        }


        // Sözleşme aşama ilerletme işlemi gerçekleştiriliyor.
        public async Task<HkSozlesmeVeriModeli> AsamaIlerletAsync(long id)
        {
            var sozlesme = await _context.Sozlesmeler.FindAsync(id);
            if (sozlesme == null) return null;

          
            int mevcutAsama = sozlesme.Durum switch
            {
                "TASLAK" => 1,
                "İÇ ONAY" => 2,
                "REVİZYON" => 3,
                "İMZA BEKLİYOR" => 4,
                "YÜRÜRLÜKTE" => 5,
                _ => 1
            };

            if (mevcutAsama < 5)
            {
                mevcutAsama++;

              
                sozlesme.Durum = mevcutAsama switch
                {
                    2 => "İÇ ONAY",
                    3 => "REVİZYON",
                    4 => "İMZA BEKLİYOR",
                    5 => "YÜRÜRLÜKTE",
                    _ => "TASLAK"
                };

                _context.Sozlesmeler.Update(sozlesme);
                await _context.SaveChangesAsync();
            }

          
            return new HkSozlesmeVeriModeli
            {
                Id = (int)sozlesme.Id,
                Baslik = sozlesme.Tur,
                Taraf = sozlesme.Taraf,
                BitisTarihi = sozlesme.BitisTarihi.ToString("dd.MM.yyyy"),
                MevcutAsama = mevcutAsama,
                AsamaDurumu = sozlesme.Durum,
                DurumBilgisi = sozlesme.Durum,
                Kategori = sozlesme.Tur
            };
        }

     
        // Yeni sözleşme ekleme işlemi gerçekleştiriliyor.
        public async Task<HkSozlesmeVeriModeli> SozlesmeEkleAsync(HkSozlesmeVeriModeli yeniDto)
        {
            if (yeniDto == null) return null;

     
            DateTime bitisZamani;
            if (!string.IsNullOrEmpty(yeniDto.BitisTarihi) && DateTime.TryParse(yeniDto.BitisTarihi, out var parsedDate))
            {
                bitisZamani = parsedDate;
            }
            else
            {
                bitisZamani = DateTime.Now.AddYears(1); 
            }

          
            var dbModel = new Sozlesmeler
            {
                Taraf = yeniDto.Taraf,
                Tur = yeniDto.Baslik ?? yeniDto.Kategori ?? "Genel",

              
                BaslangicTarihi = DateTime.Now,
                BitisTarihi = bitisZamani,

                Durum = "TASLAK", 
                DosyaUrl = "sozlesme/varsayilan.pdf",
                OlusturanId = 1, 
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Sozlesmeler.Add(dbModel);
            await _context.SaveChangesAsync();

          
            yeniDto.Id = (int)dbModel.Id;
            yeniDto.MevcutAsama = 1;
            yeniDto.AsamaDurumu = "TASLAK";
            yeniDto.DurumBilgisi = "TASLAK";

            return yeniDto;
        }


   
        // Dava dashboard verileri hazırlanıyor.
        public async Task<DavaDashboardDto> GetDavaDashboardAsync()
        {
      
            var dbList = await _context.Davalars
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            
            var davaModelleri = dbList.Select(x =>
            {
              
                string hamDurum = (x.Durum ?? "").Trim().ToLower();
                string guncelDurum = "Derdest"; 
              

                if (hamDurum == "yeni" || hamDurum == "devam ediyor")
                {
                    guncelDurum = "Derdest";
                }
                else if (hamDurum == "kritik" || hamDurum == "karar aşaması" || hamDurum == "karar aşamasında")
                {
                    guncelDurum = "Karar Aşamasında";
                }
                else if (hamDurum == "temyiz" || hamDurum == "temyizde")
                {
                    guncelDurum = "Temyizde";
                }

                return new HkDavaModeli
                {
                    Id = (int)x.Id,
                    DosyaNo = x.DosyaNo,
                    Mahkeme = string.IsNullOrEmpty(x.Mahkeme) ? "İstanbul . İş Mahkemesi" : x.Mahkeme,
                    KarsiTaraf = x.KarsiTaraf ?? "Belirtilmedi",
                    Konu = x.Konu ?? "Genel Hukuk Davası",
                    Asama = x.Asama ?? "Dilekçeler Teatisi",
                    DurusmaTarihi = x.DurusmaTarihi.HasValue ? x.DurusmaTarihi.Value.ToString("dd.MM.yyyy") : "Belirtilmedi",
                    Durum = guncelDurum, 
                    YoneticiNotu = x.YoneticiNotu ?? ""
                };
            }).ToList();

      
            int derdestSayisi = davaModelleri.Count(x => x.Durum == "Derdest");
            int temyizSayisi = davaModelleri.Count(x => x.Durum == "Temyizde");
            int aktifDavaSayisi = davaModelleri.Count(x => x.Durum != "KAPANDI");

            return new DavaDashboardDto
            {
                DerdestCount = derdestSayisi,
                AktifDavaCount = aktifDavaSayisi,
                TemyizCount = temyizSayisi,
                Davalar = davaModelleri
            };
        }

        // Yeni dava ekleme işlemi gerçekleştiriliyor.
        public async Task<bool> DavaEkleAsync(HkDavaModeli model)
        {
            var yeniDava = new Davalar
            {
                DosyaNo = model.DosyaNo,
                Mahkeme = model.Mahkeme,
                KarsiTaraf = model.KarsiTaraf,
                Konu = model.Konu,
                Asama = "Dava Açıldı",
                DurusmaTarihi = DateTimeOffset.Parse(model.DurusmaTarihi),
                Durum = model.Durum ?? "DERDEST",
                CreatedAt = DateTimeOffset.Now,
                YoneticiNotu = model.YoneticiNotu ?? ""
            };

            _context.Davalars.Add(yeniDava);
            return await _context.SaveChangesAsync() > 0;
        }


        // Dava yönetici notu güncelleme işlemi gerçekleştiriliyor.
        public async Task<bool> DavaNotGuncelleAsync(int id, string yeniNot)
        {
            var dava = await _context.Davalars.FirstOrDefaultAsync(x => x.Id == id);
            if (dava == null) return false;

            dava.YoneticiNotu = yeniNot;

            _context.Davalars.Update(dava);
            await _context.SaveChangesAsync();
            return true;
        }

        // Dava aşama ilerletme işlemi gerçekleştiriliyor.
        public async Task<bool> DavaAsamaIlerletAsync(int id)
        {
            var dava = await _context.Davalars.FindAsync((long)id);
            if (dava == null) return false;

            string mevcutAsama = dava.Asama?.Trim();
            string mahkemeAdi = dava.Mahkeme?.ToUpper() ?? "";

            if (mevcutAsama == "Dava Açıldı")
            {
                dava.Asama = "Dilekçeler";
                dava.Durum = "DERDEST";
            }
            else if (mevcutAsama == "Dilekçeler")
            {
                dava.Asama = "Bilirkişi";
                dava.Durum = "DERDEST";
            }
            else if (mevcutAsama == "Bilirkişi")
            {
               
                dava.Asama = "Karar";

              
                if (mahkemeAdi.Contains("BAM") || mahkemeAdi.Contains("YARGITAY") || dava.Durum == "TEMYİZDE")
                {
                    dava.Durum = "TEMYİZDE";
                }
                else
                {
                    dava.Durum = "KARAR AŞAMASINDA";
                }
            }
            else
            {
                return false; 
            }

            return await _context.SaveChangesAsync() > 0;
        }

       //mevzuat dashboard verileri hazırlanıyor.
        public async Task<MevzuatDashboardDto> GetMevzuatDashboardAsync()
        {
            var dbMevzuatlar = await _context.Mevzuatlar
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var frontendModelListesi = dbMevzuatlar.Select(x => new HkMevzuatModeli
            {
                Id = (int)x.Id,
                Baslik = x.Baslik,
                Kategori = x.Kategori,
                Ozet = x.Ozet,
                OnemDerecesi = x.OnemDerecesi,
                Tarih = x.YayinTarihi.ToString("dd.MM.yyyy"),
                KaynakUrl = x.DosyaUrl
            }).ToList();

            return new MevzuatDashboardDto
            {
                Mevzuatlar = frontendModelListesi
            };
        }

    
        // Yeni mevzuat ekleme işlemi gerçekleştiriliyor.
        public async Task<HkMevzuatModeli> MevzuatEkleAsync(HkMevzuatModeli yeniDto)
        {
            if (yeniDto == null) return null;

            DateTime mevzuatTarihi;

            if (!string.IsNullOrEmpty(yeniDto.Tarih))
            {
                
                string[] kabulEdilenFormatlar = { "dd.MM.yyyy", "yyyy-MM-dd", "dd/MM/yyyy" };

                bool parseBasarili = DateTime.TryParseExact(
                    yeniDto.Tarih.Trim(),
                    kabulEdilenFormatlar,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var parsedDate
                );

                if (parseBasarili)
                {
                    mevzuatTarihi = parsedDate;
                }
                else
                {
                  
                    mevzuatTarihi = DateTime.Now;
                }
            }
            else
            {
                mevzuatTarihi = DateTime.Now; 
            }

         
            string otomatikUrl = yeniDto.Kategori switch
            {
                "KVKK" => "https://www.kvkk.gov.tr",
                "Vergi & Maliye" => "https://www.gib.gov.tr",
                "Tüketici Hukuku" => "https://tuketicisikayeti.ticaret.gov.tr",
                "Ticaret" => "https://www.ticaret.gov.tr",
                "İş Hukuku" => "https://www.csgb.gov.tr",
                _ => "https://www.resmigazete.gov.tr"
            };

            var dbMevzuat = new Mevzuatlar
            {
                Baslik = yeniDto.Baslik,
                Kategori = yeniDto.Kategori,
                Ozet = yeniDto.Ozet,
                OnemDerecesi = yeniDto.OnemDerecesi ?? "Bilgi",
                OlusturanId = 1,
                CreatedAt = DateTimeOffset.UtcNow,
                YayinTarihi = mevzuatTarihi, 
                DosyaUrl = string.IsNullOrEmpty(yeniDto.KaynakUrl) ? otomatikUrl : yeniDto.KaynakUrl
            };

            _context.Mevzuatlar.Add(dbMevzuat);
            await _context.SaveChangesAsync();

            yeniDto.Id = (int)dbMevzuat.Id;
            yeniDto.KaynakUrl = dbMevzuat.DosyaUrl;
            yeniDto.Tarih = dbMevzuat.YayinTarihi.ToString("dd.MM.yyyy");

            return yeniDto;
        }

        // Dijital arşiv listesi hazırlanıyor.
        public async Task<List<HkDijitalArsivModeli>> GetArsivListesiAsync()
        {
            var dbList = await _context.Arsiv
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            DateTime suAnkiZaman = DateTime.Now;

            return dbList.Select(x => {
            string saklamaSuresiMetni = "Süresiz";
            if (x.YuklemeTarihi.HasValue && x.ImhaTarihi.HasValue)
            {
                int yilFarki = x.ImhaTarihi.Value.Year - x.YuklemeTarihi.Value.Year;
                saklamaSuresiMetni = $"{yilFarki} Yıl ({x.ImhaTarihi.Value.Year})";
            }


            string guncelDurum = x.Durum;

            if (x.Durum == "Arşivlendi" && x.ImhaTarihi.HasValue)
            {
              
                double kalanGunSayisi = (x.ImhaTarihi.Value - suAnkiZaman).TotalDays;

         
            if (kalanGunSayisi > 0 && kalanGunSayisi <= 90)
            {
                guncelDurum = "İmha Yaklaşıyor"; 
            }
            else if (kalanGunSayisi <= 0)
            {
                guncelDurum = "İmha Yaklaşıyor"; 
            }
        }

       
        string temaRenk = guncelDurum switch
        {
            "Arşivlendi" => "green",
            "Tarama Bekliyor" => "orange",
            "İmha Yaklaşıyor" => "red",
            "İmha Edildi" => "gray",
            _ => "gray"
        };

        return new HkDijitalArsivModeli
        {
            Id = (int) x.Id,
            EsasNo = x.DosyaAdi,
            Kategori = x.Kategori,
            DosyaAdi = x.DosyaAdi,
            KapanisTarihi = x.YuklemeTarihi.HasValue ? x.YuklemeTarihi.Value.ToString("dd.MM.yyyy") : "Bugün",
            SaklamaSuresi = saklamaSuresiMetni,
            Durum = guncelDurum, 
            Tema = temaRenk,
            DosyaUrl = x.DosyaUrl,
            ImhaGerekliMi = guncelDurum == "İmha Yaklaşıyor"
        };
}).ToList();
}

   // Dijital arşivdeki bir dosya imha ediliyor.
        public async Task<bool> DosyaImhaEtAsync(int id)
        {
            var dosya = await _context.Arsiv.FirstOrDefaultAsync(x => x.Id == id);
            if (dosya == null) return false;
         
            dosya.Durum = "İmha Edildi";
            _context.Arsiv.Update(dosya);

            await _context.SaveChangesAsync();
            return true;
        }

        // Dijital arşivdeki bir dosyanın durumu güncelleniyor.
        public async Task<bool> ArsivGuncelleAsync(int id, string yeniDurum, string dosyaUrl = null)
        {
            var dosya = await _context.Arsiv.FirstOrDefaultAsync(x => x.Id == id);
            if (dosya == null) return false;

            dosya.Durum = yeniDurum; 

            if (!string.IsNullOrEmpty(dosyaUrl))
            {
                dosya.DosyaUrl = dosyaUrl;
            }

            _context.Arsiv.Update(dosya);
            await _context.SaveChangesAsync();
            return true;
        }

       
        // Dijital arşivdeki bir dosyanın imha süresi uzatılıyor.
        public async Task<bool> SureUzatAsync(int id, string yeniSureMetni)
        {
            var dosya = await _context.Arsiv.FirstOrDefaultAsync(x => x.Id == id);
            if (dosya == null) return false;

        
            int eklenenYil = 10;
            string sadeceYil = new string(yeniSureMetni.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(sadeceYil, out int parsedYil))
            {
                eklenenYil = parsedYil;
            }
            
            dosya.ImhaTarihi = DateTime.Now.AddYears(eklenenYil);
            dosya.Durum = "Arşivlendi";

            _context.Arsiv.Update(dosya);
            await _context.SaveChangesAsync();
            return true;
        }

        // Dijital arşive yeni bir dosya ekleniyor.
        public async Task<HkDijitalArsivModeli> ArsivEkleAsync(HkDijitalArsivModeli model)
        {
            if (model == null) return null;

          
            DateTime kapanisDt = DateTime.Now;
            if (!string.IsNullOrEmpty(model.KapanisTarihi))
            {
                string[] kabulEdilenFormatlar = { "dd.MM.yyyy", "yyyy-MM-dd", "dd/MM/yyyy" };
                DateTime.TryParseExact(model.KapanisTarihi.Trim(), kabulEdilenFormatlar,
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out kapanisDt);
            }

          
            int saklamaYili = 10;
            DateTime? imhaTarihiValue = null; 
            string saklamaSuresiMetni = "Süresiz";

            if (model.Kategori == "Şirketler Hukuku")
            {
                imhaTarihiValue = null; 
                saklamaSuresiMetni = "Süresiz";
            }
            else if (model.Kategori == "İcra İflas")
            {
                saklamaYili = 20;
                imhaTarihiValue = kapanisDt.AddYears(saklamaYili);
                saklamaSuresiMetni = $"20 Yıl ({imhaTarihiValue.Value.Year})";
            }
            else
            {
                saklamaYili = 10; 
                imhaTarihiValue = kapanisDt.AddYears(saklamaYili);
                saklamaSuresiMetni = $"10 Yıl ({imhaTarihiValue.Value.Year})";
            }

            var dbArsiv = new Arsiv
            {
                DosyaAdi = model.EsasNo ?? model.DosyaAdi,
                Kategori = model.Kategori,
                YuklemeTarihi = kapanisDt,
                ImhaTarihi = imhaTarihiValue, 
                Durum = string.IsNullOrEmpty(model.DosyaUrl) ? "Tarama Bekliyor" : "Arşivlendi",
                DosyaUrl = string.IsNullOrEmpty(model.DosyaUrl) ? "docs/varsayilan.pdf" : model.DosyaUrl,
                OlusturanId = 1,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Arsiv.Add(dbArsiv);
            await _context.SaveChangesAsync();

            model.Id = (int)dbArsiv.Id;
            model.Durum = dbArsiv.Durum;
            model.Tema = dbArsiv.Durum == "Arşivlendi" ? "green" : "orange";
            model.SaklamaSuresi = saklamaSuresiMetni;
            model.KapanisTarihi = dbArsiv.YuklemeTarihi.Value.ToString("dd.MM.yyyy");

            return model;
        }
    }
}