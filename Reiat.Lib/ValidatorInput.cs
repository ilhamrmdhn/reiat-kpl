using System;

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

            if (!email.Contains("@") || !email.Contains("."))
            {
                throw new ArgumentException("Format email tidak valid! Harus mengandung karakter '@' dan domain.");
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