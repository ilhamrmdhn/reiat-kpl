using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reiat.Lib;

namespace Reiat.Test
{
    [TestClass]
    public class AulTest
    {
        [TestMethod]
        public void Automata_AlurNormal_StateBerubahSesuai()
        {
            // Arrange
            var machine = new StatusPesananMachine();

            // Act & Assert
            machine.LanjutKeCheckout();
            Assert.AreEqual(OrderState.Checkout, machine.StateSaatIni);

            machine.ProsesPembayaran();
            Assert.AreEqual(OrderState.Dibayar, machine.StateSaatIni);
        }

        [TestMethod]
        public void Automata_LompatState_HarusThrowException()
        {
            // Arrange
            var machine = new StatusPesananMachine();

            // Act & Assert: Memaksa bayar dari state Keranjang (harus error)
            Assert.ThrowsException<InvalidOperationException>(() => machine.ProsesPembayaran());
        }

        [TestMethod]
        public void Config_HitungPpn_Valid_SesuaiRumus()
        {
            // Arrange
            var config = new KonfigurasiAplikasi();
            decimal nominal = 100000m;

            // Ekspektasi dibuat dinamis membaca dari config yang dimuat
            decimal ekspektasiPpn = nominal * config.Ppn;

            // Act
            decimal hasil = config.HitungPpn(nominal);

            // Assert
            Assert.AreEqual(ekspektasiPpn, hasil);
        }

        [TestMethod]
        public void Config_HitungPpn_Negatif_HarusThrowException()
        {
            // Arrange
            var config = new KonfigurasiAplikasi();

            // Act & Assert: Memaksa hitung minus (harus ditahan oleh DbC)
            Assert.ThrowsException<ArgumentException>(() => config.HitungPpn(-50000m));
        }
    }
}