using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reiat.Lib;

namespace Reiat.Test
{
    [TestClass]
    public class DewoTest
    {
        [TestMethod]
        public void Automata_AlurNormal_StateBerubahSesuai()
        {
            // Arrange
            var auth = new AutentikasiMachine();

            // Act & Assert
            auth.TriggerLogin();
            Assert.AreEqual(AuthState.Authenticating, auth.StateSaatIni);

            auth.SuksesLogin();
            Assert.AreEqual(AuthState.Customer, auth.StateSaatIni);
        }

        [TestMethod]
        public void Automata_LompatState_HarusThrowException()
        {
            // Arrange
            var auth = new AutentikasiMachine();

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => auth.SuksesLogin());
        }

        [TestMethod]
        public void ValidasiEmail_FormatSalah_HarusThrowException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => ValidatorInput.ValidasiEmail("dewotelkom.edu"));
        }

        [TestMethod]
        public void ValidasiEmail_FormatBenar_TidakError()
        {
            try
            {
                ValidatorInput.ValidasiEmail("dewo@telkom.edu");
                // Jika lolos sampai sini, test berhasil
            }
            catch (Exception ex)
            {
                Assert.Fail($"Seharusnya tidak throw exception, tapi mendapat: {ex.Message}");
            }
        }
    }
}