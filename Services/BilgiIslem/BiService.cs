using Guardia.API.DTOs.BI;

namespace Guardia.API.Services.BilgiIslem
{
    public class BiService : IBiService
    {
        public async Task<List<BiTeknikImkanModeli>> TumImkanlariListeleAsync()
        {
            
            return new List<BiTeknikImkanModeli>
        {
            new BiTeknikImkanModeli { PersonelAdSoyad = "Esma Yılmaz", Departman = "Bilgi İşlem", CihazVeyaYetki = "Sistem Yöneticisi Paneli", BaglantiDurumu = "Aktif", SonErisimTarihi = "08:45" },
            new BiTeknikImkanModeli { PersonelAdSoyad = "Ali Can", Departman = "Satın Alma", CihazVeyaYetki = "ERP Lisansı", BaglantiDurumu = "Pasif", SonErisimTarihi = "Dün" }
        };
        }

        public async Task<List<BiTeknikImkanModeli>> DepartmanBazliImkanlarAsync(string departman)
        {
            var liste = await TumImkanlariListeleAsync();

            
            return liste.Where(x => x.Departman.ToLower().Trim() == departman.ToLower().Trim()).ToList();
        }

        public async Task<int> GetSon24SaatZimmetSayisiAsync()
        {
         

            return 3; 
        }

        public async Task<BiEnvanterOzetiModeli> GetEnvanterOzetiAsync()
        {
          
            return new BiEnvanterOzetiModeli
            {
                ToplamParca = 480,
                YeniEklenen = 12,
                KritikStokOrani = 5.5
            };
        }

        public async Task<List<BiAylikTalepModeli>> GetAylikTalepIstatisikleriAsync()
        {
            
            return new List<BiAylikTalepModeli>
    {
        new BiAylikTalepModeli { AyAdi = "OCAK 2026", Cozulen = 80, Bekleyen = 15 },
        new BiAylikTalepModeli { AyAdi = "ŞUBAT 2026", Cozulen = 89, Bekleyen = 8 },
        new BiAylikTalepModeli { AyAdi = "MART 2026", Cozulen = 90, Bekleyen = 5 },
        new BiAylikTalepModeli { AyAdi = "NİSAN 2026", Cozulen = 95, Bekleyen = 10 }
    };
        }

        public async Task<List<BiGunlukSistemModeli>> GetHaftalikSistemCalismaSuresiAsync()
        {
            const double beklenenDakika = 480.0;

           
            var veriler = new List<(string Gun, double AktifDakika)>
    {
        ("PAZ", 300),
        ("SAL", 350),
        ("ÇAR", 400),
        ("PER", 450),
        ("CUM", 480) 
    };

            return veriler.Select(x =>
            {
                int oran = (int)(x.AktifDakika / beklenenDakika * 100);
                return new BiGunlukSistemModeli
                {
                    GunAdi = x.Gun,
                    AktiflikOrani = oran,
                    Detay = $"{x.Gun}: %{oran}"
                };
            }).ToList();
        }

        public async Task<BiYedeklemeOzetiModeli> GetYedeklemeOzetiAsync()
        {
        
            return new BiYedeklemeOzetiModeli
            {
                YerelSunucuDurum = "AKTİF",
                BulutDolulukOrani = 80,
                HariciYedekDurum = "SENKRONİZE"
            };
        }



     

        public async Task<List<BiEnvanterModeli>> GetTumEnvanterAsync()
        {
            
            return new List<BiEnvanterModeli>
    {
        new BiEnvanterModeli { Id = 1, CihazTuru = "Bilgisayar", CihazModeli = "MacBook Pro M3", SeriNo = "SN123", Durum = "Kullanımda", ZimmetliKisi = "Esma Çelik" },
        new BiEnvanterModeli { Id = 2, CihazTuru = "Monitör", CihazModeli = "Dell UltraSharp 27", SeriNo = "SN456", Durum = "Stokta", ZimmetliKisi = "Ahmet Demir" }
    };
        }

        public async Task<bool> EnvanterEkleAsync(BiEnvanterModeli yeniCihaz)
        {
            
            return true;
        }

        public async Task<bool> EnvanterGuncelleAsync(int id, BiEnvanterModeli guncelCihaz)
        {
           
            return true;
        }

        public async Task<bool> EnvanterSilAsync(int id)
        {
           
            return true;
        }
    


    public async Task<BiEnvanterModeli> GetByIdAsync(int id)
        {
            var liste = await GetTumEnvanterAsync();
            return liste.FirstOrDefault(x => x.Id == id);
        }

    }
}
