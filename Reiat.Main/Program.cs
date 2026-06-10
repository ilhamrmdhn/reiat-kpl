using System;
using System.Diagnostics;
using Reiat.Lib;

namespace Reiat.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== [DEMO HUDA] E-COMMERCE REIAT ===");

            var kalkulator = new KalkulatorDiskon();
            var penyimpananString = new PenyimpananLokal<string>();

            try
            {
                // Demo Teknik Generics
                Console.WriteLine("\n> Menyimpan kategori produk (Teknik Generics)...");
                penyimpananString.Simpan("Kategori: Fisik");
                penyimpananString.Simpan("Kategori: Digital");
                Console.WriteLine($"Tersimpan {penyimpananString.AmbilSemua().Count} kategori.");

                // Demo Teknik Table-Driven
                Console.WriteLine("\n> Memproses Kalkulator Diskon (Teknik Table-driven)...");
                decimal totalBelanja = 200000m;
                string promo = "REIATBARU";
                Console.WriteLine($"Total Belanja : Rp {totalBelanja:N0}");
                Console.WriteLine($"Kode Promo    : {promo}");

                decimal diskon = kalkulator.DapatkanDiskon(promo, totalBelanja);
                Console.WriteLine($"Diskon Didapat: Rp {diskon:N0}");
                Console.WriteLine($"Sisa Bayar    : Rp {totalBelanja - diskon:N0}");

                // Mengetes DbC (Sengaja memasukkan kode salah)
                Console.WriteLine("\n> Mencoba promo yang tidak ada di tabel...");
                kalkulator.DapatkanDiskon("CINTAREIAT", 100000m);
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

            // Tes kecepatan class Generik untuk menampung tipe integer
            var penyimpananTest = new PenyimpananLokal<int>();

            Console.WriteLine("Menyimpan 1 Juta data angka ke dalam PenyimpananLokal<T>...");

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                penyimpananTest.Simpan(i);
            }
            stopwatch.Stop();

            Console.WriteLine($"Data berhasil disimpan: {penyimpananTest.AmbilSemua().Count} item.");
            Console.WriteLine($"Waktu Eksekusi (Performance): {stopwatch.ElapsedMilliseconds} milidetik.");
        }
    }
}