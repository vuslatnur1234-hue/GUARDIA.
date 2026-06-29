using Guardia.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Guardia.API.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Arizalar> Arizalars { get; set; }
    public virtual DbSet<Bildirimler> Bildirimlers { get; set; }
    public virtual DbSet<Admin> Adminler { get; set; }
    public virtual DbSet<Bordrolar> Bordrolars { get; set; }

    public virtual DbSet<Izinler> Izinlers { get; set; }

    public virtual DbSet<Maaslar> Maaslars { get; set; }

    public virtual DbSet<Personeller> Personellers { get; set; }

    public virtual DbSet<PersonelGecis> PersonelGecisler { get; set; }

    // DbSet'in yanına ekleyin 
    public virtual DbSet<PersonelGirisBilgileri> PersonelGirisBilgileris { get; set; }
    public virtual DbSet<Talepler> Taleplers { get; set; }

    public virtual DbSet<YemekMenusu> YemekMenusus { get; set; }

    public virtual DbSet<Duyurular> Duyurulars { get; set; }//ik duyurular için

    public DbSet<Arsiv> Arsiv { get; set; }
    public DbSet<Davalar> Davalars { get; set; }
    public virtual DbSet<Mevzuatlar> Mevzuatlar { get; set; }
    public virtual DbSet<Sozlesmeler> Sozlesmeler { get; set; }
    public virtual DbSet<HkHatirlaticilar> HkHatirlaticilars { get; set; }
   
    public DbSet<Mesajlar> Mesajlars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<PersonelGecis>(entity =>
        {
            entity.ToTable("personel_gecisler");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");
            entity.Property(e => e.SicilNo).HasColumnName("sicil_no");
            entity.Property(e => e.GecisZamani).HasColumnName("gecis_zamani");
            entity.Property(e => e.GecisYonu).HasColumnName("gecis_yonu");
            entity.HasOne(e => e.Personel)  
                .WithMany()
                .HasForeignKey(e => e.PersonelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("admin");
            entity.HasKey(e => e.id);

            entity.Property(e => e.id)
                  .HasColumnName("id")
                  .HasColumnType("bigint");          

            entity.Property(e => e.admin_no)
                  .HasColumnName("admin_no")
                  .HasColumnType("nvarchar(20)");    

            entity.Property(e => e.sifre).HasColumnName("sifre");
            entity.Property(e => e.ad_soyad).HasColumnName("ad_soyad");
            entity.Property(e => e.departman).HasColumnName("departman");
            entity.Property(e => e.yetki_seviyesi).HasColumnName("yetki_seviyesi");
            entity.Property(e => e.email).HasColumnName("email");
            entity.Property(e => e.dogrulama_kodu)
         .HasColumnName("dogrulama_kodu")
         .HasColumnType("nvarchar(10)");
            entity.Property(e => e.kod_son_kullanma).HasColumnName("kod_son_kullanma");
        });
        modelBuilder.Entity<Bildirimler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bildirimler_pkey");

            entity.ToTable("bildirimler");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Baslik)
                .HasMaxLength(255)
                .HasColumnName("baslik");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.Icon)
                .HasMaxLength(100)
                .HasColumnName("icon");
            entity.Property(e => e.Mesaj).HasColumnName("mesaj");
            entity.Property(e => e.Okundu)
                .HasDefaultValue(false)
                .HasColumnName("okundu");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");
            entity.Property(e => e.Tarih)
                .HasColumnName("tarih"); 

            entity.HasOne(d => d.Personel).WithMany(p => p.Bildirimlers)
                .HasForeignKey(d => d.PersonelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bildirimler_personel_id_fkey");
        });

        modelBuilder.Entity<Bordrolar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bordrolar_pkey");

            entity.ToTable("bordrolar");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AyYil)
                .HasMaxLength(50)
                .HasColumnName("ay_yil");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.Donem)
                .HasMaxLength(50)
                .HasColumnName("donem");
            entity.Property(e => e.DosyaUrl).HasColumnName("dosya_url");
            entity.Property(e => e.Durum)
                .HasMaxLength(100)
                .HasColumnName("durum");
            entity.Property(e => e.NetOdeme)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("net_odeme");
            entity.Property(e => e.OdemeTarihi).HasColumnName("odeme_tarihi");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.YeniMi)
                .HasDefaultValue(true)
                .HasColumnName("yeni_mi");

            entity.HasOne(d => d.Personel).WithMany(p => p.Bordrolars)
                .HasForeignKey(d => d.PersonelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bordrolar_personel_id_fkey");
        });

        modelBuilder.Entity<Izinler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("izinler_pkey");

            entity.ToTable("izinler");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aciklama).HasColumnName("aciklama");
            entity.Property(e => e.BaslangicTarihi).HasColumnName("baslangic_tarihi");
            entity.Property(e => e.BitisTarihi).HasColumnName("bitis_tarihi");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.IzinTuru)
                .HasMaxLength(100)
                .HasColumnName("izin_turu");
            entity.Property(e => e.OnayDurumu)
                .HasMaxLength(50)
                .HasDefaultValue("BEKLEMEDE")
                .HasColumnName("onay_durumu");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");

            entity.HasOne(d => d.Personel).WithMany(p => p.Izinlers)
                .HasForeignKey(d => d.PersonelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("izinler_personel_id_fkey");
        });

        modelBuilder.Entity<Maaslar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("maaslar_pkey");

            entity.ToTable("maaslar");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.Donem)
                .HasMaxLength(50)
                .HasColumnName("donem");
            entity.Property(e => e.Durum)
                .HasMaxLength(50)
                .HasDefaultValue("ÖDENMEDİ")
                .HasColumnName("durum");
            entity.Property(e => e.Kesinti)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("kesinti");
            entity.Property(e => e.NetMaas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("net_maas");
            entity.Property(e => e.OdemeTarihi).HasColumnName("odeme_tarihi");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");
            entity.Property(e => e.Prim)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("prim");
            entity.Property(e => e.SgkPayi)
    .HasDefaultValue(0m)
    .HasColumnType("decimal(18, 2)")
    .HasColumnName("sgk_payi"); 

            entity.Property(e => e.GelirVergisi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gelir_vergisi"); 

            entity.HasOne(d => d.Personel).WithMany(p => p.Maaslars)
                .HasForeignKey(d => d.PersonelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("maaslar_personel_id_fkey");
        });



        modelBuilder.Entity<Personeller>(entity =>
        {
            entity.ToTable("personeller"); 

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdSoyad).HasColumnName("ad_soyad");
            entity.Property(e => e.SicilNo).HasColumnName("sicil_no");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Telefon).HasColumnName("telefon");
            entity.Property(e => e.Unvan).HasColumnName("unvan");
            entity.Property(e => e.VardiyaGrubu).HasColumnName("vardiya_grubu");
            entity.Property(e => e.Departman).HasColumnName("departman");
            entity.Property(e => e.Iban).HasColumnName("iban");
            entity.Property(e => e.BankaAdi).HasColumnName("banka_adi");
            entity.Property(e => e.HesapNumarasi).HasColumnName("hesap_numarasi");
            entity.Property(e => e.Adres).HasColumnName("adres");
            entity.Property(e => e.KanGrubu).HasColumnName("kan_grubu");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.IkGuncellemeDurumu).HasColumnName("ik_guncelleme_durumu");
            entity.Property(e => e.AcilDurumYakini).HasColumnName("acil_durum_yakini");
            entity.Property(e => e.AcilDurumNo).HasColumnName("acil_durum_no");
            entity.Property(e => e.IseGirisTarihi).HasColumnName("ise_giris_tarihi");
            entity.Property(e => e.IzinBakiyesi).HasColumnName("izin_bakiyesi");
            entity.Property(e => e.MesaiBaslangic).HasColumnName("mesai_baslangic");
            entity.Property(e => e.QrKodData).HasColumnName("qr_kod_data");
            entity.Property(e => e.YeniAdresTalebi).HasColumnName("yeni_adres_talebi");
            entity.Property(e => e.YeniTelefonTalebi).HasColumnName("yeni_telefon_talebi");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Durum).HasColumnName("Durum").HasDefaultValue(true);
            entity.Property(e => e.AyrilmaTarihi).HasColumnName("AyrilmaTarihi");
            entity.Property(e => e.DogumTarihi).HasColumnName("dogum_tarihi");
            entity.Property(e => e.TcNo).HasColumnName("tc_no");
            entity.Property(e => e.MedeniDurum).HasColumnName("medeni_durum");
            entity.Property(e => e.YakinlikDerecesi).HasColumnName("yakinlik_derecesi");
            entity.Property(e => e.MaasMiktari).HasColumnName("maas_miktari");
            entity.Property(e => e.AskerlikDurumu).HasColumnName("askerlik_durumu");
            entity.HasQueryFilter(p => p.Durum == true);
            entity.Ignore(e => e.Sifre);
            entity.Ignore(e => e.DogrulamaKodu);
            entity.Ignore(e => e.KodSonKullanma);
            entity.Ignore(e => e.EskiSifreler);
            entity.Property(e => e.DogrulamaKodu).HasColumnName("dogrulama_kodu");
            entity.Property(e => e.KodSonKullanma).HasColumnName("kod_son_kullanma");
        });


        modelBuilder.Entity<PersonelGirisBilgileri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("personel_giris_bilgileri_pkey");

            entity.ToTable("personel_giris_bilgileri");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");
            entity.Property(e => e.SicilNo)
                .HasMaxLength(50)
                .HasColumnName("sicil_no");
            entity.Property(e => e.Sifre).HasColumnName("sifre");
            entity.Property(e => e.HataliDenemeSayisi)
                .HasDefaultValue(0)
                .HasColumnName("hatali_deneme_sayisi");
            entity.Property(e => e.HesapKilitliMi)
                .HasDefaultValue(false)
                .HasColumnName("hesap_kilitli_mi");

            entity.HasOne(d => d.Personel)
                .WithMany()
                .HasForeignKey(d => d.PersonelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("personel_giris_bilgileri_personel_id_fkey");
        });

        modelBuilder.Entity<Arizalar>(entity =>
        {
            entity.ToTable("arizalar");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");
            entity.Property(e => e.Baslik).HasColumnName("baslik");
            entity.Property(e => e.Kategori).HasColumnName("kategori");
            entity.Property(e => e.Oncelik).HasColumnName("oncelik");
            entity.Property(e => e.Aciklama).HasColumnName("aciklama");
            entity.Property(e => e.AtananKisi).HasColumnName("atanan_kisi");
            entity.Property(e => e.Durum).HasColumnName("durum");
            entity.Property(e => e.Lokasyon).HasColumnName("lokasyon");
            entity.Property(e => e.TakipNo).HasColumnName("takip_no");
            entity.Property(e => e.CreatedAt)
        .HasColumnName("created_at")
        .HasDefaultValueSql("(sysdatetimeoffset())"); 

            entity.HasOne(d => d.Personel).WithMany()
                .HasForeignKey(d => d.PersonelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Talepler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("talepler_pkey");

            entity.ToTable("talepler");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aciklama).HasColumnName("aciklama");
            entity.Property(e => e.BaslangicTarihi).HasColumnName("baslangic_tarihi");
            entity.Property(e => e.BitisTarihi).HasColumnName("bitis_tarihi");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("created_at");
            entity.Property(e => e.OnayDurumu)
                .HasMaxLength(50)
                .HasColumnName("onay_durumu");
            entity.Property(e => e.OnayTarihi).HasColumnName("onay_tarihi");
            entity.Property(e => e.OnaylayanId).HasColumnName("onaylayan_id");
            entity.Property(e => e.PersonelId).HasColumnName("personel_id");
            entity.Property(e => e.RedNedeni).HasColumnName("red_nedeni");
            entity.Property(e => e.TalepTipi)
                .HasMaxLength(100)
                .HasColumnName("talep_tipi");
            entity.Property(e => e.ToplamGun).HasColumnName("toplam_gun");


            entity.HasOne(d => d.Personel).WithMany(p => p.Taleplers)
                .HasForeignKey(d => d.PersonelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("talepler_personel_id_fkey");
        });

        modelBuilder.Entity<YemekMenusu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("yemek_menusu_pkey");

            entity.ToTable("yemek_menusu");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnaYemek)
                .HasMaxLength(255)
                .HasColumnName("ana_yemek");
            entity.Property(e => e.Corba)
                .HasMaxLength(255)
                .HasColumnName("corba");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Gun)
                .HasMaxLength(50)
                .HasColumnName("gun");
            entity.Property(e => e.IcecekTatli)
                .HasMaxLength(255)
                .HasColumnName("icecek_tatli");
            entity.Property(e => e.Kalori)
                .HasMaxLength(50)
                .HasColumnName("kalori");
            entity.Property(e => e.Tarih)
                .HasMaxLength(50)
                .HasColumnName("tarih");
            entity.Property(e => e.YanUrun)
                .HasMaxLength(255)
                .HasColumnName("yan_urun");
        });



        modelBuilder.Entity<Duyurular>(entity =>
        {
            entity.ToTable("ik_duyurular"); 

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Baslik).HasColumnName("baslik");
            entity.Property(e => e.Icerik).HasColumnName("icerik");
            entity.Property(e => e.YayinlayanId).HasColumnName("yayinlayan_id");
            entity.Property(e => e.Kategori).HasColumnName("kategori");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.HedefKitle).HasColumnName("hedef_kitle");
            entity.Property(e => e.DosyaUrl).HasColumnName("dosya_url");
            entity.Property(e => e.BildirimGonder).HasColumnName("bildirim_gonder");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });


        modelBuilder.Entity<Arsiv>(entity =>
        {
            entity.ToTable("arsiv"); 
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("bigint");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.DosyaAdi).HasColumnName("dosya_adi");
            entity.Property(e => e.Kategori).HasColumnName("kategori");
            entity.Property(e => e.YuklemeTarihi).HasColumnName("yukleme_tarihi").HasColumnType("date");
            entity.Property(e => e.ImhaTarihi).HasColumnName("imha_tarihi").HasColumnType("date");
            entity.Property(e => e.Durum).HasColumnName("durum");
            entity.Property(e => e.DosyaUrl).HasColumnName("dosya_url");
            entity.Property(e => e.OlusturanId).HasColumnName("olusturan_id").HasColumnType("bigint");
        });

        modelBuilder.Entity<Davalar>(entity =>
        {
            entity.ToTable("davalar");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("bigint");

          
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired(false);
            entity.Property(e => e.OlusturanId).HasColumnName("olusturan_id").HasColumnType("bigint").IsRequired(false);

            entity.Property(e => e.DosyaNo).HasColumnName("dava_no");
            entity.Property(e => e.KarsiTaraf).HasColumnName("taraf");
            entity.Property(e => e.Konu).HasColumnName("konu");
            entity.Property(e => e.Asama).HasColumnName("asama");
            entity.Property(e => e.Durum).HasColumnName("durum");
            entity.Property(e => e.DosyaUrl).HasColumnName("dosya_url");
            entity.Property(e => e.Mahkeme).HasColumnName("mahkeme");
            entity.Property(e => e.YoneticiNotu).HasColumnName("yonetici_notu");
            entity.Property(e => e.DurusmaTarihi).HasColumnName("durusma_tarihi").HasColumnType("datetimeoffset");
        });

        modelBuilder.Entity<Mevzuatlar>(entity =>
        {
            entity.ToTable("mevzuatlar");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("bigint");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Baslik).HasColumnName("baslik");
            entity.Property(e => e.Kategori).HasColumnName("kategori");
            entity.Property(e => e.Ozet).HasColumnName("ozet");
            entity.Property(e => e.OnemDerecesi).HasColumnName("oncelik"); 
            entity.Property(e => e.DosyaUrl).HasColumnName("dosya_url");
            entity.Property(e => e.OlusturanId).HasColumnName("olusturan_id").HasColumnType("bigint");

           
            entity.Property(e => e.YayinTarihi).HasColumnName("yayin_tarihi").HasColumnType("date");
        });

        modelBuilder.Entity<Sozlesmeler>(entity =>
        {
            entity.ToTable("sozlesmeler");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("bigint"); 

            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Taraf).HasColumnName("taraf");
            entity.Property(e => e.Tur).HasColumnName("tur");
            entity.Property(e => e.BaslangicTarihi)
        .HasColumnName("baslangic_tarihi")
        .HasColumnType("datetime"); 

            entity.Property(e => e.BitisTarihi)
                  .HasColumnName("bitis_tarihi")
                  .HasColumnType("datetime"); 
            entity.Property(e => e.Durum).HasColumnName("durum");
            entity.Property(e => e.DosyaUrl).HasColumnName("dosya_url");

            entity.Property(e => e.OlusturanId)
                  .HasColumnName("olusturan_id")
                  .HasColumnType("bigint"); 
        });

        modelBuilder.Entity<HkHatirlaticilar>(entity =>
        {
            entity.ToTable("hk_hatirlaticilar"); 
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Icerik).HasColumnName("icerik");
            entity.Property(e => e.TarihEtiketi).HasColumnName("tarih_etiketi"); 
            entity.Property(e => e.IsTamamlandi).HasColumnName("is_tamamlandi");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("(sysdatetimeoffset())");

           
        });

        modelBuilder.Entity<Mesajlar>(entity =>
        {
         
            entity.ToTable("hk_mesajlar");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("bigint");
            entity.Property(e => e.GonderenBirim).HasColumnName("gonderen_birim").IsRequired();
            entity.Property(e => e.AliciBirim).HasColumnName("alici_birim").IsRequired();
            entity.Property(e => e.MesajIcerigi).HasColumnName("mesaj_icerigi").IsRequired();
            entity.Property(e => e.GonderimSaati).HasColumnName("gonderim_saati").IsRequired();
            entity.Property(e => e.OkunduMu).HasColumnName("okundu_mu");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
        OnModelCreatingPartial(modelBuilder);
    }





    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}