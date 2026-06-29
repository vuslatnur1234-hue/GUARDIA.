using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Guardia.API.Data;
using Guardia.API.Models;
using Guardia.API.DTOs.PeGiris;
using Guardia.API.DTOs.PE;
using Guardia.API.DTOs;
using Guardia.API.Services.Personel;
using Guardia.API.Services;
using System.Collections.Generic;
using System.Linq;

namespace Guardia.Tests
{
    public class PersonelServisTestleri
    {
        // ─── MOCK CONTEXT BUILDER ───
        private class MockContextBuilder
        {
            private Mock<AppDbContext> _context;
            private Dictionary<string, object> _mocks = new();

            public MockContextBuilder()
            {
                _context = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            }

            public MockContextBuilder WithPersoneller(List<Personeller> veri)
            {
                var mock = CreateMockDbSet(veri);
                _context.Setup(c => c.Personellers).Returns(mock.Object);
                return this;
            }

            public MockContextBuilder WithGirisBilgileri(List<PersonelGirisBilgileri> veri)
            {
                var mock = CreateMockDbSet(veri);
                _context.Setup(c => c.PersonelGirisBilgileris).Returns(mock.Object);
                return this;
            }

            public MockContextBuilder WithBildirimler(List<Bildirimler> veri)
            {
                var mock = CreateMockDbSet(veri);
                _context.Setup(c => c.Bildirimlers).Returns(mock.Object);
                return this;
            }

            public MockContextBuilder WithArizalar(List<Arizalar> veri)
            {
                var mock = CreateMockDbSet(veri);
                _context.Setup(c => c.Arizalars).Returns(mock.Object);
                return this;
            }

            public MockContextBuilder WithBordrolar(List<Bordrolar> veri)
            {
                var mock = CreateMockDbSet(veri);
                _context.Setup(c => c.Bordrolars).Returns(mock.Object);
                return this;
            }

            public MockContextBuilder WithIzinler(List<Izinler> veri)
            {
                var mock = CreateMockDbSet(veri);
                _context.Setup(c => c.Izinlers).Returns(mock.Object);
                return this;
            }

            public MockContextBuilder WithSaveChanges(int returnValue = 1)
            {
                _context.Setup(c => c.SaveChanges()).Returns(returnValue);
                return this;
            }

            public Mock<AppDbContext> Build() => _context;

            private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> veri) where T : class
            {
                var sorgu = veri.AsQueryable();
                var mockSet = new Mock<DbSet<T>>();
                mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(sorgu.Provider);
                mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(sorgu.Expression);
                mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(sorgu.ElementType);
                mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(sorgu.GetEnumerator());
                return mockSet;
            }
        }

        // ─── TEST DATA FIXTURES ───
        private static Personeller PersonelSIC001() =>
            new Personeller { Id = 1, SicilNo = "SIC001", AdSoyad = "Ahmet Yılmaz", Telefon = "05321112233" };

        private static PersonelGirisBilgileri GirisBilgisiSIC001(string sifre = "Ahmet123!", bool kilit = false, int hatali = 0) =>
            new PersonelGirisBilgileri { SicilNo = "SIC001", Sifre = sifre, HesapKilitliMi = kilit, HataliDenemeSayisi = hatali };

        // ─── 1. GİRİŞ TESTLERİ ───

        [Fact]
        public void GirisYap_DogruBilgiler_TrueDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithGirisBilgileri(new List<PersonelGirisBilgileri> { GirisBilgisiSIC001() })
                .Build();

            var servis = new PersonelGirisServisi(context.Object);
            var bilgi = new PersonelGirisBilgisi { SicilNo = "SIC001", Sifre = "Ahmet123!" };

