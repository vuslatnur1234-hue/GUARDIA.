using Guardia.API.DTOs;
using Guardia.API.DTOs.IK;
using Guardia.API.Services.InsanKaynaklari;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guardia.Tests
{
    // internal yerine public yaptık, hatasız implementasyon sağlandı.
    public class IkTestService : IIkTestService
    {
        // ─────────────────────────────────────────────────────────────────
        // BELLEK İÇİ (IN-MEMORY) VERİ DEPOLARI (STATIC LISTS)
        // ─────────────────────────────────────────────────────────────────

        private static Dictionary<string, object> _onayBekleyenVeriler = new Dictionary<string, object>();

        private static List<IkPersonelDetayModeli> _personeller = new List<IkPersonelDetayModeli>
        {
            new() { FotoUrl = "img/ayse_nur.jpg", AdSoyad = "Ayşenur Çelik", SicilNo = "202401", Pozisyon = "Kıdemli Uzman", Departman = "İnsan Kaynakları", Durum = "Aktif" },
            new() { FotoUrl = "img/mehmet_kaya.jpg", AdSoyad = "Mehmet Kaya", SicilNo = "202402", Pozisyon = "Üretim Mühendisi", Departman = "Mavi Yaka Üretim", Durum = "Aktif" },
            new() { FotoUrl = "img/zeynep_arslan.jpg", AdSoyad = "Zeynep Arslan", SicilNo = "202403", Pozisyon = "Finans Uzmanı", Departman = "Finans ve Muhasebe", Durum = "Aktif" },
            new() { FotoUrl = "img/can_ozdemir.jpg", AdSoyad = "Can Özdemir", SicilNo = "202404", Pozisyon = "Yazılım Geliştirici", Departman = "Bilgi İşlem", Durum = "Aktif" },
            new() { FotoUrl = "img/elif_sahin.jpg", AdSoyad = "Elif Şahin", SicilNo = "202405", Pozisyon = "Satın Alma Uzmanı", Departman = "Satın Alma", Durum = "Aktif" },
            new() { FotoUrl = "img/burak_yildiz.jpg", AdSoyad = "Burak Yıldız", SicilNo = "202406", Pozisyon = "Depo Sorumlusu", Departman = "İş Sağlığı ve Güvenliği", Durum = "Aktif" }
        };

        private static List<IkIzinTalepModeli> _izinTalepleri = new List<IkIzinTalepModeli>
        {
            new IkIzinTalepModeli { Id=1, PersonelAd="Ahmet Demir", IzinTuru="Yıllık", BaslangicTarihi="10.06.2026", BitisTarihi="14.06.2026", Durum="Bekliyor" },
            new IkIzinTalepModeli { Id=2, PersonelAd="Zeynep Kaya", IzinTuru="Mazeret", BaslangicTarihi="05.06.2026", BitisTarihi="05.06.2026", Durum="Bekliyor" },
        };

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
        };

        private static List<IkDuyuruModeli> _duyuruListesi = new List<IkDuyuruModeli>
        {
            new IkDuyuruModeli { Id = 1, Baslik = "Maaş Ödemeleri Hakkında", DuyuruIcerigi = "Banka sistem güncellemesi nedeniyle ödemeler yarın yapılacaktır.", HedefKitle = "Tüm Personel", Yayinlayan = "Selin Aksoy", YayinTarihi = DateTime.Now.AddDays(-2), OkunduOnay = "Onaylandı" },
            new IkDuyuruModeli { Id = 2, Baslik = "Yeni Ekip Arkadaşımız!", DuyuruIcerigi = "Ekibimize yeni katılan arkadaşımıza hoş geldin diyoruz.", HedefKitle = "Üretim Birimi", Yayinlayan = "Selin Aksoy", YayinTarihi = DateTime.Now.AddDays(-1), OkunduOnay = "Yayında" }
        };

        // ─────────────────────────────────────────────────────────────────
        // PANEL & LİSTELEME METODLARI
        // ─────────────────────────────────────────────────────────────────

        public IkPanelIstatistikleri PanelIstatistikleriniGetir()
        {
            return new IkPanelIstatistikleri { ToplamPersonel = 9999, FabrikadaAktif = 8860, MemnuniyetYuzdesi = 87, TurnoverYuzdesi = 2.4 };
        }

        public List<IkPersonelGirisModeli> GirisListesiniGetir()
        {
            return new List<IkPersonelGirisModeli>
            {
                new() { AdSoyad = "Selin Yurt", Departman = "İnsan Kaynakları", Saat = "08:30 Giriş", Durum = "ZAMANINDA", Tema = "green" },
                new() { AdSoyad = "Av. Mert Kaya", Departman = "Hukuk ve Uyumluluk", Saat = "09:45 Giriş", Durum = "GECİKME", Tema = "red" },
                new() { AdSoyad = "Hüseyin Demir", Departman = "Mavi Yaka Üretim", Saat = "19:15 Çıkış", Durum = "MESAİ", Tema = "blue" },
                new() { AdSoyad = "Kemal Aras", Departman = "Lojistik ve Sevkiyat", Saat = "16:30 Çıkış", Durum = "ERKEN", Tema = "orange" }
            };
        }

        public List<IkAktiviteModeli> AktiviteleriGetir()
        {
            return new List<IkAktiviteModeli>
            {
                new() { Baslik = "Canan Demir", Aciklama = "Bugün Doğum Günü!", Etiket = "BUGÜN", Tema = "purple" },
                new() { Baslik = "Teknik Toplantı", Aciklama = "14:00 - Oda B", Etiket = "TEKNİK", Tema = "blue" },
                new() { Baslik = "Yeni Duyuru", Aciklama = "Yemek Menüsü Güncellendi", Etiket = "DUYURU", Tema = "orange" },
                new() { Baslik = "İSG Eğitimi", Aciklama = "Tüm Personel Katılımı", Etiket = "EĞİTİM", Tema = "green" }
            };
        }

        public List<IkDepartmanDagilimModeli> DepartmanDagiliminiGetir(string tip)
        {
            var tumDepartmanlar = new List<IkDepartmanDagilimModeli>
            {
                new() { DepartmanAdi = "İnsan Kaynakları", KisiSayisi = 18, Tip = "idari" },
                new() { DepartmanAdi = "Hukuk ve Uyumluluk", KisiSayisi = 4, Tip = "idari" },
                new() { DepartmanAdi = "İdari İşler", KisiSayisi = 15, Tip = "idari" },
                new() { DepartmanAdi = "Satın Alma", KisiSayisi = 8, Tip = "idari" },
                new() { DepartmanAdi = "Finans ve Muhasebe", KisiSayisi = 10, Tip = "idari" },
                new() { DepartmanAdi = "Bilgi İşlem", KisiSayisi = 6, Tip = "teknik" },
                new() { DepartmanAdi = "İş Sağlığı ve Güvenliği", KisiSayisi = 10, Tip = "teknik" },
                new() { DepartmanAdi = "Bakım ve Enerji", KisiSayisi = 22, Tip = "teknik" },
                new() { DepartmanAdi = "Kalite Kontrol", KisiSayisi = 18, Tip = "teknik" },
                new() { DepartmanAdi = "Mavi Yaka Üretim", KisiSayisi = 850, Tip = "saha" },
                new() { DepartmanAdi = "Gri Yaka Formen", KisiSayisi = 45, Tip = "saha" },
                new() { DepartmanAdi = "Lojistik ve Sevkiyat", KisiSayisi = 85, Tip = "saha" },
                new() { DepartmanAdi = "Endüstriyel Temizlik", KisiSayisi = 14, Tip = "saha" },
                new() { DepartmanAdi = "Güvenlik", KisiSayisi = 16, Tip = "saha" }
            };

            if (tip == "tumu") return tumDepartmanlar;
            return tumDepartmanlar.Where(d => d.Tip == tip).ToList();
        }

        public List<MesajModeli> GelenMesajlariGetir()
        {
            return new List<MesajModeli>
            {
                new MesajModeli { Birim = "HUKUK", Mesaj = "6 adet yeni iş sözleşmesi onaylandı.", Saat = "10:50", OkunduMu = false },
                new MesajModeli { Birim = "BİLGİ İŞLEM", Mesaj = "Sunucu bakım çalışması tamamlandı.", Saat = "09:12", OkunduMu = true }
            };
        }

        public IkMemnuniyetAnalizModeli MemnuniyetAnaliziGetir()
        {
            return new IkMemnuniyetAnalizModeli { Hijyen = 91, Yemekhane = 90, Iletisim = 75, SosyalHaklar = 67, Katilim = 94, YanitSayisi = 1160, AylikTrend = 1.5 };
        }

        public IkTurnoverAnalizModeli TurnoverAnaliziGetir()
        {
            return new IkTurnoverAnalizModeli { ToplamAyrilan = 16, YillikOran = 2.1, SektorOrtalamasi = 4.5, RiskliDepartman = "Lojistik", RiskOrani = 8.9, IstenCikarilma = 42, BaskaIsTeklifi = 15, SehirDegisikligi = 15, EmeklilikSaglik = 28 };
        }

        public List<IkSahaAktiflikModeli> GetSahaAktiflik()
        {
            return new List<IkSahaAktiflikModeli>
            {
                new() { BirimAdi = "A-Blok Pres & Metal", AltBilgi = "Vardiya Amiri: Kenan Y.", MevcutPersonel = 185, ToplamKapasite = 200, MetrikAdi = "Hat Doluluk", MetrikYuzde = 92, DurumEtiketi = "AKTİF", Tema = "blue" },
                new() { BirimAdi = "B-Blok CNC Merkezi", AltBilgi = "Teknik Operatörler", MevcutPersonel = 112, ToplamKapasite = 120, MetrikAdi = "Otonom Verimlilik", MetrikYuzde = 93, DurumEtiketi = "NORMAL", Tema = "blue" },
                new() { BirimAdi = "Teknik Bakım & Onarım", AltBilgi = "Mekanik Müdahale Ekibi", MevcutPersonel = 14, ToplamKapasite = 18, MetrikAdi = "Ekip Hazırlık", MetrikYuzde = 63, DurumEtiketi = "MÜDAHALE VAR", Tema = "red" },
                new() { BirimAdi = "Yemekhane & Sosyal Tesisler", AltBilgi = "Personel Dinlenme Alanı", MevcutPersonel = 85, ToplamKapasite = 150, MetrikAdi = "Kapasite Doluluğu", MetrikYuzde = 56, DurumEtiketi = "MOLA SAATİ", Tema = "green" }
            };
        }

        // ─────────────────────────────────────────────────────────────────
        // PERSONEL CRUD METODLARI
        // ─────────────────────────────────────────────────────────────────

        public List<IkPersonelDetayModeli> PersonelListesiniGetir() => _personeller;

        public IkPersonelDetayModeli PersonelDetayGetir(string sicilNo)
        {
            return PersonelListesiniGetir().FirstOrDefault(x => x.SicilNo == sicilNo);
        }

        public void YeniPersonelKaydet(IkPersonelEkleModeli model)
        {
            _personeller.Add(new IkPersonelDetayModeli
            {
                AdSoyad = model.AdSoyad,
                Pozisyon = model.GorevUnvan,
                Departman = model.Departman,
                SicilNo = (202400 + _personeller.Count + 1).ToString(),
                Durum = "Aktif",
                FotoUrl = "img/default-avatar.png"
            });
        }

        public bool PersonelSil(string sicilNo)
        {
            var personel = _personeller.FirstOrDefault(p => p.SicilNo == sicilNo);
            if (personel != null)
            {
                _personeller.Remove(personel);
                return true;
            }
            return false;
        }

        public bool PersonelGuncelle(string sicilNo, IkPersonelEkleModeli model)
        {
            var personel = _personeller.FirstOrDefault(p => p.SicilNo == sicilNo);
            if (personel != null)
            {
                personel.AdSoyad = model.AdSoyad;
                personel.Pozisyon = model.GorevUnvan;
                personel.Departman = model.Departman;
                personel.TcKimlik = model.TcKimlikNo;
                personel.DogumTarihi = model.DogumTarihi;
                personel.KanGrubu = model.KanGrubu;
                personel.MedeniDurum = model.MedeniDurum;
                personel.AskerlikDurumu = model.AskerlikDurumu;
                personel.Telefon = model.Telefon;
                personel.Eposta = model.Eposta;
                personel.Adres = model.Adres;
                personel.YakinAdiSoyadi = model.YakinAdiSoyadi;
                personel.YakinlikDerecesi = model.YakinlikDerecesi;
                personel.YakinTelefon = model.YakinTelefon;
                personel.Maas = model.Maas;
                personel.Iban = model.Iban;
                personel.BankaAdi = model.BankaAdi;
                personel.HesapNumarasi = model.HesapNumarasi;
                return true;
            }
            return false;
        }

        // ─────────────────────────────────────────────────────────────────
        // İZİN YÖNETİMİ METODLARI
        // ─────────────────────────────────────────────────────────────────

        public bool IzinOnayla(int talepId)
        {
            var talep = _izinTalepleri.FirstOrDefault(x => x.Id == talepId);
            if (talep == null) return false;
            talep.Durum = "Onaylandı";
            return true;
        }

        public bool IzinReddet(int talepId, string neden)
        {
            var talep = _izinTalepleri.FirstOrDefault(x => x.Id == talepId);
            if (talep == null) return false;
            talep.Durum = "Reddedildi";
            return true;
        }

        public List<IkIzinTalepModeli> IzinListesiniGetir() => _izinTalepleri;

        public IkIzinIstatistikModeli IzinIstatistikleriniGetir()
        {
            var bugun = DateTime.Today;
            return new IkIzinIstatistikModeli
            {
                BekleyenCount = _izinTalepleri.Count(x => x.Durum == "Bekliyor"),
                IzindeCount = _izinTalepleri.Count(x => x.Durum == "Onaylandı"),
                GelecekCount = _izinTalepleri.Count(x => x.Durum == "Onaylandı")
            };
        }

        // ─────────────────────────────────────────────────────────────────
        // DUYURU & TALEP METODLARI
        // ─────────────────────────────────────────────────────────────────

        public List<IkDuyuruModeli> SonDuyurulariGetir()
        {
            return _duyuruListesi.OrderByDescending(x => x.YayinTarihi).ToList();
        }

        public bool DuyuruKaydet(IkDuyuruModeli yeniDuyuru)
        {
            try
            {
                yeniDuyuru.Id = _duyuruListesi.Max(x => x.Id) + 1;
                _duyuruListesi.Add(yeniDuyuru);
                return true;
            }
            catch { return false; }
        }

        public object BekleyenVeriyiGetir(string sicilNo)
        {
            return _onayBekleyenVeriler.ContainsKey(sicilNo) ? _onayBekleyenVeriler[sicilNo] : null;
        }

        public void GuncellemeTalebiOlustur(string sicilNo, object yeniVeriler)
        {
            if (_onayBekleyenVeriler.ContainsKey(sicilNo))
                _onayBekleyenVeriler[sicilNo] = yeniVeriler;
            else
                _onayBekleyenVeriler.Add(sicilNo, yeniVeriler);
        }

        public void BekleyenTalebiSil(string sicilNo)
        {
            if (_onayBekleyenVeriler.ContainsKey(sicilNo)) _onayBekleyenVeriler.Remove(sicilNo);
        }

        public List<IkDuyuruModeli> PersoneleGoreDuyuruGetir(string dept)
        {
            return _duyuruListesi
                .Where(d => d.HedefKitle == "Tüm Personel" || d.HedefKitle == dept)
                .OrderByDescending(x => x.YayinTarihi)
                .ToList();
        }

        // ─────────────────────────────────────────────────────────────────
        // BORDRO & MAAŞ SİSTEMİ METODLARI
        // ─────────────────────────────────────────────────────────────────

        private void MaaslariHesapla()
        {
            foreach (var p in _geciciBordroListesi)
            {
                p.SgkPayi = p.BrutMaas * 0.14m;
                decimal vergiOrani = p.BrutMaas <= 35000 ? 0.15m : p.BrutMaas <= 70000 ? 0.20m : 0.27m;
                p.GelirVergisi = p.BrutMaas * vergiOrani;
                p.Kesintiler = p.SgkPayi + p.GelirVergisi;
                p.NetMaas = p.BrutMaas - p.Kesintiler;
                p.VergiOraniGosterge = (int)(vergiOrani * 100);
            }
        }

        public async Task<List<IkBordroListeModel>> GetBordroListesiAsync()
        {
            MaaslariHesapla();
            return await Task.FromResult(_geciciBordroListesi);
        }

        public async Task<IkBordroOzetModel> GetBordroOzetAsync()
        {
            MaaslariHesapla();
            var brutToplam = _geciciBordroListesi.Sum(x => x.BrutMaas);
            var sgkIsci = _geciciBordroListesi.Sum(x => x.SgkPayi);
            var gelirVergisi = _geciciBordroListesi.Sum(x => x.GelirVergisi);

            var sgkIsveren = brutToplam * 0.205m;
            var damgaVergisi = brutToplam * 0.00759m;

            var netToplam = _geciciBordroListesi
                .Where(x => x.Durum == "Onaylandı" || x.Durum == "Ödendi")
                .Sum(x => x.NetMaas);

            return await Task.FromResult(new IkBordroOzetModel
            {
                BrutToplam = brutToplam,
                NetOdenen = netToplam,
                BekleyenCount = _geciciBordroListesi.Count(x => x.Durum == "Beklemede"),
                BankaMaasOdemeleri = netToplam * 0.7m,
                IkramiyeBonusOdemeleri = netToplam * 0.2m,
                YanHaklarYolYemek = netToplam * 0.1m,
                SgkIsverenToplami = sgkIsveren,
                SgkIsciToplami = sgkIsci,
                GelirVergisiToplami = gelirVergisi,
                DamgaVergisiToplami = damgaVergisi,
                ToplamYasalYukumluluk = sgkIsveren + sgkIsci + gelirVergisi + damgaVergisi
            });
        }

        public async Task<bool> BordroDurumGuncelleAsync(int id, string yeniDurum)
        {
            var personel = _geciciBordroListesi.FirstOrDefault(x => x.Id == id);
            if (personel == null) return false;
            personel.Durum = yeniDurum;
            return await Task.FromResult(true);
        }

        public async Task<bool> TopluOdemeEmriGonderAsync()
        {
            var onaylananlar = _geciciBordroListesi.Where(x => x.Durum == "Onaylandı").ToList();
            if (!onaylananlar.Any()) return false;

            foreach (var p in onaylananlar) p.Durum = "Ödendi";
            return await Task.FromResult(true);
        }

        public async Task<bool> HepsiniOnaylaAsync()
        {
            var bekleyenler = _geciciBordroListesi.Where(x => x.Durum == "Beklemede").ToList();
            if (!bekleyenler.Any()) return false;

            foreach (var p in bekleyenler) p.Durum = "Onaylandı";
            return await Task.FromResult(true);
        }
    }
}