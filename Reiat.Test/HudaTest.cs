using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reiat.Lib;

namespace Reiat.Test
{
    [TestClass]
    public class HudaTest
    {
        [TestMethod]
        public void HitungDiskon_PromoValid_DiskonSesuaiTabel()
        {
            // Arrange
            var kalkulator = new KalkulatorDiskon();

            // Act: 15% dari 100.000 adalah 15.000
            decimal diskon = kalkulator.DapatkanDiskon("REIATBARU", 100000m);

            // Assert
            Assert.AreEqual(15000m, diskon);
        }

        [TestMethod]
        public void HitungDiskon_PromoSalah_HarusThrowException()
        {
            // Arrange
            var kalkulator = new KalkulatorDiskon();

            // Act & Assert: DbC harus melempar error karena promo tidak ada di tabel
            Assert.ThrowsException<ArgumentException>(() => kalkulator.DapatkanDiskon("PROMOASAL", 100000m));
        }

        [TestMethod]
        public void SimpanGenerics_DataValid_BerhasilDisimpan()
        {
            // Arrange (Memakai tipe data string)
            var penyimpananString = new PenyimpananLokal<string>();

            // Act
            penyimpananString.Simpan("Kemeja Reiat");

            // Assert
            Assert.AreEqual(1, penyimpananString.AmbilSemua().Count);
        }

        [TestMethod]
        public void SimpanGenerics_DataNull_HarusThrowException()
        {
            // Arrange (Memakai tipe data object)
            var penyimpananObjek = new PenyimpananLokal<object>();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => penyimpananObjek.Simpan(null));
        }
    }
}