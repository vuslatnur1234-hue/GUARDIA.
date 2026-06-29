using Guardia.API.Data;
using Guardia.API.DTOs;
using Guardia.API.DTOs.IK;
using Guardia.API.Models;
using Microsoft.EntityFrameworkCore;


namespace Guardia.API.Services.InsanKaynaklari
{
    public class IkService : IIkService
    {

        private readonly AppDbContext _context;
        public IkService(AppDbContext context)
        {
            _context = context;
        }


       

        // IK Paneli için temel istatistikleri hesaplayan metot
        public async Task<IkPanelIstatistikleri> PanelIstatistikleriniGetirAsync()
        {
            int aktifPersonelSayisi = await _context.Personellers.CountAsync(p => p.Durum == true);

            var bugun = DateOnly.FromDateTime(DateTime.Today);

            int suanIzindeOlanlar = await _context.Izinlers
                .CountAsync(i => i.OnayDurumu == "ONAYLANDI" &&
                                 i.BaslangicTarihi <= bugun &&
                                 i.BitisTarihi >= bugun);

            int fabrikadaAktif = aktifPersonelSayisi - suanIzindeOlanlar;
            int ayrilanSayisi = await _context.Personellers.IgnoreQueryFilters().CountAsync(p => p.Durum == false);
            int toplamKayitHafizasi = await _context.Personellers.IgnoreQueryFilters().CountAsync();

            double turnover = toplamKayitHafizasi > 0
                ? Math.Round(((double)ayrilanSayisi / toplamKayitHafizasi) * 100, 1)
                : 0;

            double memnuniyet = 87.0;

            return new IkPanelIstatistikleri
            {
                ToplamPersonel = aktifPersonelSayisi,
                FabrikadaAktif = fabrikadaAktif,
                MemnuniyetYuzdesi = memnuniyet,
                TurnoverYuzdesi = turnover
            };
        }

        // IK Paneli için son giriş hareketlerini getiren metot
        public async Task<List<IkPersonelGirisModeli>> GirisListesiniGetirAsync()
        {
            var bugun = DateTimeOffset.Now.Date;

            // 1. ADIM: Bugünün gerçek geçiş kayıtlarını veritabanından (SQL) çekiyoruz
            var gecislerDb = await _context.PersonelGecisler
                .Where(g => g.GecisZamani.Date == bugun)
                .OrderByDescending(g => g.GecisZamani)
                .Select(g => new
                {
                    AdSoyad = g.Personel.AdSoyad ?? "",
                    Departman = g.Personel.Departman ?? "",
                    GecisZamani = g.GecisZamani,
                    GecisYonu = g.GecisYonu ?? "GİRİŞ"
                })
                .ToListAsync();

            // 2. ADIM: Çektiğimiz gerçek veriler üzerinde durum ve tema hesaplamasını yapıyoruz
            var modelListesi = gecislerDb.Select(g =>
            {
               
                string hesaplananDurum = GecisDurumuHesapla(g.GecisZamani.DateTime, g.GecisYonu);

                return new IkPersonelGirisModeli
                {
                    AdSoyad = g.AdSoyad,
                    Departman = g.Departman,
                    Saat = g.GecisZamani.ToString("HH:mm") + " " + g.GecisYonu,
                    Durum = hesaplananDurum,
                    Tema = TemaBelirle(hesaplananDurum)
                };
            }).ToList();

            return modelListesi;
        }

     

        private string GecisDurumuHesapla(DateTime gecisZamani, string gecisYonu)
        {
            // Sadece saat ve dakika kısmını al
            TimeSpan saat = gecisZamani.TimeOfDay;

            if (gecisYonu == "GİRİŞ")
            {
                if (saat < new TimeSpan(8, 15, 0)) return "ERKEN";
                else if (saat <= new TimeSpan(8, 30, 0)) return "ZAMANINDA";
                else return "GECİKME";
            }
            else // ÇIKIŞ durumu
            {
                if (saat < new TimeSpan(17, 30, 0)) return "ERKEN";
                else if (saat <= new TimeSpan(18, 0, 0)) return "ZAMANINDA";
                else return "MESAİ";
            }
        }

        private string TemaBelirle(string durum)
        {
            switch (durum)
            {
                case "ZAMANINDA": return "green";
                case "GECİKME": return "red";
                case "MESAİ": return "blue";
                case "ERKEN": return "orange";
                default: return "purple";
            }
        }


