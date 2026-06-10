using System;
using System.Diagnostics;
using Reiat.Lib;

namespace Reiat.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== [DEMO AUL] E-COMMERCE REIAT ===");

            var config = new KonfigurasiAplikasi();
            Console.WriteLine($"[Config] PPN saat ini dibaca sebesar: {config.Ppn * 100}%\n");

            var machine = new StatusPesananMachine();
            Console.WriteLine($"Status Awal: {machine.StateSaatIni}");

            try
            {
                // Simulasi pergerakan state Automata
                machine.LanjutKeCheckout();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");

                // Simulasi kalkulasi Runtime Config
                decimal belanjaan = 150000m;
                decimal pajak = config.HitungPpn(belanjaan);

                Console.WriteLine($"\n[Kalkulasi]");
                Console.WriteLine($"Tagihan Barang : Rp {belanjaan:N0}");
                Console.WriteLine($"PPN (11%)      : Rp {pajak:N0}");
                Console.WriteLine($"Total Tagihan  : Rp {belanjaan + pajak:N0}\n");

                machine.ProsesPembayaran();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");

                machine.SelesaikanPesanan();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terjadi Kesalahan (DbC Aktif): {ex.Message}");
            }

            // Menjalankan Performance Testing
            Console.WriteLine("\n=== MENJALANKAN PERFORMANCE TESTING ===");
            JalankanPerformanceTest();
        }

        static void JalankanPerformanceTest()
        {
            var stopwatch = new Stopwatch();
            var configTest = new KonfigurasiAplikasi();

            Console.WriteLine("Menjalankan kalkulasi PPN sebanyak 1 Juta kali (Simulasi Load Config)...");

            stopwatch.Start();
            decimal totalPajakSintetis = 0;

            for (int i = 0; i < 1000000; i++)
            {
                // Menghitung pajak Rp 100 berulang kali untuk stress test memori
                totalPajakSintetis += configTest.HitungPpn(100m);
            }

            stopwatch.Stop();

            Console.WriteLine("Kalkulasi Selesai.");
            Console.WriteLine($"Waktu Eksekusi (Performance): {stopwatch.ElapsedMilliseconds} milidetik.");
        }
    }
}