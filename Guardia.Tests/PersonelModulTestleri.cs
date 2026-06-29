/*using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Guardia.API.Data;
using Guardia.API.Services.Personel;
using Guardia.API.DTOs.PE;
using Guardia.API.DTOs.PeGiris;
using Guardia.API.Models;

namespace Guardia.Tests
{
    // ──────────────────────────────────────────────
    // 1. GENEL PERSONEL MODÜL TESTLERİ
    // ──────────────────────────────────────────────
    public class PersonelModulTestleri
    {
        // 1. GİRİŞ TESTİ
        [Fact]
        public void GirisYap_DogruBilgiler_TrueDonmeli()
        {
            var sahteVeri = new List<Personeller>
            {
                new Personeller { SicilNo = "1001", Sifre = "GizliSifre123" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Personeller>>();
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.Provider).Returns(sahteVeri.Provider);
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.Expression).Returns(sahteVeri.Expression);
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.ElementType).Returns(sahteVeri.ElementType);
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.GetEnumerator()).Returns(sahteVeri.GetEnumerator());

            var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            mockContext.Setup(c => c.Personellers).Returns(mockSet.Object);

            var servis = new PersonelGirisServisi(mockContext.Object);
            var bilgi = new PersonelGirisBilgisi { SicilNo = "1001", Sifre = "GizliSifre123" };

            Assert.True(servis.GirisYap(bilgi));
        }

        // 2. İZİN SERVİSİ TESTİ
        [Fact]
        public void IzinTalepGonder_GecerliVeri_TrueDonmeli()
        {
            var servis = new PIzinServisi();
            var model = new PIzinTalepModeli
            {
                IzinTuru = "Yıllık",
                Tarihler = "10.05-20.05",
                Aciklama = "Staj Sonu"
            };
            Assert.True(servis.IzinTalepGonder(model));
        }

        [Fact]
        public void IzinTalepGonder_NullModel_FalseDonmeli()
        {
            var servis = new PIzinServisi();
            Assert.False(servis.IzinTalepGonder(null));
        }

        // 3. BORDRO SERVİSİ TESTİ
        [Fact]
        public void BordrolariGetir_ListeSayisiDogruMu()
        {
            var servis = new PBordroServisi();
            var sonuc = servis.BordrolariGetir();
            Assert.Equal(6, sonuc.Count);
        }

        [Fact]
        public void BordrolariGetir_IlkKayitYeniMiTrue_Olmalidir()
        {
            var servis = new PBordroServisi();
            var sonuc = servis.BordrolariGetir();
            Assert.True(sonuc.First().YeniMi); // Haziran 2026 en yeni bordro
        }

        // 4. ŞİFRE SERVİSİ TESTLERİ
        [Fact]
        public void SifreGuncelle_ElesmeyenSifre_HataDonmeli()
        {
            var servis = new SifreServisi();
            var model = new PSifreGuncellemeModeli
            {
                MevcutSifre = "eskiSifre",
                YeniSifre = "123456",
                YeniSifreTekrar = "654321"
            };
            var sonuc = servis.SifreGuncelle(model);
            Assert.False(sonuc.Basarili);
            Assert.Equal("Hata: Girdiğiniz yeni şifreler birbirini tutmuyor!", sonuc.Mesaj);
        }

        [Fact]
        public void SifreGuncelle_KisaSifre_HataDonmeli()
        {
            var servis = new SifreServisi();
            var model = new PSifreGuncellemeModeli
            {
                MevcutSifre = "eskiSifre",
                YeniSifre = "123",
                YeniSifreTekrar = "123"
            };
            var sonuc = servis.SifreGuncelle(model);
            Assert.False(sonuc.Basarili);
            Assert.Equal("Şifre en az 6 karakter olmalıdır.", sonuc.Mesaj);
        }

        [Fact]
        public void SifreGuncelle_EskiSifreKullanilirsa_HataDonmeli()
        {
            var servis = new SifreServisi();
            var model = new PSifreGuncellemeModeli
            {
                MevcutSifre = "eskiSifre",
                YeniSifre = "123456",    // Geçmiş listesindeki şifre
                YeniSifreTekrar = "123456"
            };
            var sonuc = servis.SifreGuncelle(model);
            Assert.False(sonuc.Basarili);
            Assert.Equal("Güvenlik Hatası: Yeni şifreniz, son kullandığınız 3 şifreden biri olamaz.", sonuc.Mesaj);
        }

        [Fact]
        public void SifreGuncelle_BosAlan_HataDonmeli()
        {
            var servis = new SifreServisi();
            var model = new PSifreGuncellemeModeli
            {
                MevcutSifre = "",
                YeniSifre = "yeniSifre1",
                YeniSifreTekrar = "yeniSifre1"
            };
            var sonuc = servis.SifreGuncelle(model);
            Assert.False(sonuc.Basarili);
            Assert.Equal("Lütfen tüm alanları doldurunuz.", sonuc.Mesaj);
        }

        // 5. ŞİFRE UNUTTUM TESTLERİ
        [Fact]
        public void SicilDogrula_DogruSicil_TrueDonmeli()
        {
            var servis = new PSifreUnuttumServisi();
            Assert.True(servis.SicilDogrula("1055"));
        }

        [Fact]
        public void SicilDogrula_YanlisSicil_FalseDonmeli()
        {
            var servis = new PSifreUnuttumServisi();
            Assert.False(servis.SicilDogrula("9999"));
        }

        [Fact]
        public void SmsDogrula_DogruKod_TrueDonmeli()
        {
            var servis = new PSifreUnuttumServisi();
            Assert.True(servis.SmsDogrula("123456")); // DEMO_KOD
        }

        [Fact]
        public void SmsDogrula_YanlisKod_FalseDonmeli()
        {
            var servis = new PSifreUnuttumServisi();
            Assert.False(servis.SmsDogrula("000000"));
        }

        [Fact]
        public void SifreSifirla_ElesmeyenSifreler_HataDonmeli()
        {
            var servis = new PSifreUnuttumServisi();
            var model = new PSifreSifirlamaModeli
            {
                YeniSifre = "yeniSifre1",
                YeniSifreTekrar = "farkliSifre"
            };
            var sonuc = servis.SifreSifirla(model);
            Assert.False(sonuc.Basarili);
            Assert.Equal("Şifreler eşleşmiyor.", sonuc.Mesaj);
        }

        // 6. QR SERVİSİ TESTLERİ
        [Fact]
        public void QrOlustur_FormatKontrolu_GuardiaVuslatIcermeli()
        {
            var servis = new QrServisi();
            var sonuc = servis.QrOlustur("2026");
            Assert.Contains("GUARDIA_VUSLAT", sonuc.QrData);
        }

        [Fact]
        public void QrOlustur_SicilNoKontrolu_VeriIcindeSicilNoOlmali()
        {
            var servis = new QrServisi();
            var sonuc = servis.QrOlustur("2026");
            Assert.Contains("2026", sonuc.QrData);
            Assert.Equal("Vuslat Çeliktepe", sonuc.PersonelAd);
        }

        // 7. YEMEK VE PANEL SERVİSLERİ
        [Fact]
        public void YemekMenusu_HaftaBesBesGunOlmali()
        {
            var servis = new PYemekServisi();
            var sonuc = servis.HaftaMenusuGetir();
            Assert.Equal(5, sonuc.Count);
        }

        [Fact]
        public void YemekMenusu_BugunMuSadeceBirGunTrue_Olmalidir()
        {
            var servis = new PYemekServisi();
            var sonuc = servis.HaftaMenusuGetir();
            Assert.Equal(1, sonuc.Count(g => g.BugunMu)); 
        }

        [Fact]
        public void PanelBilgisi_AdSoyad_AyseYilmazOlmali()
        {
            var servis = new PersonelPanelServisi();
            var sonuc = servis.PanelBilgisiniGetir();
            Assert.Equal("Ayşe Yılmaz", sonuc.AdSoyad);
        }

        [Fact]
        public void PanelBilgisi_BildirimVarMi_TrueOlmali()
        {
            var servis = new PersonelPanelServisi();
            var sonuc = servis.PanelBilgisiniGetir();
            Assert.True(sonuc.BildirimVarMi);
        }
    }

    // ──────────────────────────────────────────────
    // 2. PERSONEL GİRİŞ SERVİSİ TESTLERİ (Moq)
    // ──────────────────────────────────────────────
    public class PersonelGirisServisiTestleri
    {
        private Mock<DbSet<Personeller>> MokDbSetOlustur(List<Personeller> veri)
        {
            var sorgulanabilir = veri.AsQueryable();
            var mockSet = new Mock<DbSet<Personeller>>();
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.Provider).Returns(sorgulanabilir.Provider);
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.Expression).Returns(sorgulanabilir.Expression);
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.ElementType).Returns(sorgulanabilir.ElementType);
            mockSet.As<IQueryable<Personeller>>().Setup(m => m.GetEnumerator()).Returns(sorgulanabilir.GetEnumerator());
            return mockSet;
        }

        [Fact]
        public void GirisYap_DogruSicilNoVeSifre_TrueDonmeli()
        {
            var mockSet = MokDbSetOlustur(new List<Personeller>
            {
                new Personeller { SicilNo = "1001", Sifre = "GizliSifre123" }
            });
            var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            mockContext.Setup(c => c.Personellers).Returns(mockSet.Object);

            var servis = new PersonelGirisServisi(mockContext.Object);
            var bilgi = new PersonelGirisBilgisi { SicilNo = "1001", Sifre = "GizliSifre123" };

            Assert.True(servis.GirisYap(bilgi));
        }

        [Fact]
        public void GirisYap_YanlisSifre_FalseDonmeli()
        {
            var mockSet = MokDbSetOlustur(new List<Personeller>
            {
                new Personeller { SicilNo = "1001", Sifre = "GizliSifre123" }
            });
            var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            mockContext.Setup(c => c.Personellers).Returns(mockSet.Object);

            var servis = new PersonelGirisServisi(mockContext.Object);
            var bilgi = new PersonelGirisBilgisi { SicilNo = "1001", Sifre = "YanlisSifre456" };

            Assert.False(servis.GirisYap(bilgi));
        }
    }

    // ──────────────────────────────────────────────
    // 3. İZİN SERVİSİ TESTLERİ
    // ──────────────────────────────────────────────
    public class PIzinServisiTestleri
    {
        [Fact]
        public void IzinTalepGonder_GecerliModelGeldiginde_TrueDonmeli()
        {
            var servis = new PIzinServisi();
            var testModeli = new PIzinTalepModeli
            {
                IzinTuru = "Yıllık İzin",
                Tarihler = "15.06.2026 - 20.06.2026",
                Aciklama = "Yaz tatili"
            };
            Assert.True(servis.IzinTalepGonder(testModeli));
        }

        [Fact]
        public void IzinTalepGonder_ModelBosGelirse_FalseDonmeli()
        {
            var servis = new PIzinServisi();
            Assert.False(servis.IzinTalepGonder(null));
        }
    }
}*\

/* 
 * NOT: Bu testler veritabanı entegrasyonundan önce yazılmıştır.
 * Birim testi prensibine uygun olarak mock veri kullanılmıştır.
 * Veritabanı bağlantısı sonrası servis imzaları değiştiğinden
 * bu testler güncelleme beklemektedir.
 */