        // IK Paneli için son duyuruları ve doğum günlerini getiren metot
        public async Task<List<IkAktiviteModeli>> AktiviteleriGetirAsync(string kategori = "tumu")
        {
            var modelListesi = new List<IkAktiviteModeli>();
            var bugun = DateTime.Today;

         
            string temizKategori = (kategori ?? "tumu").Trim().ToLower();

 
            var query = _context.Duyurulars.AsQueryable();

            if (temizKategori != "tumu")
            {
                query = query.Where(d => d.Kategori != null && d.Kategori.Trim().ToLower() == temizKategori);
            }

            var veritabaniDuyurulari = await query
                .OrderByDescending(d => d.Id)
                .Take(10)
                .ToListAsync();

            foreach (var d in veritabaniDuyurulari)
            {
                string etiket = d.Kategori?.Trim().ToUpper() ?? "DUYURU";
                string tema = "orange"; 

                
                if (etiket.Contains("TEKNİK") || etiket.Contains("GÜVENLİK")) tema = "blue";
                else if (etiket.Contains("EĞİTİM")) tema = "green";
                else if (etiket.Contains("KİŞİSEL") || etiket.Contains("BUGÜN")) tema = "purple";
                else if (etiket.Contains("POLİTİKA")) tema = "green";

                modelListesi.Add(new IkAktiviteModeli
                {
                    Baslik = d.Baslik,
                    Aciklama = d.Icerik,
                    Etiket = etiket,
                    Tema = tema,
                    Kategori = d.Kategori
                });
            }

            if (temizKategori == "tumu" || temizKategori == "kişisel")
            {
                var bugunDoganlar = await _context.Personellers
                    .Where(p => p.Durum == true &&
                                p.DogumTarihi.HasValue &&
                                p.DogumTarihi.Value.Month == bugun.Month &&
                                p.DogumTarihi.Value.Day == bugun.Day)
                    .ToListAsync();

                foreach (var p in bugunDoganlar)
                {
                    modelListesi.Insert(0, new IkAktiviteModeli
                    {
                        Baslik = p.AdSoyad,
                        Aciklama = "Bugün Doğum Günü! 🎂",
                        Etiket = "BUGÜN",
                        Tema = "purple",
                        Kategori = "Kişisel"
                    });
                }
            }

            return modelListesi;
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          

        // IK Paneli için departman dağılımını getiren metot
        public async Task<List<IkDepartmanDagilimModeli>> DepartmanDagiliminiGetirAsync(string tip)
        {
         
            var rawData = await _context.Personellers
                .GroupBy(p => p.Unvan)
                .Select(g => new
                {
                    Unvan = g.Key ?? "Belirtilmemiş",
                    Sayi = g.Count()
                })
                .ToListAsync(); 

           
            var dagilim = rawData.Select(x => new IkDepartmanDagilimModeli
            {
                DepartmanAdi = x.Unvan,
                KisiSayisi = x.Sayi,
                Tip = BelirleDepartmanTipi(x.Unvan) 
            }).ToList();

          
            if (tip == "tumu" || string.IsNullOrEmpty(tip))
                return dagilim;

            return dagilim.Where(d => d.Tip == tip).ToList();
        }
  

        // Verilen departman adına göre "saha", "teknik" veya "idari" tipini belirleyen yardımcı metot
        private string BelirleDepartmanTipi(string departmanAdi)
        {
            if (string.IsNullOrEmpty(departmanAdi)) return "diger";

            string ad = departmanAdi.ToLower();

            if (ad.Contains("üretim") || ad.Contains("lojistik") || ad.Contains("güvenlik") || ad.Contains("temizlik") || ad.Contains("formen"))
                return "saha";

            if (ad.Contains("bilgi işlem") || ad.Contains("bakım") || ad.Contains("kalite") || ad.Contains("sağlığı"))
                return "teknik";

            return "idari";
        }

        // 1. Gelen Mesajları Veritabanından Çekme
        public async Task<List<MesajModeli>> GelenMesajlariGetirAsync(string aktifBirim)
        {
            return await _context.Mesajlars // 🛠️ Güncellendi
                .Where(x => x.AliciBirim == aktifBirim)
                .OrderByDescending(x => x.Id)
                .Select(x => new MesajModeli
                {
                    Id = x.Id,
                    Birim = x.GonderenBirim,
                    Mesaj = x.MesajIcerigi,
                    Saat = x.GonderimSaati,
                    OkunduMu = x.OkunduMu
                }).ToListAsync();
        }

        // 2. Giden Mesajları Veritabanından Çekme
     
        public async Task<List<MesajModeli>> GidenMesajlariGetirAsync(string aktifBirim)
        {
            return await _context.Mesajlars
                .Where(x => x.GonderenBirim == aktifBirim)
                .OrderByDescending(x => x.Id)
                .Select(x => new MesajModeli
                {
                    Id = x.Id,
                    Birim = x.AliciBirim,
                    Mesaj = x.MesajIcerigi,
                    Saat = x.GonderimSaati,
                    OkunduMu = x.OkunduMu
                }).ToListAsync();
        }

        // 3. Birimler Arası Yeni Not / Cevap Ekleme
    
        public async Task<bool> MesajGonderAsync(string gonderenBirim, MesajModeli model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Mesaj) || string.IsNullOrWhiteSpace(model.Birim))
                return false;

