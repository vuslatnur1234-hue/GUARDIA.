

 using Guardia.API.DTOs.IK;
using Guardia.API.Services.InsanKaynaklari;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Guardia.Tests
{
    public class InsanKaynaklariModulTestleri
    {
        private readonly IkTestService _service;

        public InsanKaynaklariModulTestleri()
        {
            _service = new IkTestService();
        }

        // 1. PANEL İSTATİSTİKLERİ TESTİ
        [Fact]
        public void PanelIstatistikleriniGetir_VerilerEksiksizDonmeli()
        {
            var sonuc = _service.PanelIstatistikleriniGetir();
            Assert.Equal(9999, sonuc.ToplamPersonel);
            Assert.Equal(2.4, sonuc.TurnoverYuzdesi);
        }

        // 2. PERSONEL GİRİŞ LİSTESİ TESTİ
        [Fact]
        public void GirisListesiniGetir_DortKayitDonmeli()
        {
            var liste = _service.GirisListesiniGetir();
            Assert.Equal(4, liste.Count);
            Assert.Contains(liste, x => x.AdSoyad == "Selin Yurt");
        }

        // 3. AKTİVİTE LİSTESİ TESTİ
        [Fact]
        public void AktiviteleriGetir_EtiketKontrolu()
        {
            var liste = _service.AktiviteleriGetir();
            Assert.Contains(liste, x => x.Etiket == "BUGÜN");
            Assert.Equal(4, liste.Count);
        }

        // 4. DEPARTMAN DAĞILIMI TESTLERİ (Filtreleme Mantığı)
        [Theory]
        [InlineData("tumu", 14)]
        [InlineData("idari", 5)]
        [InlineData("teknik", 4)]
        [InlineData("saha", 5)]
        public void DepartmanDagiliminiGetir_FiltreyeGoreDogruSayidaDonmeli(string tip, int beklenenSayi)
        {
            var sonuc = _service.DepartmanDagiliminiGetir(tip);
            Assert.Equal(beklenenSayi, sonuc.Count);
        }

        // 5. MESAJ TESTİ
        [Fact]
        public void GelenMesajlariGetir_OkunmamisMesajVarMi()
        {
            var mesajlar = _service.GelenMesajlariGetir();
            Assert.Contains(mesajlar, m => !m.OkunduMu);
        }

        // 6. ANALİZ TESTLERİ (Memnuniyet & Turnover)
        [Fact]
        public void AnalizleriGetir_DegerlerTutarlıMi()
        {
            var memnuniyet = _service.MemnuniyetAnaliziGetir();
            var turnover = _service.TurnoverAnaliziGetir();

            Assert.Equal(91, memnuniyet.Hijyen);
            Assert.Equal("Lojistik", turnover.RiskliDepartman);
        }

        // 7. SAHA AKTİFLİK TESTİ
        [Fact]
        public void GetSahaAktiflik_KapasiteKontrolu()
        {
            var saha = _service.GetSahaAktiflik();
            Assert.Contains(saha, s => s.BirimAdi == "A-Blok Pres & Metal");
            Assert.True(saha.Any(s => s.MetrikYuzde > 0));
        }

        // 8. PERSONEL CRUD TESTLERİ
        [Fact]
        public void PersonelIslemleri_EklemeGuncellemeVeSilme_BasariliOlmali()
        {
            // EKLEME
            var yeni = new IkPersonelEkleModeli { AdSoyad = "İrem Ekinci", Departman = "Yazılım", GorevUnvan = "Geliştirici" };
            _service.YeniPersonelKaydet(yeni);
            var liste = _service.PersonelListesiniGetir();
            var eklenen = liste.FirstOrDefault(x => x.AdSoyad == "İrem Ekinci");
            Assert.NotNull(eklenen);

            // GÜNCELLEME
            var guncelModel = new IkPersonelEkleModeli { AdSoyad = "İrem Güncel", Departman = "Yazılım", GorevUnvan = "Senior" };
            var guncelleSonuc = _service.PersonelGuncelle(eklenen.SicilNo, guncelModel);
            Assert.True(guncelleSonuc);
            Assert.Equal("İrem Güncel", eklenen.AdSoyad);

            // SİLME
            var silSonuc = _service.PersonelSil(eklenen.SicilNo);
            Assert.True(silSonuc);
            Assert.DoesNotContain(_service.PersonelListesiniGetir(), x => x.SicilNo == eklenen.SicilNo);
        }

        
        [Fact]
        public void PersonelSil_OlmayanSicilNo_FalseDonmeli()
        {
            // Atomik/Negatif: Olmayan bir personeli silmeye çalışınca sistemin hata vermediğini, 
            // false dönerek durumu yönettiğini kanıtlar.
            var sonuc = _service.PersonelSil("999999"); // Olmayan bir numara
            Assert.False(sonuc);
        }

        [Fact]
        public void BekleyenVeriyiGetir_TalepYoksa_NullDonmeli()
        {
            // Atomik: Henüz talep oluşturulmamış bir sicil no için null dönüp dönmediğini bakar.
            var sonuc = _service.BekleyenVeriyiGetir("HIC_TALEP_YOK");
            Assert.Null(sonuc);
        }


        // 9. İZİN YÖNETİMİ TESTLERİ
        [Fact]
        public void IzinIslemleri_OnayVeRed_DurumuDegistirmeli()
        {
            _service.IzinOnayla(1);
            Assert.Equal("Onaylandı", _service.IzinListesiniGetir().First(x => x.Id == 1).Durum);

            _service.IzinReddet(2, "Uygun değil");
            Assert.Equal("Reddedildi", _service.IzinListesiniGetir().First(x => x.Id == 2).Durum);
        }

        // 10. BORDRO HESAPLAMA MANTIĞI TESTİ (En Kritik Test)
        [Fact]
        public async Task GetBordroOzetAsync_HesaplamalarYasalOranlaraUygunMu()
        {
            var ozet = await _service.GetBordroOzetAsync();

            // Esma Çelik 45.000 TL -> %20 Vergi dilimi (45000 * 0.20 = 9000)
            // Sgk Payı %14 -> (45000 * 0.14 = 6300)
            // Beklenen Net: 45000 - (9000 + 6300) = 29700

            var list = await _service.GetBordroListesiAsync();
            var esma = list.First(x => x.AdSoyad == "Esma Çelik");

            Assert.Equal(29700m, esma.NetMaas);
            Assert.Equal(9000m, esma.GelirVergisi);
        }

        // 11. BORDRO TOPLU İŞLEMLER TESTİ
        [Fact]
        public async Task BordroIslemleri_TopluOnayVeOdeme_DurumlariGuncellemeli()
        {
            await _service.HepsiniOnaylaAsync();
            var liste = await _service.GetBordroListesiAsync();
            Assert.All(liste.Where(x => x.Durum != "Ödendi"), x => Assert.Equal("Onaylandı", x.Durum));

            await _service.TopluOdemeEmriGonderAsync();
            Assert.All(liste.Where(x => x.Durum == "Onaylandı"), x => Assert.Equal("Ödendi", x.Durum));
        }

        // 12. DUYURU TESTLERİ
        [Fact]
        public void DuyuruIslemleri_KaydetVeFiltrele()
        {
            var yeniDuyuru = new IkDuyuruModeli { Baslik = "Test Duyuru", HedefKitle = "Yazılım", YayinTarihi = DateTime.Now };
            _service.DuyuruKaydet(yeniDuyuru);

            var tumDuyurular = _service.SonDuyurulariGetir();
            Assert.Contains(tumDuyurular, d => d.Baslik == "Test Duyuru");

            var yazilimDuyuru = _service.PersoneleGoreDuyuruGetir("Yazılım");
            Assert.Contains(yazilimDuyuru, d => d.HedefKitle == "Yazılım");
        }

        // 13. DICTIONARY TABANLI TALEP YÖNETİMİ (Zil/Bildirim) TESTİ
        [Fact]
        public void OnayBekleyenVeri_DictionaryMantigiDogruCalismali()
        {
            string sicil = "202401";
            var yeniVeri = new { Telefon = "555-0000", Adres = "İstanbul" };

            _service.GuncellemeTalebiOlustur(sicil, yeniVeri);
            var bekleyen = _service.BekleyenVeriyiGetir(sicil);

            Assert.NotNull(bekleyen);
            Assert.Equal(yeniVeri, bekleyen);

            _service.BekleyenTalebiSil(sicil);
            Assert.Null(_service.BekleyenVeriyiGetir(sicil));
        }


        [Fact]
        public void GelenMesajlariGetir_VeriBosDonmemeli()
        {
            // Atomik: Sadece mesaj listesinin null olmadığını ve veri içerdiğini denetler
            var sonuc = _service.GelenMesajlariGetir();
            Assert.NotNull(sonuc);
            Assert.NotEmpty(sonuc);
        }

        [Fact]
        public void MemnuniyetAnaliziGetir_DegerAraligiDogruMu()
        {
            // Atomik: Sadece memnuniyet verisinin mantıklı sınırlar içinde olduğunu denetler
            var sonuc = _service.MemnuniyetAnaliziGetir();
            Assert.InRange(sonuc.Hijyen, 0, 100);
        }

        [Theory]
        [InlineData(35000, 15)] // Tam sınır: %15 olmalı
        [InlineData(35001, 20)] // Sınırın 1 TL üstü: %20'ye geçmeli
        [InlineData(70000, 20)] // Üst sınır: %20 olmalı
        [InlineData(70001, 27)] // Üst sınırın 1 TL üstü: %27'ye geçmeli
        public async Task GetBordroListesiAsync_VergiDilimiSinirlari_DogruHesaplanmali(decimal brutMaas, int bekleyenVergiOrani)
        {
            // Arrange
            var testPersonel = new IkBordroListeModel { Id = 99, AdSoyad = "Sınır Testi", BrutMaas = brutMaas, Durum = "Beklemede" };

            // Service içindeki static listeye yansıtmak ya da test etmek için
           
            var liste = await _service.GetBordroListesiAsync();
            var hedef = liste.First();
            var eskiBrut = hedef.BrutMaas;

            hedef.BrutMaas = brutMaas;

            // Act
            await _service.GetBordroListesiAsync(); // Hesaplamayı tetikle

            // Assert
            Assert.Equal(bekleyenVergiOrani, hedef.VergiOraniGosterge);

            // Cleanup 
            hedef.BrutMaas = eskiBrut;
        }

        [Fact]
        public void IzinIstatistikleriniGetir_IzinDurumlariDegistikce_IstatistiklerGuncellenmeli()
        {
            
            // Eğer diğer testler yüzünden hiç kalmadıysa, teste özel geçici bir talep ekleyebiliriz.
            var liste = _service.IzinListesiniGetir();
            var bekleyenTalep = liste.FirstOrDefault(x => x.Durum == "Bekliyor");

            if (bekleyenTalep == null)
            {
                // Eğer önceki testler listeyi tükettiyse, testin çalışabilmesi için araya sahte bir bekleyen ekleniyor
                bekleyenTalep = new IkIzinTalepModeli { Id = 999, PersonelAd = "Test", Durum = "Bekliyor" };
                liste.Add(bekleyenTalep);
            }
            // Arrange: İlk durum sayılarını dinamik olarak alıyoruz
            var ilkIstatistik = _service.IzinIstatistikleriniGetir();
            var ilkBekleyen = ilkIstatistik.BekleyenCount;
            var ilkOnaylanan = ilkIstatistik.IzindeCount;

         
            _service.IzinOnayla(bekleyenTalep.Id);
            var yeniIstatistik = _service.IzinIstatistikleriniGetir();

          
            Assert.Equal(ilkBekleyen - 1, yeniIstatistik.BekleyenCount);
            Assert.Equal(ilkOnaylanan + 1, yeniIstatistik.IzindeCount);

            // Cleanup
            if (bekleyenTalep.Id == 999)
            {
                liste.Remove(bekleyenTalep);
            }
        }

        [Fact]
        public void IzinOnaylaVeReddet_OlmayanIzinId_FalseDonmeli()
        {
            // Act
            var onaySonuc = _service.IzinOnayla(9999); // Geçersiz ID
            var redSonuc = _service.IzinReddet(9999, "Neden"); // Geçersiz ID

            // Assert
            Assert.False(onaySonuc);
            Assert.False(redSonuc);
        }


        [Fact]
        public void GuncellemeTalebiOlustur_AyniSicilNoTekrarGeldiginde_VeriyiGuncellemeli()
        {
            // Arrange
            string sicilNo = "202402";
            var ilkVeri = new { Telefon = "111" };
            var ikinciVeri = new { Telefon = "222" };

            // Act
            _service.GuncellemeTalebiOlustur(sicilNo, ilkVeri);
            _service.GuncellemeTalebiOlustur(sicilNo, ikinciVeri); // Üzerine yazmalı

            var sonuc = _service.BekleyenVeriyiGetir(sicilNo);

            // Assert
            Assert.NotNull(sonuc);
            Assert.Equal(ikinciVeri, sonuc); // Veri ilki değil ikincisi olmalı

            // Cleanup
            _service.BekleyenTalebiSil(sicilNo);
        }


        [Fact]
        public async Task HepsiniOnaylaAsync_BekleyenBordroYoksa_FalseDonmeli()
        {
            // Act & Arrange
            await _service.HepsiniOnaylaAsync(); // İlk çağrıda hepsini onayladık, "Beklemede" statüsü kalmadı.

            var ikinciCagriSonucu = await _service.HepsiniOnaylaAsync(); // Şimdi kontrol et

            // Assert
            Assert.False(ikinciCagriSonucu);
        }


        [Fact]
        public void DuyuruKaydet_YeniDuyuruEklendiginde_IdOtomatikArtmali()
        {
            // Arrange
            var sonDuyuru = _service.SonDuyurulariGetir().Max(x => x.Id);
            var yeniDuyuru = new IkDuyuruModeli { Baslik = "Yeni Sistem Duyurusu", HedefKitle = "Tüm Personel" };

            // Act
            var basariliMi = _service.DuyuruKaydet(yeniDuyuru);

            // Assert
            Assert.True(basariliMi);
            Assert.Equal(sonDuyuru + 1, yeniDuyuru.Id);
        }


        [Fact]
        public void PersonelDetayGetir_GecersizSicilNo_NullDonmeli()
        {
            // Act
            var sonuc = _service.PersonelDetayGetir("GEÇERSİZ_SİCİL");

            // Assert
            Assert.Null(sonuc);
        }


        [Fact]
        public async Task GetBordroOzetAsync_YasalYukumluluklerinToplami_GenelToplamaEsitOlmali()
        {
            // Act
            var ozet = await _service.GetBordroOzetAsync();

            decimal beklenenToplam = ozet.SgkIsverenToplami +
                                     ozet.SgkIsciToplami +
                                     ozet.GelirVergisiToplami +
                                     ozet.DamgaVergisiToplami;

            // Assert
            Assert.Equal(beklenenToplam, ozet.ToplamYasalYukumluluk);
        }

        [Fact]
        public void PersonelGuncelle_OlmayanPersonel_FalseDonmeli()
        {
            // Arrange
            var model = new IkPersonelEkleModeli { AdSoyad = "Hata Testi" };

            // Act
            var sonuc = _service.PersonelGuncelle("999999", model);

            // Assert
            Assert.False(sonuc);
        }


        [Fact]
        public void PersoneleGoreDuyuruGetir_SadeceIlgiliDepartmanVeGenelDuyurulariGetirmeli()
        {
            // Act
            var yazilimDuyurulari = _service.PersoneleGoreDuyuruGetir("Yazılım");

            // Assert
            // İçinde "Üretim Birimi" gibi tamamen farklı bir kitleye ait duyuru olmamalı.
            Assert.DoesNotContain(yazilimDuyurulari, d => d.HedefKitle == "Üretim Birimi");
            // "Tüm Personel" olanları içermeli.
            Assert.Contains(yazilimDuyurulari, d => d.HedefKitle == "Tüm Personel");
        }
    }
}
