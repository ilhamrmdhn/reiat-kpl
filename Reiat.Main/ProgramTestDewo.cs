using System;
using System.Diagnostics;
using Reiat.Lib;

namespace Reiat.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== [DEMO] E-COMMERCE REIAT ===");

            var authMachine = new AutentikasiMachine();
            Console.WriteLine($"Status Awal: {authMachine.StateSaatIni}");

            try
            {
                // Demo Teknik Automata
                Console.WriteLine("\n> User mencoba checkout...");
                authMachine.TriggerLogin();
                Console.WriteLine($"-> Sistem meminta login. Status berubah: {authMachine.StateSaatIni}");

                // Demo Teknik Code Reuse & DbC
                Console.WriteLine("\n[Proses Autentikasi]");
                Console.Write("Masukkan Email: ");
                string emailInput = "saleh.com"; 
                Console.WriteLine(emailInput);

                Console.WriteLine("> Memvalidasi email...");
                ValidatorInput.ValidasiEmail(emailInput); // 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[GAGAL LOGIN]: {ex.Message}");
            }

            try
            {
                // Simulasi perbaikan input
                Console.WriteLine("\n> Mengulang input email yang benar...");
                ValidatorInput.ValidasiEmail("dewo@telkom.edu");
                Console.WriteLine("[Validasi Lolos] Email benar.");

                authMachine.SuksesLogin();
                Console.WriteLine($"-> Login Sukses! Status berubah: {authMachine.StateSaatIni}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Menjalankan Performance Testing
            Console.WriteLine("\n=== MENJALANKAN PERFORMANCE TESTING ===");
            JalankanPerformanceTest();
        }

        static void JalankanPerformanceTest()
        {
            var stopwatch = new Stopwatch();

            Console.WriteLine("Menjalankan validasi format email stress test");

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                // Eksekusi berulang fungsi statis untuk mengukur beban string manipulation
                ValidatorInput.ValidasiEmail("test@domain.com");
            }
            stopwatch.Stop();

            Console.WriteLine("Validasi Selesai.");
            Console.WriteLine($"Waktu Eksekusi (Performance): {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}