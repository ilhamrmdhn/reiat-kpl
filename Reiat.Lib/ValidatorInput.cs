using System;
using System.Text.RegularExpressions;

namespace Reiat.Lib
{
    // Teknik Code Reuse
    public static class ValidatorInput
    {
        public static void ValidasiEmail(string email)
        {
            // Pre-condition
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email tidak boleh kosong.");
            }

            // Upgrade: Validasi menggunakan Regex standar industri
            string polaEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, polaEmail))
            {
                throw new ArgumentException("Format email tidak valid! Pastikan formatnya benar (contoh: user@domain.com).");
            }
        }

        public static void ValidasiPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentException("Password harus diisi dan minimal 6 karakter.");
            }
        }
    }
}