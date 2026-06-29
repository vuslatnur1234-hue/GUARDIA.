using Xunit;

namespace Guardia.Tests
{
    public class IsinmaTesti
    {
        [Fact]
        public void ToplamaIslemi_DogruSonucVermeli()
        {
            // Arrange (Hazırlık)
            int sayi1 = 5;
            int sayi2 = 5;

            // Act (Eylem)
            int sonuc = sayi1 + sayi2;

            // Assert (Doğrulama)
            Assert.Equal(10, sonuc); // 10, beklediğimiz sonuçtur
        }
    }
}