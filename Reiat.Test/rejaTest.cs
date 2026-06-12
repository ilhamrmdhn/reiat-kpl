using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reiat.Lib;

namespace Reiat.Test
{
    [TestClass]
    public class RejaTest
    {
        [TestMethod]
        public void ResponAPI_SuksesDenganData_BerhasilDibuat()
        {
            // Arrange & Act (Memasukkan tipe data String ke T)
            var respon = new ResponAPI<string>(true, "OK", "Katalog Kemeja");

            // Assert
            Assert.IsTrue(respon.Sukses);
            Assert.AreEqual("Katalog Kemeja", respon.Data);
        }

        [TestMethod]
        public void ResponAPI_SuksesTanpaData_HarusThrowException()
        {
            // Act & Assert: DbC mencegah balikan sukses tapi datanya null
            Assert.ThrowsException<ArgumentNullException>(() => new ResponAPI<string>(true, "OK", null));
        }

        [TestMethod]
        public void ResponAPI_GagalTanpaPesan_HarusThrowException()
        {
            // Act & Assert: DbC mencegah balikan gagal yang pesannya kosong
            Assert.ThrowsException<ArgumentException>(() => new ResponAPI<string>(false, "", null));
        }
    }
}