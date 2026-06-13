using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reiat.Lib;

namespace Reiat.Test
{
    [TestClass]
    public class KeranjangBelanjaTest
    {
        [TestMethod]
        public void TambahProduk_InputValid_TotalHargaSesuai()
        {
            // Arrange
            var keranjang = new KeranjangBelanja();
            var baju = new PakaianFisik("Kemeja Reiat", 150000, "L", 250);
            var pola = new PolaDigital("Pola Celana", 45000, "PDF", "reiat.com/pola");

            // Act
            keranjang.TambahProduk(baju);
            keranjang.TambahProduk(pola);

            // Assert: Memastikan fungsi keranjang berjalan sesuai ekspektasi 
            Assert.AreEqual(2, keranjang.LihatKeranjang().Count);
            Assert.AreEqual(195000m, keranjang.HitungTotalHarga());
        }

        [TestMethod]
        public void TambahProduk_InputNull_HarusThrowException()
        {
            // Arrange
            var keranjang = new KeranjangBelanja();

            // Act & Assert: Memastikan DbC berjalan dan melempar error yang tepat
            Assert.ThrowsException<ArgumentNullException>(() => keranjang.TambahProduk(null));
        }
    }
}