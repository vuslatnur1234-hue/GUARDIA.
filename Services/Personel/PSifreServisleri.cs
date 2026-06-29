using System;
using System.Linq;
using Guardia.API.Data;
using Guardia.API.DTOs.PeGiris;
using BCrypt.Net;

namespace Guardia.API.Services.Personel
{
    public interface IPSifreUnuttumServisi
    {
        (bool Basarili, string Telefon, string SmsKodu) SicilDogrula(string sicilNo);
        bool SmsDogrula(string kod);
        (bool Basarili, string Mesaj) SifreSifirla(PSifreSifirlamaModeli model);

        (bool Basarili, string Mesaj) SifreGuncelle(PSifreGuncellemeModeli model);



    }

    public class PSifreUnuttumServisi : IPSifreUnuttumServisi
    {
        private readonly AppDbContext _context;

        public PSifreUnuttumServisi(AppDbContext context)
        {
            _context = context;
        }

        // SicilDogrula metodunu 
        public (bool Basarili, string Telefon, string SmsKodu) SicilDogrula(string sicilNo)
        {
            var personel = _context.Personellers.FirstOrDefault(p => p.SicilNo == sicilNo);
            if (personel == null) return (false, null, null);

            var girisBilgisi = _context.PersonelGirisBilgileris
                .FirstOrDefault(g => g.SicilNo == sicilNo);
            if (girisBilgisi == null) return (false, null, null);

            int yeniKod = new Random().Next(100000, 999999);
            girisBilgisi.DogrulamaKodu = yeniKod;
            girisBilgisi.KodSonKullanma = DateTimeOffset.Now.AddMinutes(5);
            _context.SaveChanges();

            var tel = personel.Telefon ?? "";
            string maskeliTel = tel.Length >= 10
                ? tel.Substring(0, 4) + " *** ** " + tel.Substring(tel.Length - 2)
                : tel;

            return (true, maskeliTel, yeniKod.ToString());
        }


        public bool SmsDogrula(string kod)
        {
            if (!int.TryParse(kod, out int dogrulamaKodu)) return false;

            var girisBilgisi = _context.PersonelGirisBilgileris.FirstOrDefault(g =>
                g.DogrulamaKodu == dogrulamaKodu &&
                g.KodSonKullanma >= DateTimeOffset.Now);

            return girisBilgisi != null;
        }

        public (bool Basarili, string Mesaj) SifreSifirla(PSifreSifirlamaModeli model)
        {
            if (model.YeniSifre != model.YeniSifreTekrar)
                return (false, "Şifreler eşleşmiyor.");
            var buyukHarfVar = model.YeniSifre.Any(char.IsUpper);
            var rakamVar = model.YeniSifre.Any(char.IsDigit);
            var ozelVar = model.YeniSifre.Any(c => "!@#$%^&*(),.?\":{}|<>".Contains(c));

            if (model.YeniSifre.Length < 8 || !buyukHarfVar || !rakamVar || !ozelVar)
                return (false, "Şifre en az 8 karakter olmalı, büyük harf, rakam ve özel karakter içermelidir.");

            var girisBilgisi = _context.PersonelGirisBilgileris
                .FirstOrDefault(g => g.SicilNo == model.SicilNo);
            if (girisBilgisi == null)
                return (false, "Kullanıcı bulunamadı.");

            girisBilgisi.Sifre = BCrypt.Net.BCrypt.HashPassword(model.YeniSifre);
            girisBilgisi.HataliDenemeSayisi = 0;
            girisBilgisi.HesapKilitliMi = false;
            girisBilgisi.DogrulamaKodu = null;
            girisBilgisi.KodSonKullanma = null;
            _context.SaveChanges();

            return (true, "Şifreniz başarıyla sıfırlandı.");

        }

        public (bool Basarili, string Mesaj) SifreGuncelle(PSifreGuncellemeModeli model)
        {
            if (string.IsNullOrEmpty(model.MevcutSifre) || string.IsNullOrEmpty(model.YeniSifre) || string.IsNullOrEmpty(model.YeniSifreTekrar))
                return (false, "Lütfen tüm alanları doldurunuz.");

            if (model.YeniSifre != model.YeniSifreTekrar)
                return (false, "Yeni şifreler birbirini tutmuyor!");

            var buyukHarfVar = model.YeniSifre.Any(char.IsUpper);
            var rakamVar = model.YeniSifre.Any(char.IsDigit);
            var ozelVar = model.YeniSifre.Any(c => "!@#$%^&*(),.?\":{}|<>".Contains(c));

            if (model.YeniSifre.Length < 8 || !buyukHarfVar || !rakamVar || !ozelVar)
                return (false, "Şifre en az 8 karakter olmalı, büyük harf, rakam ve özel karakter içermelidir.");

            var girisBilgisi = _context.PersonelGirisBilgileris
                .FirstOrDefault(g => g.SicilNo == model.SicilNo);
            if (girisBilgisi == null)
                return (false, "Kullanıcı bulunamadı.");

            if (!BCrypt.Net.BCrypt.Verify(model.MevcutSifre, girisBilgisi.Sifre))
                return (false, "Mevcut şifrenizi yanlış girdiniz.");

            if (BCrypt.Net.BCrypt.Verify(model.YeniSifre, girisBilgisi.Sifre))
                return (false, "Yeni şifre eskiyle aynı olamaz.");

            girisBilgisi.Sifre = BCrypt.Net.BCrypt.HashPassword(model.YeniSifre);
            _context.SaveChanges();

            return (true, "Şifreniz başarıyla güncellendi!");
        }


    }
}