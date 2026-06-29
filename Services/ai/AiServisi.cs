using Guardia.API.DTOs;
using Guardia.API.Data;
using System.Text;
using System.Text.Json;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Globalization;

namespace Guardia.API.Services.ai
{
    public class AiServisi : IAiServisi
    {
        private readonly string _mistralKey = "gsCvXAufMF1ef0f7TrZnSMNXiUOvJWEg";
        private readonly AppDbContext _context;

        public AiServisi(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CevapModeli> SoruCevapla(SoruModeli istek)
        {
            if (istek == null || string.IsNullOrEmpty(istek.Soru))
                return new CevapModeli { Cevap = "Lütfen bir soru yazın." };

            try
            {
                var personel = _context.Personellers.FirstOrDefault(p => p.SicilNo == istek.SicilNo);

                if (personel == null)
                    return new CevapModeli { Cevap = "Personel kaydı bulunamadı, lütfen sicil numaranızı kontrol edin." };

                var sonMaas = _context.Maaslars
                    .Where(m => m.PersonelId == personel.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                string maasBilgisi = (sonMaas?.Durum == "ÖDENDİ")
                    ? "Son maaşınız başarıyla yatırılmıştır."
                    : "Maaş ödemeleri her ayın 15'inde yapılmaktadır.";

                string bugunGun = DateTime.Now.ToString("dddd", new CultureInfo("tr-TR"));
                var yemek = _context.YemekMenusus.FirstOrDefault(y => y.Gun == bugunGun);

                string menuMesaji = (yemek != null)
                    ? $"Bugün menüde {yemek.Corba}, {yemek.AnaYemek}, {yemek.YanUrun} ve {yemek.IcecekTatli} var."
                    : "Bugün için henüz yemek menüsü girilmemiş.";
               
                string systemPrompt = $@"Sen Guardia şirketinin akıllı asistanısın. 
                SADECE aşağıdaki veritabanı bilgilerini kullanarak kullanıcıya yanıt ver:
                - Kullanıcı Adı: {personel.AdSoyad}
                - Vardiya Bilgisi: {personel.VardiyaGrubu} grubunda çalışmaktasınız.
                - İzin Bakiyesi: Toplam {personel.IzinBakiyesi} gün izin hakkınız bulunmaktadır.
                - Maaş Durumu: {maasBilgisi}
                - Yemek Menüsü: {menuMesaji}
                - Arıza Bildirimi: Teknik arızalar için 444 0 444 numarasını arayınız.

                KRİTİK KURALLAR:
                1. Sadece kullanıcının sorusuyla ilgili bilgiyi ver.
                2. Yemek sormuyorsa yemekten, vardiya sormuyorsa vardiyadan asla bahsetme.
                3. Cevapların çok nazik ama kısa (en fazla 1-2 cümle) olsun.";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_mistralKey}");

                var payload = new
                {
                    model = "open-mistral-7b",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = istek.Soru }
                    },
                    temperature = 0.2
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.mistral.ai/v1/chat/completions", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var hataMesaji = await response.Content.ReadAsStringAsync();
                    return new CevapModeli { Cevap = "Yapay zeka şu an yanıt veremiyor, lütfen teknik destekle iletişime geçin." };
                }

                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);
                string aiCevap = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "Yanıt oluşturulamadı.";

                return new CevapModeli { Cevap = aiCevap.Trim() };
            }
            catch (Exception ex)
            {
                return new CevapModeli { Cevap = "Bir bağlantı hatası oluştu. Lütfen daha sonra tekrar deneyiniz." };
            }
        }
    }
}
