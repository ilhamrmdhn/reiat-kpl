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

            // --- INISIALISASI MODUL ---
            var config = new KonfigurasiAplikasi();
            var machine = new StatusPesananMachine();
            var keranjang = new KeranjangBelanja();
            var kalkulator = new KalkulatorDiskon();
            var penyimpananKategori = new PenyimpananLokal<string>();

            Console.WriteLine($"[Config] PPN saat ini dibaca sebesar: {config.Ppn * 100}%");
            Console.WriteLine($"[State] Status Awal Pesanan: {machine.StateSaatIni}\n");

            try
            {
                // --- BAGIAN HUDA (Demo Generics) ---
                Console.WriteLine("> Menyimpan data master (Teknik Generics)...");
                penyimpananKategori.Simpan("Kategori: Pakaian Fisik");
                penyimpananKategori.Simpan("Kategori: Pola Digital");
                Console.WriteLine($"Tersimpan {penyimpananKategori.AmbilSemua().Count} kategori di PenyimpananLokal.\n");

                // --- BAGIAN ILHAM (Keranjang Belanja) ---
                var baju = new PakaianFisik("Kemeja Reiat Basic", 120000, "M", 200);
                var polaDigital = new PolaDigital("Pola Celana Cargo", 45000, "PDF", "https://reiat.com/dl/cargo");

                Console.WriteLine("> Menambahkan produk ke keranjang...");
                keranjang.TambahProduk(baju);
                keranjang.TambahProduk(polaDigital);

                decimal subtotal = keranjang.HitungTotalHarga();
                Console.WriteLine($"Total Item : {keranjang.LihatKeranjang().Count}");
                Console.WriteLine($"Subtotal Harga: Rp {subtotal:N0}\n");

                // --- BAGIAN HUDA (Table-Driven Kalkulator Diskon) ---
                Console.WriteLine("> Memproses Kalkulator Diskon (Teknik Table-driven)...");
                string promo = "REIATBARU";
                decimal diskon = kalkulator.DapatkanDiskon(promo, subtotal);
                decimal totalSetelahDiskon = subtotal - diskon;

                Console.WriteLine($"Kode Promo Dipakai : {promo}");
                Console.WriteLine($"Diskon Didapat     : Rp {diskon:N0}");
                Console.WriteLine($"Subtotal Sementara : Rp {totalSetelahDiskon:N0}\n");

                // --- BAGIAN AUL (Automata Status & Perhitungan PPN) ---
                machine.LanjutKeCheckout();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");

                // Pajak dihitung dari harga yang sudah didiskon
                decimal pajak = config.HitungPpn(totalSetelahDiskon);
                decimal grandTotal = totalSetelahDiskon + pajak;

                Console.WriteLine($"\n[Tagihan Akhir]");
                Console.WriteLine($"PPN (11%)          : Rp {pajak:N0}");
                Console.WriteLine($"Grand Total        : Rp {grandTotal:N0}\n");

                machine.ProsesPembayaran();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");

                machine.SelesaikanPesanan();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}\n");

                // --- TEST DbC HUDA (Sengaja ditaruh di akhir agar demo utama selesai dulu) ---
                Console.WriteLine("> [Test DbC] Mencoba kode promo yang tidak ada di tabel...");
                kalkulator.DapatkanDiskon("CINTAREIAT", subtotal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terjadi Kesalahan (DbC Aktif): {ex.Message}");
            }

            // Menjalankan Performance Testing Gabungan
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

            stopwatch.Start();
            decimal total = keranjangTest.HitungTotalHarga();
            stopwatch.Stop();

            Console.WriteLine($"Total Harga Terkalkulasi: Rp {total:N0}");
            Console.WriteLine($"Waktu Eksekusi Keranjang: {stopwatch.ElapsedMilliseconds} milidetik.");

            stopwatch.Reset();

            // --- Performance Test Bagian Aul ---
            Console.WriteLine("\n[TEST AUL] Menjalankan kalkulasi PPN sebanyak 1 Juta kali (Simulasi Load Config)...");
            var configTest = new KonfigurasiAplikasi();
            decimal totalPajakSintetis = 0;

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                totalPajakSintetis += configTest.HitungPpn(100m);
            }
            stopwatch.Stop();

            Console.WriteLine("Kalkulasi PPN Selesai.");
            Console.WriteLine($"Waktu Eksekusi PPN: {stopwatch.ElapsedMilliseconds} milidetik.");

            stopwatch.Reset();

            // --- Performance Test Bagian Huda ---
            Console.WriteLine("\n[TEST HUDA] Menyimpan 1 Juta data angka ke dalam PenyimpananLokal<T>...");
            var penyimpananTest = new PenyimpananLokal<int>();

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                penyimpananTest.Simpan(i);
            }
            stopwatch.Stop();

            Console.WriteLine($"Data berhasil disimpan: {penyimpananTest.AmbilSemua().Count} item.");
            Console.WriteLine($"Waktu Eksekusi Generics: {stopwatch.ElapsedMilliseconds} milidetik.");
        }
    }
}