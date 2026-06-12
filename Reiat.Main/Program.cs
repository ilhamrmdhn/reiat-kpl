using System;
using System.Collections.Generic;
using System.Diagnostics;
using Reiat.Lib;

namespace Reiat.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== [DEMO REJA] E-COMMERCE REIAT ===");

            try
            {
                // Demo Teknik Parameterization/Generics (Membungkus balikan tipe List<string>)
                Console.WriteLine("\n> Simulasi Call API & Menggunakan ResponAPI<T>...");

                var daftarProduk = new List<string>
                {
                    "1. [Fisik] Kemeja Reiat Basic - Rp 150.000",
                    "2. [Digital] Pola Jahit Celana Cargo - Rp 45.000"
                };

                var responSukses = new ResponAPI<List<string>>(true, "Berhasil memuat data katalog", daftarProduk);

                Console.WriteLine($"[Respons Server] Status : {(responSukses.Sukses ? "SUKSES" : "GAGAL")}");
                Console.WriteLine($"[Respons Server] Pesan  : {responSukses.Pesan}");
                Console.WriteLine("[Respons Server] Data   :");
                foreach (var item in responSukses.Data)
                {
                    Console.WriteLine($"  {item}");
                }

                // Menguji DbC (Sengaja membuat objek yang melanggar aturan)
                Console.WriteLine("\n> Menguji pertahanan sistem (DbC)...");
                Console.WriteLine("Memaksa sistem mengembalikan status SUKSES tetapi data NULL...");

                var responError = new ResponAPI<string>(true, "Berhasil", null); // Baris ini akan ditahan DbC
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[DITAHAN OLEH DbC]: {ex.Message}");
            }

            // Menjalankan Performance Testing
            Console.WriteLine("\n=== MENJALANKAN PERFORMANCE TESTING ===");
            JalankanPerformanceTest();
        }

        static void JalankanPerformanceTest()
        {
            var stopwatch = new Stopwatch();

            Console.WriteLine("Menjalankan pembungkusan (wrapping) ResponAPI<T> sebanyak 1 Juta kali...");

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                // Mensimulasikan proses backend membungkus data respons secara masif
                var responSintetis = new ResponAPI<string>(true, "OK", "Data Dummy");
            }
            stopwatch.Stop();

            Console.WriteLine("Proses Wrapping Selesai.");
            Console.WriteLine($"Waktu Eksekusi (Performance): {stopwatch.ElapsedMilliseconds} milidetik.");
        }
    }
}