using System;
using System.Diagnostics;
using Reiat.Lib;

namespace Reiat.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("         E-COMMERCE REIAT - DEMO TUBES CLO 2");
            Console.WriteLine("======================================================\n");

            // --- BAGIAN AUL (Konfigurasi & Automata) ---
            var config = new KonfigurasiAplikasi();
            Console.WriteLine($"[Config] PPN saat ini dibaca sebesar: {config.Ppn * 100}%\n");

            var machine = new StatusPesananMachine();
            Console.WriteLine($"Status Awal Pesanan: {machine.StateSaatIni}\n");

            // --- BAGIAN ILHAM (Keranjang Belanja) ---
            var keranjang = new KeranjangBelanja();

            try
            {
                var baju = new PakaianFisik("Kemeja Reiat Basic", 120000, "M", 200);
                var polaDigital = new PolaDigital("Pola Celana Cargo", 45000, "PDF", "https://reiat.com/dl/cargo");

                Console.WriteLine("> Menambahkan produk ke keranjang...");
                keranjang.TambahProduk(baju);
                keranjang.TambahProduk(polaDigital);

                decimal totalBelanja = keranjang.HitungTotalHarga();
                Console.WriteLine("\n[Ringkasan Pesanan Sementara]");
                Console.WriteLine($"Total Item : {keranjang.LihatKeranjang().Count}");
                Console.WriteLine($"Total Harga: Rp {totalBelanja:N0}\n");

                // --- MENGGABUNGKAN LOGIKA AUL & ILHAM ---
                machine.LanjutKeCheckout();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");

                decimal pajak = config.HitungPpn(totalBelanja);

                Console.WriteLine($"\n[Kalkulasi Akhir]");
                Console.WriteLine($"Tagihan Barang : Rp {totalBelanja:N0}");
                Console.WriteLine($"PPN (11%)      : Rp {pajak:N0}");
                Console.WriteLine($"Total Tagihan  : Rp {totalBelanja + pajak:N0}\n");

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

            // --- Performance Test Bagian Ilham ---
            Console.WriteLine("\n[TEST ILHAM] Menyiapkan 1 Juta data dummy produk untuk dihitung...");
            var keranjangTest = new KeranjangBelanja();
            for (int i = 0; i < 1000000; i++)
            {
                keranjangTest.TambahProduk(new PolaDigital($"Dummy {i}", 100, "PDF", "link"));
            }

            Console.WriteLine("Mulai menghitung total harga...");
            stopwatch.Start();
            decimal total = keranjangTest.HitungTotalHarga();
            stopwatch.Stop();

            Console.WriteLine($"Total Harga Terkalkulasi: Rp {total:N0}");
            Console.WriteLine($"Waktu Eksekusi Keranjang: {stopwatch.ElapsedMilliseconds} milidetik.");

            stopwatch.Reset(); // Reset stopwatch untuk test berikutnya milik Aul

            // --- Performance Test Bagian Aul ---
            Console.WriteLine("\n[TEST AUL] Menjalankan kalkulasi PPN sebanyak 1 Juta kali (Simulasi Load Config)...");
            var configTest = new KonfigurasiAplikasi();
            decimal totalPajakSintetis = 0;

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                // Menghitung pajak Rp 100 berulang kali untuk stress test memori
                totalPajakSintetis += configTest.HitungPpn(100m);
            }
            stopwatch.Stop();

            Console.WriteLine("Kalkulasi PPN Selesai.");
            Console.WriteLine($"Waktu Eksekusi PPN: {stopwatch.ElapsedMilliseconds} milidetik.");
        }
    }
}