            Assert.True(servis.GirisYap(bilgi));
        }

        [Fact]
        public void GirisYap_YanlisSifre_FalseDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithGirisBilgileri(new List<PersonelGirisBilgileri> { GirisBilgisiSIC001() })
                .Build();

            var servis = new PersonelGirisServisi(context.Object);
            var bilgi = new PersonelGirisBilgisi { SicilNo = "SIC001", Sifre = "YanlisSifre" };

            Assert.False(servis.GirisYap(bilgi));
        }

        [Fact]
        public void GirisYap_KilitliHesap_FalseDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithGirisBilgileri(new List<PersonelGirisBilgileri> { GirisBilgisiSIC001(kilit: true) })
                .Build();

            var servis = new PersonelGirisServisi(context.Object);
            var bilgi = new PersonelGirisBilgisi { SicilNo = "SIC001", Sifre = "Ahmet123!" };

            Assert.False(servis.GirisYap(bilgi));
        }

        [Fact]
        public void GirisYap_OlmayanSicil_FalseDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller>())
                .WithGirisBilgileri(new List<PersonelGirisBilgileri>())
                .Build();

            var servis = new PersonelGirisServisi(context.Object);
            var bilgi = new PersonelGirisBilgisi { SicilNo = "SIC999", Sifre = "herhangi" };

            Assert.False(servis.GirisYap(bilgi));
        }

        // ─── 2. PROFİL TESTLERİ ───

        [Fact]
        public void ProfilGetir_GecerliSicil_BilgileriDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithBildirimler(new List<Bildirimler>())
                .Build();

            var servis = new PersonelMerkeziServis(context.Object);
            var sonuc = servis.ProfilBilgileriniGetir("SIC001");

            Assert.NotNull(sonuc);
            Assert.Equal("Ahmet Yılmaz", sonuc.AdSoyad);
            Assert.Equal("05321112233", sonuc.Telefon);
        }

        [Fact]
        public void ProfilGetir_OlmayanSicil_NullDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller>())
                .Build();

            var servis = new PersonelMerkeziServis(context.Object);
            var sonuc = servis.ProfilBilgileriniGetir("SIC999");

            Assert.Null(sonuc);
        }

        // ─── 3. ARIZA TESTLERİ ───

        [Fact]
        public void ArizaKaydet_GecerliVeri_TrueDonmeli()
        {
            var mockArizalar = new Mock<DbSet<Arizalar>>();
            mockArizalar.Setup(m => m.Add(It.IsAny<Arizalar>()));

            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithSaveChanges(1)
                .Build();
            context.Setup(c => c.Arizalars).Returns(mockArizalar.Object);

            var servis = new ArizaServisi(context.Object);
            var model = new ArizaKayitModeli
            {
                SicilNo = "SIC001",
                Makine = "CNC Hattı",
                Tip = "Mekanik",
                Aciliyet = "Kritik",
                Detay = "Makine durdu"
            };

            Assert.True(servis.ArizaKaydet(model));
        }

        [Fact]
        public void ArizaKaydet_OlmayanPersonel_FalseDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller>())
                .Build();

            var servis = new ArizaServisi(context.Object);
            var model = new ArizaKayitModeli { SicilNo = "SIC999" };

            Assert.False(servis.ArizaKaydet(model));
        }

        // ─── 4. BİLDİRİM TESTLERİ ───

        [Theory]
        [InlineData("SIC001", 1)]
        [InlineData("SIC999", 0)]
        public void BildirimleriGetir(string sicil, int beklenenCount)
        {
            var personeller = sicil == "SIC001" ? new List<Personeller> { PersonelSIC001() } : new List<Personeller>();
            var bildirimler = sicil == "SIC001" ? new List<Bildirimler>
            {
                new Bildirimler { Id = 1, PersonelId = 1, Baslik = "Test", Mesaj = "Mesaj", Okundu = false }
            } : new List<Bildirimler>();

            var context = new MockContextBuilder()
                .WithPersoneller(personeller)
                .WithBildirimler(bildirimler)
                .Build();

            var servis = new BildirimServisi(context.Object);
            var sonuc = servis.BildirimleriGetir(sicil);

            Assert.Equal(beklenenCount, sonuc.Count);
            if (beklenenCount > 0)
                Assert.Equal("Test", sonuc.First().Baslik);
        }

        // ─── 5. ŞİFRE DEĞİŞTİRME TESTLERİ ───

        [Fact]
        public void SifreGuncelle_DogruBilgiler_TrueDonmeli()
        {
            var context = new MockContextBuilder()
                .WithGirisBilgileri(new List<PersonelGirisBilgileri> { GirisBilgisiSIC001() })
                .WithSaveChanges(1)
                .Build();

            var servis = new PSifreUnuttumServisi(context.Object);
            var model = new PSifreGuncellemeModeli
            {
                SicilNo = "SIC001",
                MevcutSifre = "Ahmet123!",
                YeniSifre = "YeniSifre1!",
                YeniSifreTekrar = "YeniSifre1!"
            };

            var sonuc = servis.SifreGuncelle(model);
            Assert.True(sonuc.Basarili);
        }

        [Theory]
        [InlineData("YanlisSifre", "Ahmet123!", "Ahmet123!")]  // Yanlış mevcut şifre
        [InlineData("Ahmet123!", "YeniSifre1!", "FarkliSifre!")]  // Eşleşmeyen yeni şifreler
        [InlineData("Ahmet123!", "123", "123")]  // Kısa şifre
        [InlineData("", "YeniSifre1!", "YeniSifre1!")]  // Boş alan
        public void SifreGuncelle_HataScenarios_FalseDonmeli(string mevcutSifre, string yeniSifre, string tekrar)
        {
            var context = new MockContextBuilder()
                .WithGirisBilgileri(new List<PersonelGirisBilgileri> { GirisBilgisiSIC001() })
                .Build();

            var servis = new PSifreUnuttumServisi(context.Object);
            var model = new PSifreGuncellemeModeli
            {
                SicilNo = "SIC001",
                MevcutSifre = mevcutSifre,
                YeniSifre = yeniSifre,
                YeniSifreTekrar = tekrar
            };

            var sonuc = servis.SifreGuncelle(model);
            Assert.False(sonuc.Basarili);
        }

        // ─── 6. ŞİFRE SIFIRLAMA TESTLERİ ───

        [Fact]
        public void SifreSifirla_DogruBilgiler_TrueDonmeli()
        {
            var context = new MockContextBuilder()
                .WithGirisBilgileri(new List<PersonelGirisBilgileri> { GirisBilgisiSIC001() })
                .WithSaveChanges(1)
                .Build();

            var servis = new PSifreUnuttumServisi(context.Object);
            var model = new PSifreSifirlamaModeli
            {
                SicilNo = "SIC001",
                YeniSifre = "YeniSifre1!",
                YeniSifreTekrar = "YeniSifre1!"
            };

            var sonuc = servis.SifreSifirla(model);
            Assert.True(sonuc.Basarili);
        }

        [Theory]
        [InlineData("YeniSifre1!", "FarkliSifre!", "Şifreler eşleşmiyor.")]
        [InlineData("123", "123", "Şifre en az 6 karakter olmalıdır.")]
        public void SifreSifirla_HataScenarios_FalseDonmeli(string sifre1, string sifre2, string expectedMsg)
        {
            var context = new MockContextBuilder().Build();
            var servis = new PSifreUnuttumServisi(context.Object);

            var model = new PSifreSifirlamaModeli
            {
                SicilNo = "SIC001",
                YeniSifre = sifre1,
                YeniSifreTekrar = sifre2
            };

            var sonuc = servis.SifreSifirla(model);
            Assert.False(sonuc.Basarili);
            Assert.Equal(expectedMsg, sonuc.Mesaj);
        }

        // ─── 7. BORDRO TESTLERİ ───

        [Fact]
        public void BordrolariGetir_GecerliSicil_ListeDonmeli()
        {
            var bordrolar = new List<Bordrolar>
            {
                new Bordrolar { Id = 1, PersonelId = 1, AyYil = "Mayıs 2026", OdemeTarihi = new DateOnly(2026, 5, 15) },
                new Bordrolar { Id = 2, PersonelId = 1, AyYil = "Nisan 2026", OdemeTarihi = new DateOnly(2026, 4, 15) }
            };

            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithBordrolar(bordrolar)
                .Build();

            var servis = new BordroServisi(context.Object);
            var sonuc = servis.BordrolariGetir("SIC001");

            Assert.NotEmpty(sonuc);
            Assert.Equal(2, sonuc.Count);
        }

        [Fact]
        public void BordrolariGetir_OlmayanSicil_BosListeDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller>())
                .Build();

            var servis = new BordroServisi(context.Object);
            var sonuc = servis.BordrolariGetir("SIC999");

            Assert.Empty(sonuc);
        }

        // ─── 8. PROFİL GÜNCELLEME TESTLERİ ───

        [Fact]
        public void ProfilGuncelle_GecerliVeri_TrueDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithSaveChanges(1)
                .Build();

            var servis = new PersonelMerkeziServis(context.Object);
            var model = new PersonelProfilModeli
            {
                SicilNo = "SIC001",
                Telefon = "05329998877",
                Adres = "Yeni Adres"
            };

            Assert.True(servis.ProfilGuncelle(model));
        }

        [Theory]
        [InlineData("SIC999")]  // Olmayan sicil
        [InlineData("")]        // Boş sicil
        public void ProfilGuncelle_HataScenarios_FalseDonmeli(string sicil)
        {
            var personeller = string.IsNullOrEmpty(sicil) ? new List<Personeller>() : new List<Personeller> { PersonelSIC001() };
            var context = new MockContextBuilder()
                .WithPersoneller(personeller)
                .Build();

            var servis = new PersonelMerkeziServis(context.Object);
            var model = new PersonelProfilModeli { SicilNo = sicil };

            Assert.False(servis.ProfilGuncelle(model));
        }

        // ─── 9. İZİN TALEBİ TESTLERİ ───

        [Fact]
        public void IzinTalepGonder_GecerliVeri_TrueDonmeli()
        {
            var mockIzinler = new Mock<DbSet<Izinler>>();
            mockIzinler.Setup(m => m.Add(It.IsAny<Izinler>()));

            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller> { PersonelSIC001() })
                .WithSaveChanges(1)
                .Build();
            context.Setup(c => c.Izinlers).Returns(mockIzinler.Object);

            var servis = new PIzinServisi(context.Object);
            var model = new PIzinTalepModeli
            {
                SicilNo = "SIC001",
                IzinTuru = "Yıllık İzin",
                BaslangicTarihi = new DateOnly(2026, 6, 1),
                BitisTarihi = new DateOnly(2026, 6, 5),
                Aciklama = "Yaz tatili"
            };

            Assert.True(servis.IzinTalepGonder(model));
        }

        [Fact]
        public void IzinTalepGonder_OlmayanPersonel_FalseDonmeli()
        {
            var context = new MockContextBuilder()
                .WithPersoneller(new List<Personeller>())
                .Build();

            var servis = new PIzinServisi(context.Object);
            var model = new PIzinTalepModeli { SicilNo = "SIC999" };

            Assert.False(servis.IzinTalepGonder(model));
        }
    }
}