            string suAnkiSaat = DateTime.Now.ToString("HH:mm");

       
            var hedefBirimler = model.Birim.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var alici in hedefBirimler)
            {
                var yeniMesaj = new Mesajlar
                {
                    GonderenBirim = gonderenBirim,
                    AliciBirim = alici.Trim().ToUpper(), 
                    MesajIcerigi = model.Mesaj,
                    GonderimSaati = suAnkiSaat,
                    OkunduMu = false,
                    CreatedAt = DateTimeOffset.Now
                };
                _context.Mesajlars.Add(yeniMesaj);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        // IK Paneli için memnuniyet analiz verilerini getiren metot
        public IkMemnuniyetAnalizModeli MemnuniyetAnaliziGetir()
        {
            return new IkMemnuniyetAnalizModeli
            {
                Hijyen = 91,
                Yemekhane = 90,
                Iletisim = 75,
                SosyalHaklar = 67,
                Katilim = 94,
                YanitSayisi = 1160,
                AylikTrend = 1.5
            };
        }

        // IK Paneli için turnover analiz verilerini getiren metot
        public async Task<IkTurnoverAnalizModeli> TurnoverAnaliziGetirAsync()
        {
            
            int toplamAyrilan = await _context.Personellers.IgnoreQueryFilters().CountAsync(p => p.Durum == false);
            int toplamKayit = await _context.Personellers.IgnoreQueryFilters().CountAsync();

           
            double yillikOran = toplamKayit > 0 ? Math.Round(((double)toplamAyrilan / toplamKayit) * 100, 1) : 0;

            var riskliGrup = await _context.Personellers.IgnoreQueryFilters()
                .Where(p => p.Durum == false)
                .GroupBy(p => p.Unvan)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Ad = g.Key, Sayi = g.Count() })
                .FirstOrDefaultAsync();

