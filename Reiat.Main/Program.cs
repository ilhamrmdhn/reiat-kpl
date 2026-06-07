using System;
using System.Diagnostics;
using Reiat.Lib;

namespace Reiat.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== [DEMO ILHAM] E-COMMERCE REIAT ===");

            var keranjang = new KeranjangBelanja();

            try
            {
                var baju = new PakaianFisik("Kemeja Reiat Basic", 120000, "M", 200);
                var polaDigital = new PolaDigital("Pola Celana Cargo", 45000, "PDF", "https://reiat.com/dl/cargo");

                Console.WriteLine("\n> Menambahkan produk ke keranjang...");
                keranjang.TambahProduk(baju);
                keranjang.TambahProduk(polaDigital);

                Console.WriteLine("\n[Ringkasan Pesanan Sementara]");
                Console.WriteLine($"Total Item : {keranjang.LihatKeranjang().Count}");
                Console.WriteLine($"Total Harga: Rp {keranjang.HitungTotalHarga():N0}");
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
            var keranjangTest = new KeranjangBelanja();

            Console.WriteLine("Menyiapkan 1 Juta data dummy produk untuk dihitung...");
            for (int i = 0; i < 1000000; i++)
            {
                keranjangTest.TambahProduk(new PolaDigital($"Dummy {i}", 100, "PDF", "link"));
            }

            Console.WriteLine("Mulai menghitung total harga...");

            // Eksekusi pengujian waktu proses 
            stopwatch.Start();
            decimal total = keranjangTest.HitungTotalHarga();
            stopwatch.Stop();

            Console.WriteLine($"Total Harga Terkalkulasi: Rp {total:N0}");
            Console.WriteLine($"Waktu Eksekusi (Performance): {stopwatch.ElapsedMilliseconds} milidetik.");
        }
    }
}