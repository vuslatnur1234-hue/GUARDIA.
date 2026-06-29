using Guardia.API.Data;
using BCrypt.Net;
using Guardia.API.DTOs.DeGiris;
using Guardia.API.DTOs; // ServiceResult için eklendi

namespace Guardia.API.Services.AuthDepartman
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JWTService _jwt;

        public AuthService(AppDbContext db, JWTService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        private static readonly Dictionary<string, string> _sayfaMap =
    new(StringComparer.OrdinalIgnoreCase)
    {
        { "İnsan Kaynakları", "ik-index.html" },
        { "insan Kaynakları", "ik-index.html" }, 
        { "Bilgi İşlem",      "bi-index.html" },
        { "bilgi işlem",      "bi-index.html" },
        { "Hukuk",            "hk-index.html" },
        { "Satın Alma",       "sa-index.html" },
        { "İdari İşler",      "ii-index.html" }
    };

        private static readonly Dictionary<string, string> _prefixMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
        { "İnsan Kaynakları", "IK" },
        { "insan Kaynakları", "IK" },
        { "Bilgi İşlem",      "BI" },
        { "bilgi işlem",      "BI" },
        { "Hukuk",            "HK" },
        { "Satın Alma",       "SA" },
        { "İdari İşler",      "II" }
            };

        // GİRİŞ YAP
        public ServiceResult GirisYap(GirisBilgisi bilgi)
        {
            var admin = _db.Adminler
                .FirstOrDefault(a => a.admin_no.Trim() == bilgi.adminId.Trim());

            if (admin == null)
            {
                return ServiceResult.Failure("Kullanıcı bulunamadı.");
            }

            string dbSifre = admin.sifre.Trim();
            string gelenSifre = bilgi.adminPass.Trim();

            bool sifreDogru = false;
            bool eskiSifreMi = false;

            if (dbSifre.StartsWith("$2a$") || dbSifre.StartsWith("$2b$"))
            {
                sifreDogru = BCrypt.Net.BCrypt.Verify(gelenSifre, dbSifre);
            }
            else
            {
                sifreDogru = dbSifre == gelenSifre;
                eskiSifreMi = true; // Şifre düz metin formatında
            }

            if (!sifreDogru)
            {
                return ServiceResult.Failure("Hatalı şifre girdiniz.");
            }

           
            if (eskiSifreMi)
            {
                admin.sifre = BCrypt.Net.BCrypt.HashPassword(gelenSifre);
                _db.SaveChanges();
                Console.WriteLine($"[Sistem] {admin.admin_no} sicilli yöneticinin şifresi BCrypt formatına güncellendi.");
            }

            string departmanKey = admin.departman.Trim();
            if (!_sayfaMap.TryGetValue(departmanKey, out var hedefSayfa))
            {
                return ServiceResult.Failure("Kullanıcının departman bilgisi sistemdeki sayfalarla eşleşmiyor.");
            }

            if (_prefixMap.TryGetValue(departmanKey, out var beklenenPrefix))
            {
                if (!admin.admin_no.Trim().StartsWith(beklenenPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult.Failure($"Sicil numaranız ait olduğunuz departman ({departmanKey}) ile uyuşmuyor.");
                }
            }

            var token = _jwt.AdminTokenUret(admin);

            return ServiceResult.Success(new
            {
                token,
                hedefSayfa,
                departmanAdi = departmanKey,
                adSoyad = admin.ad_soyad
            });
        }

        // KOD GÖNDER
        public ServiceResult KodGonder(SifreSifirlamaBilgisi bilgi)
        {
            if (!bilgi.Email.EndsWith("@guardia.com"))
                return ServiceResult.Failure("Sadece şirket maili (@guardia.com) kullanabilirsiniz.");

            var admin = _db.Adminler
                .FirstOrDefault(a => a.admin_no == bilgi.SicilNo && a.email == bilgi.Email);

            if (admin == null)
                return ServiceResult.Failure("Sicil numarası veya e-posta adresi eşleşmiyor.");

            var kod = new Random().Next(100000, 999999).ToString();

            admin.dogrulama_kodu = kod;

            admin.kod_son_kullanma = DateTime.UtcNow.AddMinutes(10);

            _db.SaveChanges();

            Console.WriteLine($"[Doğrulama Kodu] {bilgi.Email} → {kod}");

            return ServiceResult.Success(new { mesaj = "Kod gönderildi.", uretilenDogrulamaKodu = kod });
        }

        // KODU DOĞRULA
        public ServiceResult KoduDogrula(KodDogrulamaBilgisi bilgi)
        {
            var admin = _db.Adminler
    .FirstOrDefault(a => a.admin_no == bilgi.SicilNo && a.email == bilgi.Email);

            if (admin == null)
                return ServiceResult.Failure("Kullanıcı bulunamadı.");

            if (string.IsNullOrEmpty(admin.dogrulama_kodu) || admin.kod_son_kullanma == null)
                return ServiceResult.Failure("Geçerli bir doğrulama kodu bulunamadı.");

        
            if (DateTime.UtcNow > admin.kod_son_kullanma)
            {
                admin.dogrulama_kodu = null;
                admin.kod_son_kullanma = null;
                _db.SaveChanges();
                return ServiceResult.Failure("Doğrulama kodunun süresi dolmuş.");
            }

            if (admin.dogrulama_kodu != bilgi.GirilenKod)
                return ServiceResult.Failure("Girdiğiniz kod hatalı.");

            return ServiceResult.Success(new { mesaj = "Kod doğrulandı." });
        }

        // ŞİFREYİ GÜNCELLE
        public ServiceResult SifreyiGuncelle(YeniSifreBilgisi bilgi)
        {
            if (bilgi.YeniSifre != bilgi.YeniSifreTekrar)
                return ServiceResult.Failure("Şifreler eşleşmiyor.");

            var harfVar = bilgi.YeniSifre.Any(char.IsLetter);
            var rakamVar = bilgi.YeniSifre.Any(char.IsDigit);
            var ozelVar = bilgi.YeniSifre.Any(c => "!@#$%^&*(),.?\":{}|<>".Contains(c));

            if (bilgi.YeniSifre.Length < 8 || !harfVar || !rakamVar || !ozelVar)
                return ServiceResult.Failure("Şifre en az 8 karakter olmalı, harf, rakam ve özel karakter içermelidir.");

            var admin = _db.Adminler
     .FirstOrDefault(a => a.admin_no == bilgi.SicilNo && a.email == bilgi.Email);

            if (admin == null)
                return ServiceResult.Failure("Kullanıcı bulunamadı.");

            admin.sifre = BCrypt.Net.BCrypt.HashPassword(bilgi.YeniSifre);
            admin.dogrulama_kodu = null;
            admin.kod_son_kullanma = null;

            _db.SaveChanges();

            return ServiceResult.Success(new { mesaj = "Şifre başarıyla güncellendi." });
        }
    }
}