            return new IkTurnoverAnalizModeli
            {
                ToplamAyrilan = toplamAyrilan, 
                YillikOran = yillikOran,
                SektorOrtalamasi = 4.5,
                RiskliDepartman = riskliGrup?.Ad ?? "Belirlenmedi",
                RiskOrani = riskliGrup != null ? Math.Round(((double)riskliGrup.Sayi / toplamAyrilan) * 100, 1) : 0,
                IstenCikarilma = 42,
                BaskaIsTeklifi = 15,
                SehirDegisikligi = 15,
                EmeklilikSaglik = 28
            };
        }


        // IK Paneli için saha aktiflik verilerini getiren metot
        public async Task<List<IkSahaAktiflikModeli>> GetSahaAktiflikAsync()
        {
            var gruplar = await _context.Personellers
                .Where(p => p.Durum == true && p.VardiyaGrubu != null)
                .GroupBy(p => p.VardiyaGrubu)
                .Select(g => new
                {
                    GrupAdi = g.Key,
                    Sayi = g.Count()
                })
                .ToListAsync();

            var sahaListesi = new List<IkSahaAktiflikModeli>();

            foreach (var grup in gruplar)
            {
                sahaListesi.Add(new IkSahaAktiflikModeli
                {
                    BirimAdi = grup.GrupAdi + " Operasyon Merkezi",
                    AltBilgi = "Aktif Çalışma Grubu",
                    MevcutPersonel = grup.Sayi,
                    ToplamKapasite = 200, 
                    MetrikAdi = "Bölge Doluluğu",
                    MetrikYuzde = (int)((double)grup.Sayi / 200 * 100),
                    DurumEtiketi = "AKTİF",
                    Tema = "blue"
                });
            }
            if (!sahaListesi.Any())
            {
                sahaListesi.Add(new IkSahaAktiflikModeli { BirimAdi = "Genel Saha", MevcutPersonel = 0, ToplamKapasite = 100, DurumEtiketi = "BEKLEMEDE", Tema = "green" });
            }

            return sahaListesi;
        }


        // IK Paneli için personel detaylarını getiren metot
        public async Task<List<IkPersonelDetayModeli>> PersonelListesiniGetirAsync()
        {
            var personeller = await _context.Personellers.ToListAsync();
            return personeller.Select(p => new IkPersonelDetayModeli
            {
                FotoUrl = p.AvatarUrl ?? "img/default-avatar.png",
                AdSoyad = p.AdSoyad,
                SicilNo = p.SicilNo,
                Pozisyon = p.Unvan,
                Departman = p.VardiyaGrubu,
                Durum = p.Durum == true ? "Aktif" : "Ayrıldı"
            }).ToList();
        }

        // IK Paneli için belirli bir personelin detaylarını getiren metot
        public async Task<IkPersonelDetayModeli> PersonelDetayiniGetirAsync(string sicilNo)
        {
            var p = await _context.Personellers
                .Include(x => x.Maaslars)
                .FirstOrDefaultAsync(x => x.SicilNo == sicilNo);

            if (p == null) return null;

            var aktifMaasKaydi = p.Maaslars.FirstOrDefault(m => m.Donem == "Mayıs 2026");

            return new IkPersonelDetayModeli
            {
                AdSoyad = p.AdSoyad,
                SicilNo = p.SicilNo,
                FotoUrl = p.AvatarUrl ?? "img/default-avatar.png",
                TcKimlik = p.TcNo ?? "12345678901", 
                DogumTarihi = p.DogumTarihi?.ToString("dd.MM.yyyy") ?? p.CreatedAt.ToString("dd.MM.yyyy"),
                KanGrubu = p.KanGrubu ?? "A Rh+",
                MedeniDurum = p.MedeniDurum ?? "Bekar",
                AskerlikDurumu = p.AskerlikDurumu ?? "Seçiniz",
                Telefon = p.Telefon,
                Eposta = p.Email,
                Adres = p.Adres,
                YakinAdiSoyadi = p.AcilDurumYakini,
                YakinlikDerecesi = p.YakinlikDerecesi ?? "Eşi", 
                YakinTelefon = p.AcilDurumNo,
                Maas = (aktifMaasKaydi != null && aktifMaasKaydi.NetMaas != null)
    ? Convert.ToDecimal(aktifMaasKaydi.NetMaas).ToString("N2") + " ₺"
    : "42.500,00 ₺",
                Iban = p.Iban ?? "TR68 0000 0000 0000 0000 0000 00",
                BankaAdi = p.BankaAdi ?? "Garanti BBVA",
                HesapNumarasi = p.HesapNumarasi ?? "12345678"
            };
        }


        // IK Paneli için yeni personel kaydı ekleyen metot

        public void YeniPersonelKaydet(IkPersonelEkleModeli model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var enSonSicil = _context.Personellers
                    .IgnoreQueryFilters()
                    .Where(p => p.SicilNo != null && p.SicilNo.StartsWith("2024"))
                    .OrderByDescending(p => p.SicilNo)
                    .Select(p => p.SicilNo)
                    .FirstOrDefault();

                int yeniSicilNoRakam = 202401;

                if (enSonSicil != null && int.TryParse(enSonSicil, out int sonDeger))
                {
                    yeniSicilNoRakam = sonDeger + 1;
                }

                string yeniSicilNo = yeniSicilNoRakam.ToString();

              
                var yeniPersonel = new Personeller
                {
                    AdSoyad = model.AdSoyad,
                    Unvan = model.GorevUnvan ?? "Kadro Personeli",
                    Departman = string.IsNullOrEmpty(model.Departman) ? "Genel Kadro" : model.Departman,
                    VardiyaGrubu = string.IsNullOrEmpty(model.Departman) ? "Genel Kadro" : model.Departman,
                    Email = model.Eposta ?? "personel@guardia.com",
                    Telefon = model.Telefon ?? "0500 000 00 00",
                    Adres = model.Adres ?? "Belirtilmemiş",
                    KanGrubu = model.KanGrubu ?? "A Rh+",
                    AcilDurumYakini = model.YakinAdiSoyadi ?? "Yakını",
                    AcilDurumNo = model.YakinTelefon ?? "0500 000 00 00",
                    MedeniDurum = model.MedeniDurum ?? "Bekar",
                    YakinlikDerecesi = model.YakinlikDerecesi ?? "Belirtilmemiş",
                    TcNo = model.TcKimlikNo ?? "12345678901",
                    AskerlikDurumu = model.AskerlikDurumu ?? "Seçiniz",
                    Iban = model.Iban ?? "TR00 0000 0000 0000 0000 0000 00",
                    BankaAdi = model.BankaAdi ?? "Belirtilmemiş",
                    HesapNumarasi = model.HesapNumarasi ?? "00000000",
                    DogumTarihi = DateTime.TryParse(model.DogumTarihi, out var dt) ? dt : null,

                    SicilNo = yeniSicilNo,
                    AvatarUrl = "img/default-avatar.png",
                    IkGuncellemeDurumu = "ONAYLANDI",
                    Durum = true,
                    IseGirisTarihi = DateOnly.FromDateTime(DateTime.Today),
                    IzinBakiyesi = 15,

                  
                    Sifre = "123456",

                    QrKodData = "GUARDIA-" + yeniSicilNo,
                    CreatedAt = DateTime.Now
                };

                _context.Personellers.Add(yeniPersonel);
                _context.SaveChanges(); 

                DateTime bugun = DateTime.Now;
                string dinamikDonem = bugun.ToString("MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
                int ayinSonGunu = DateTime.DaysInMonth(bugun.Year, bugun.Month);
                DateOnly dinamikOdemeTarihi = new DateOnly(bugun.Year, bugun.Month, ayinSonGunu);

                string temizMaas = model.Maas?.Replace(".", "").Replace(",", "").Replace(" ₺", "").Replace("-", "").Trim();

                if (!decimal.TryParse(temizMaas, out decimal girilenMaas) || girilenMaas == 0)
                {
                    string secilenDepartman = model.Departman ?? "";

                    if (secilenDepartman.Contains("Bilgi İşlem") || secilenDepartman.Contains("Yazılım"))
                        girilenMaas = 65000.00m;
                    else if (secilenDepartman.Contains("Finans") || secilenDepartman.Contains("Muhasebe"))
                        girilenMaas = 55000.00m;
                    else if (secilenDepartman.Contains("İnsan Kaynakları"))
                        girilenMaas = 45000.00m;
                    else if (secilenDepartman.Contains("İş Sağlığı") || secilenDepartman.Contains("Güvenliği"))
                        girilenMaas = 48000.00m;
                    else if (secilenDepartman.Contains("Satın Alma"))
                        girilenMaas = 42000.00m;
                    else if (secilenDepartman.Contains("Lojistik") || secilenDepartman.Contains("Depo"))
                        girilenMaas = 35000.00m;
                    else if (secilenDepartman.Contains("Üretim") || secilenDepartman.Contains("Mavi Yaka"))
                        girilenMaas = 30000.00m;
                    else
                        girilenMaas = 35000.00m;
                }

                var personelMaasKaydi = new Maaslar
                {
                    PersonelId = yeniPersonel.Id,
                    Donem = dinamikDonem,
                    NetMaas = girilenMaas,
                    Prim = 0m,
                    SgkPayi = Math.Round((girilenMaas * 1.4m) * 0.14m, 2),
                    GelirVergisi = Math.Round((girilenMaas * 1.4m) * 0.15m, 2),
                    Kesinti = Math.Round(((girilenMaas * 1.4m) * 0.14m) + ((girilenMaas * 1.4m) * 0.15m), 2),
                    Durum = "Beklemede",
                    OdemeTarihi = dinamikOdemeTarihi,
                    CreatedAt = DateTime.Now
                };

                _context.Maaslars.Add(personelMaasKaydi);
                _context.SaveChanges();



                var yeniGirisHesabi = new PersonelGirisBilgileri 
                {
                    PersonelId = yeniPersonel.Id,
                    SicilNo = yeniSicilNo,
                    Sifre = "123456",
                    DogrulamaKodu = null,
                    KodSonKullanma = null,
                    HataliDenemeSayisi = 0,
                    HesapKilitliMi = false
                    
                };

                _context.PersonelGirisBilgileris.Add(yeniGirisHesabi);
                _context.SaveChanges();


           
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                string gercekHata = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"VERİTABANI HATASI: {gercekHata}");
            }
        }

        // IK Paneli için personel kaydını silen metot (durumu pasif yapar)
        public async Task<bool> PersonelSilAsync(string sicilNo)
        {
            var personel = await _context.Personellers.FirstOrDefaultAsync(p => p.SicilNo == sicilNo);

            if (personel != null)
            {
                personel.Durum = false;
                personel.AyrilmaTarihi = DateTime.Now; 

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // IK Paneli için personel kaydını güncelleyen metot
        public bool PersonelGuncelle(string sicilNo, IkPersonelEkleModeli model)
        { 
            var personel = _context.Personellers
                .Include(p => p.Maaslars)
                .FirstOrDefault(p => p.SicilNo == sicilNo);

            if (personel != null)
            {
             
                personel.AdSoyad = model.AdSoyad;
                personel.Unvan = model.GorevUnvan;
                personel.Departman = model.Departman;
                personel.TcNo = model.TcKimlikNo;
                personel.KanGrubu = model.KanGrubu;
                personel.MedeniDurum = model.MedeniDurum;
                personel.AskerlikDurumu = model.AskerlikDurumu;
                personel.Telefon = model.Telefon;
                personel.Email = model.Eposta;
                personel.Adres = model.Adres;
                personel.AcilDurumYakini = model.YakinAdiSoyadi;
                personel.YakinlikDerecesi = model.YakinlikDerecesi;
                personel.AcilDurumNo = model.YakinTelefon;
                personel.Iban = model.Iban;
                personel.BankaAdi = model.BankaAdi;
                personel.HesapNumarasi = model.HesapNumarasi;

                if (decimal.TryParse(model.Maas?.Replace(".", "").Replace(",", "").Replace(" ₺", ""), out decimal girilenMaas))
                {
                    var mevcutMaas = personel.Maaslars.FirstOrDefault(m => m.Donem == "Mayıs 2026");
                    if (mevcutMaas != null)
                    {
                        mevcutMaas.NetMaas = girilenMaas;
                        mevcutMaas.SgkPayi = Math.Round((girilenMaas * 1.4m) * 0.14m, 2);
                        mevcutMaas.GelirVergisi = Math.Round((girilenMaas * 1.4m) * 0.15m, 2);
                        mevcutMaas.Kesinti = Math.Round(((girilenMaas * 1.4m) * 0.14m) + ((girilenMaas * 1.4m) * 0.15m), 2);
                    }
                }

                _context.SaveChanges();
                return true;
            }

            return false;
        }


    // IK Paneli için izin talebini onaylayan metot
        public async Task<bool> IzinOnaylaAsync(int talepId)
        {
            var izin = await _context.Izinlers.FindAsync((long)talepId);
            if (izin == null) return false;

            izin.OnayDurumu = "ONAYLANDI";
        
            await _context.SaveChangesAsync();
            return true;
        }


        // IK Paneli için izin talebini reddeden metot
        public async Task<bool> IzinReddetAsync(int talepId, string neden)
        {
            var izin = await _context.Izinlers.FindAsync((long)talepId);
            if (izin == null) return false;

            izin.OnayDurumu = "REDDEDİLDİ";
            izin.Aciklama = neden;

            await _context.SaveChangesAsync();
            return true;
        }
      
        // IK Paneli için izin istatistiklerini getiren metot
        public async Task<IkIzinIstatistikModeli> IzinIstatistikleriniGetirAsync()
        {
            var bugun = DateOnly.FromDateTime(DateTime.Today);
            var tumIzinler = await _context.Izinlers.ToListAsync();

            return new IkIzinIstatistikModeli
            {
                BekleyenCount = tumIzinler.Count(x => x.OnayDurumu == "BEKLEMEDE"),
                IzindeCount = tumIzinler.Count(x =>
                    x.OnayDurumu == "ONAYLANDI" &&
                    x.BaslangicTarihi <= bugun &&
                    x.BitisTarihi >= bugun),
                GelecekCount = tumIzinler.Count(x =>
                    x.OnayDurumu == "ONAYLANDI" &&
                    x.BaslangicTarihi > bugun)
            };
        }


     // IK Paneli için izin taleplerini getiren metot
        public async Task<List<IkIzinTalepModeli>> IzinListesiniGetirAsync()
        {
            var izinler = await _context.Izinlers
                .Include(i => i.Personel)
                .Where(i => i.Personel != null && i.Personel.Durum == true)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return izinler.Select(i => new IkIzinTalepModeli
            {
                Id = (int)i.Id,
                PersonelAd = i.Personel?.AdSoyad ?? "Bilinmeyen Personel",
                SicilNo = i.Personel?.SicilNo ?? "#000000",
                Departman = i.Personel?.Unvan ?? "Genel Müdürlük",
                IzinTuru = i.IzinTuru,
                BaslangicTarihi = i.BaslangicTarihi?.ToString("dd.MM.yyyy") ?? "",
                BitisTarihi = i.BitisTarihi?.ToString("dd.MM.yyyy") ?? "",
                TarihAraligi = i.BaslangicTarihi.HasValue ? $"{i.BaslangicTarihi:dd MMM} - {i.BitisTarihi:dd MMM}" : "",
                Durum = (i.OnayDurumu == "BEKLEMEDE" || i.OnayDurumu == "0") ? "Bekliyor" : i.OnayDurumu,

                Sure = (i.BitisTarihi.HasValue && i.BaslangicTarihi.HasValue)
                        ? (i.BitisTarihi.Value.DayNumber - i.BaslangicTarihi.Value.DayNumber)
                        : 0,

                KalanGun = (int)(i.Personel?.IzinBakiyesi ?? 0)
            }).ToList();
        }

        /*     // Veritabanı yerine geçici bir liste 
             private static List<IkBordroListeModel> _geciciBordroListesi = new List<IkBordroListeModel>
     {
         new IkBordroListeModel { Id = 1, AdSoyad = "Esma Çelik", SicilNo = "#202401", BrutMaas = 45000, Durum = "Ödendi", Departman = "Yazılım" },
         new IkBordroListeModel { Id = 2, AdSoyad = "Ahmet Demir", SicilNo = "#202402", BrutMaas = 32000, Durum = "Beklemede", Departman = "Üretim" },
         new IkBordroListeModel { Id = 3, AdSoyad = "Zeynep Kaya", SicilNo = "#202388", BrutMaas = 38500, Durum = "Beklemede", Departman = "Kalite Kontrol" },
         new IkBordroListeModel { Id = 4, AdSoyad = "Can Öztürk", SicilNo = "#202301", BrutMaas = 29000, Durum = "Ödendi", Departman = "Lojistik" },
         new IkBordroListeModel { Id = 5, AdSoyad = "Elif Şahin", SicilNo = "#202199", BrutMaas = 41200, Durum = "Beklemede", Departman = "İdari İşler" },
         new IkBordroListeModel { Id = 6, AdSoyad = "Burak Aydın", SicilNo = "#202077", BrutMaas = 35750, Durum = "Ödendi", Departman = "Bakım" },
         new IkBordroListeModel { Id = 7, AdSoyad = "Merve Koç", SicilNo = "#202415", BrutMaas = 33000, Durum = "Beklemede", Departman = "Pazarlama" },
         new IkBordroListeModel { Id = 8, AdSoyad = "Onur Yıldız", SicilNo = "#202350", BrutMaas = 48000, Durum = "Beklemede", Departman = "Yazılım" }
     };*/


        


        // IK Paneli için bordro listesini getiren metot

        public async Task<List<IkBordroListeModel>> GetBordroListesiAsync()
        {
            var bordroVerileri = await _context.Maaslars
                .Include(m => m.Personel)
                .Where(m => m.Personel.Durum == true)
                .AsNoTracking()
                .ToListAsync();

            var tekilBordrolar = bordroVerileri
                .GroupBy(m => m.PersonelId)
                .Select(g => g.First())
                .ToList();

            var modelListesi = tekilBordrolar.Select(m => {
                decimal netGelen = m.NetMaas ?? 0m;
                decimal dbSgkPayi = m.SgkPayi ?? 0m;
                decimal dbGelirVergisi = m.GelirVergisi ?? 0m;
                decimal brutMaas = Math.Round(netGelen * 1.4m, 2);
                decimal vergiOrani = brutMaas <= 35000 ? 0.15m : brutMaas <= 70000 ? 0.20m : 0.27m;

                return new IkBordroListeModel
                {
                    Id = (int)m.Id,
                    PersonelId = (int)m.PersonelId,
                    AdSoyad = m.Personel?.AdSoyad ?? "Bilinmiyor",
                    SicilNo = m.Personel?.SicilNo ?? "-",
                    Departman = m.Personel?.Unvan ?? "-",
                    BrutMaas = brutMaas,
                    SgkPayi = dbSgkPayi,           
                    GelirVergisi = dbGelirVergisi,  
                    Kesintiler = dbSgkPayi + dbGelirVergisi, 
                    NetMaas = netGelen,
                    VergiOraniGosterge = (int)(vergiOrani * 100),
                    Donem = m.Donem ?? "Mayıs 2026",
                    Durum = m.Durum ?? "Beklemede"
                };
            }).ToList();

            return modelListesi;
        }
 
        // IK Paneli için bordro özetini getiren metot
        public async Task<IkBordroOzetModel> GetBordroOzetAsync()
        {
           
            var maasSorgu = _context.Maaslars.Select(x => new
            {
                NetMaas = (decimal?)x.NetMaas ?? 0m,
                Prim = (decimal?)x.Prim ?? 0m,
                SgkPayi = (decimal?)x.SgkPayi ?? 0m,
                GelirVergisi = (decimal?)x.GelirVergisi ?? 0m,
                Durum = x.Durum ?? "Beklemede"
            });

            var maasVerileri = await maasSorgu.ToListAsync();

            decimal toplamNet = maasVerileri.Sum(x => x.NetMaas);
            decimal toplamPrim = maasVerileri.Sum(x => x.Prim);
            decimal sgkIsci = maasVerileri.Sum(x => x.SgkPayi);
            decimal gelirVergisi = maasVerileri.Sum(x => x.GelirVergisi);
            decimal toplamKesinti = sgkIsci + gelirVergisi;
            decimal brutToplam = maasVerileri.Sum(x => x.NetMaas * 1.4m);
            decimal sgkIsveren = brutToplam * 0.205m;
            decimal damgaVergisi = brutToplam * 0.00759m;
            decimal netOdenen = maasVerileri
                .Where(x => x.Durum == "ÖDENDİ" || x.Durum == "Ödendi")
                .Sum(x => x.NetMaas);

            int bekleyenSayisi = maasVerileri.Count(x => x.Durum == "ÖDENMEDİ" || x.Durum == "Beklemede");

            return new IkBordroOzetModel
            {
                BrutToplam = Math.Round(brutToplam, 2),
                NetOdenen = Math.Round(netOdenen, 2),
                BekleyenCount = bekleyenSayisi,

                BankaMaasOdemeleri = Math.Round(toplamNet * 0.85m, 2),
                IkramiyeBonusOdemeleri = Math.Round(toplamPrim, 2),
                YanHaklarYolYemek = Math.Round(toplamNet * 0.15m, 2),

                SgkIsverenToplami = Math.Round(sgkIsveren, 2),
                SgkIsciToplami = Math.Round(sgkIsci, 2),
                GelirVergisiToplami = Math.Round(gelirVergisi, 2),
                DamgaVergisiToplami = Math.Round(damgaVergisi, 2),
                ToplamYasalYukumluluk = Math.Round(sgkIsveren + sgkIsci + gelirVergisi + damgaVergisi, 2)
            };
        }


        // IK Paneli için bordro durumunu güncelleyen metot
        public async Task<bool> BordroDurumGuncelleAsync(int id, string yeniDurum)
        {
        
            var maasKaydi = await _context.Maaslars.FirstOrDefaultAsync(x => x.Id == (long)id);

            if (maasKaydi == null) return false;
            maasKaydi.Durum = yeniDurum;

            await _context.SaveChangesAsync();

            return true;
        }

        // IK Paneli için toplu ödeme emri gönderen metot
        public async Task<bool> TopluOdemeEmriGonderAsync()
        {
            var onaylananlar = await _context.Maaslars
                .Where(x => x.Durum == "Onaylandı")
                .ToListAsync();

            if (!onaylananlar.Any()) return false;

            foreach (var maas in onaylananlar)
            {
                maas.Durum = "Ödendi";
               
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // IK Paneli için tüm bekleyen ödemeleri onaylayan metot
        public async Task<bool> HepsiniOnaylaAsync()
        {
            var bekleyenler = await _context.Maaslars
                .Where(x => x.Durum == "Beklemede" || x.Durum == "ÖDENMEDİ")
                .ToListAsync();

            if (!bekleyenler.Any()) return false;

            foreach (var maas in bekleyenler)
            {
                maas.Durum = "Onaylandı";
            }

            await _context.SaveChangesAsync();

            return true;
        }



        private static List<IkDuyuruModeli> _duyuruListesi = new List<IkDuyuruModeli>
        {
            new IkDuyuruModeli { Id = 1, Baslik = "Maaş Ödemeleri Hakkında", DuyuruIcerigi = "Banka sistem güncellemesi nedeniyle ödemeler yarın yapılacaktır.", HedefKitle = "Tüm Personel", Yayinlayan = "Selin Aksoy", YayinTarihi = DateTime.Now.AddDays(-2), OkunduOnay = "Onaylandı" },
            new IkDuyuruModeli { Id = 2, Baslik = "Yeni Ekip Arkadaşımız!", DuyuruIcerigi = "Ekibimize yeni katılan arkadaşımıza hoş geldin diyoruz.", HedefKitle = "Üretim Birimi", Yayinlayan = "Selin Aksoy", YayinTarihi = DateTime.Now.AddDays(-1), OkunduOnay = "Yayında" }
        };

        // IK Paneli için son duyuruları getiren metot
        public List<IkDuyuruModeli> SonDuyurulariGetir()
        {
           
            return _duyuruListesi.OrderByDescending(x => x.YayinTarihi).ToList();
        }

  

        // IK Paneli için personele göre duyuruları getiren metot
        public async Task<List<IkDuyuruModeli>> PersoneleGoreDuyuruGetir(string dept)
        {
  
            var duyurular = await _context.Duyurulars
                .OrderByDescending(x => x.Tarih)
                .ToListAsync();

     
            return duyurular.Select(d => new IkDuyuruModeli
            {
                Baslik = d.Baslik,
                Icerik = d.Icerik,
                Kategori = d.Kategori,
                YayinTarihi = d.Tarih?.DateTime ?? DateTime.Now,
                HedefKitle = d.HedefKitle,
                Yayinlayan = "Selin Aksoy" 
            }).ToList();
        }

        // IK Paneli için yeni duyuru kaydeden metot
        public async Task<bool> DuyuruKaydet(IkDuyuruModeli model)
        {
            var yeniDuyuru = new Duyurular
            {
                Baslik = model.Baslik,
                Icerik = model.Icerik, 
                Kategori = model.Kategori,
                HedefKitle = model.HedefKitle,
                Tarih = DateTime.Now,
                YayinlayanId = 1, 
                CreatedAt = DateTime.Now
            };

            _context.Duyurulars.Add(yeniDuyuru);
            await _context.SaveChangesAsync();
            return true;
        }


        public void GuncellemeTalebiOlustur(string sicilNo, Dictionary<string, string> veriler)
        {
            var personel = _context.Personellers.FirstOrDefault(p => p.SicilNo == sicilNo);
            if (personel != null)
            {
                if (veriler.ContainsKey("telefon"))
                    personel.YeniTelefonTalebi = veriler["telefon"];

                string adresTalebi = veriler.ContainsKey("adres") ? veriler["adres"] : "";

                if (veriler.ContainsKey("yakinAdiSoyadi"))
                {
                    adresTalebi += " | YAKIN: " + veriler["yakinAdiSoyadi"];
                }

                personel.YeniAdresTalebi = adresTalebi;
                personel.IkGuncellemeDurumu = "BEKLEMEDE";

                _context.SaveChanges();
            }
        }


        // TumBekleyenVerileriGetir ve BekleyenVeriyiGetir içinde parse mantığını güncelle:

        private static (string adres, string yakin, string yakinTel) AdresParcala(string ham)
        {
            string adres = ham ?? "";
            string yakin = "";
            string yakinTel = "";

            if (adres.Contains(" | YAKIN: "))
            {
                var parts = adres.Split(new[] { " | YAKIN: " }, StringSplitOptions.None);
                adres = parts[0];
                string geri = parts.Length > 1 ? parts[1] : "";

                if (geri.Contains(" | YAKIN_TEL: "))
                {
                    var telParts = geri.Split(new[] { " | YAKIN_TEL: " }, StringSplitOptions.None);
                    yakin = telParts[0];
                    yakinTel = telParts.Length > 1 ? telParts[1] : "";
                }
                else
                {
                    yakin = geri;
                }
            }

            return (adres, yakin, yakinTel);
        }

        public Dictionary<string, object> TumBekleyenVerileriGetir()
        {
            var bekleyenler = _context.Personellers
                .Where(p => p.IkGuncellemeDurumu == "BEKLEMEDE")
                .ToList();

            var sonuc = new Dictionary<string, object>();

            foreach (var p in bekleyenler)
            {
                var (adres, yakin, yakinTel) = AdresParcala(p.YeniAdresTalebi ?? "");

                sonuc.Add(p.SicilNo, new
                {
                    adSoyad = p.AdSoyad,           // IK modalında isim göstermek için
                    telefon = p.YeniTelefonTalebi,
                    adres = adres,
                    yakinAdiSoyadi = yakin,
                    yakinTelefon = yakinTel
                });
            }

            return sonuc;
        }

        // 3. IK Paneli için tek bir personelin onay bekleyen verisini getiren metot (VERİTABANINDAN OKUR)
        public object BekleyenVeriyiGetir(string sicilNo)
        {
            var p = _context.Personellers.FirstOrDefault(x => x.SicilNo == sicilNo && x.IkGuncellemeDurumu == "BEKLEMEDE");
            if (p == null) return null;

            string adres = p.YeniAdresTalebi ?? "";
            string yakin = "";

            if (adres.Contains(" | YAKIN: "))
            {
                var parts = adres.Split(new[] { " | YAKIN: " }, StringSplitOptions.None);
                adres = parts[0];
                yakin = parts.Length > 1 ? parts[1] : "";
            }

            return new
            {
                telefon = p.YeniTelefonTalebi,
                adres = adres,
                yakinAdiSoyadi = yakin
            };
        }

        // 4. IK Paneli için bekleyen talebi silen metot (VERİTABANINI GÜNCELLER)
        public void BekleyenTalebiSil(string sicilNo)
        {
            var p = _context.Personellers.FirstOrDefault(x => x.SicilNo == sicilNo);
            if (p != null)
            {
                p.IkGuncellemeDurumu = "ONAYLANDI"; // Durumu temize çekiyoruz
                p.YeniTelefonTalebi = null; // Talepleri temizliyoruz
                p.YeniAdresTalebi = null;

                _context.SaveChanges();
            }
        }



    }




}
