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
            Console.WriteLine("======================================================");
            Console.WriteLine("         E-COMMERCE REIAT - DEMO TUBES CLO 2");
            Console.WriteLine("======================================================\n");

            // --- INISIALISASI MODUL ---
            var config = new KonfigurasiAplikasi();
            var machine = new StatusPesananMachine();
            var keranjang = new KeranjangBelanja();
            var kalkulator = new KalkulatorDiskon();
            var penyimpananKategori = new PenyimpananLokal<string>();
            var authMachine = new AutentikasiMachine();

            Console.WriteLine($"[Config] PPN saat ini dibaca sebesar: {config.Ppn * 100}%");
            Console.WriteLine($"[State]  Status Awal Login  : {authMachine.StateSaatIni}");
            Console.WriteLine($"[State]  Status Awal Pesanan: {machine.StateSaatIni}\n");

            // --- BAGIAN DEWO (Sistem Autentikasi & Validasi Input) ---
            try
            {
                Console.WriteLine("> User mencoba checkout/akses platform...");
                authMachine.TriggerLogin();
                Console.WriteLine($"-> Sistem meminta login. Status berubah: {authMachine.StateSaatIni}");

                Console.WriteLine("\n[Proses Autentikasi]");
                Console.Write("Masukkan Email: ");
                string emailInput = "dewosalah.com"; // Simulasi input salah
                Console.WriteLine(emailInput);

                Console.WriteLine("> Memvalidasi email...");
                ValidatorInput.ValidasiEmail(emailInput); // Melempar Exception
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[GAGAL LOGIN - DbC Aktif]: {ex.Message}");
            }

            try
            {
                // Simulasi perbaikan input email yang benar
                Console.WriteLine("\n> Mengulang input email yang benar...");
                ValidatorInput.ValidasiEmail("dewo@telkom.edu");
                Console.WriteLine("[Validasi Lolos] Email benar.");

                authMachine.SuksesLogin();
                Console.WriteLine($"-> Login Sukses! Status berubah: {authMachine.StateSaatIni}\n");

                // --- ALUR BERLANJUT JIKA AUTH BERHASIL ---

                // --- BAGIAN REJA (Demo Katalog & ResponAPI<T>) ---
                Console.WriteLine("> Simulasi Call API untuk memuat katalog produk...");
                var daftarProduk = new List<string>
                {
                    "1. [Fisik] Kemeja Reiat Basic - Rp 120.000",
                    "2. [Digital] Pola Celana Cargo - Rp 45.000"
                };

                var responKatalog = new ResponAPI<List<string>>(true, "Berhasil memuat data katalog", daftarProduk);

                Console.WriteLine($"[Respons API] Status : {(responKatalog.Sukses ? "SUKSES" : "GAGAL")}");
                Console.WriteLine($"[Respons API] Pesan  : {responKatalog.Pesan}");
                Console.WriteLine("[Respons API] Data   :");
                foreach (var item in responKatalog.Data)
                {
                    Console.WriteLine($"  {item}");
                }
                Console.WriteLine();

                // --- BAGIAN HUDA (Demo Generics Menyimpan Kategori) ---
                Console.WriteLine("> Menyimpan data master kategori produk...");
                penyimpananKategori.Simpan("Kategori: Pakaian Fisik");
                penyimpananKategori.Simpan("Kategori: Pola Digital");
                Console.WriteLine($"Tersimpan {penyimpananKategori.AmbilSemua().Count} kategori di PenyimpananLokal.\n");

                // --- BAGIAN ILHAM (Keranjang Belanja) ---
                var baju = new PakaianFisik("Kemeja Reiat Basic", 120000, "M", 200);
                var polaDigital = new PolaDigital("Pola Celana Cargo", 45000, "PDF", "https://reiat.com/dl/cargo");

                Console.WriteLine("> User menambahkan produk dari katalog ke keranjang...");
                keranjang.TambahProduk(baju);
                keranjang.TambahProduk(polaDigital);

                decimal subtotal = keranjang.HitungTotalHarga();
                Console.WriteLine($"Total Item : {keranjang.LihatKeranjang().Count}");
                Console.WriteLine($"Subtotal Harga: Rp {subtotal:N0}\n");

                // --- BAGIAN HUDA (Table-Driven Kalkulator Diskon) ---
                Console.WriteLine("> Memproses Kalkulator Diskon...");
                string promo = "REIATBARU";
                decimal diskon = kalkulator.DapatkanDiskon(promo, subtotal);
                decimal totalSetelahDiskon = subtotal - diskon;

                Console.WriteLine($"Kode Promo Dipakai : {promo}");
                Console.WriteLine($"Diskon Didapat     : Rp {diskon:N0}");
                Console.WriteLine($"Subtotal Sementara : Rp {totalSetelahDiskon:N0}\n");

                // --- BAGIAN AUL (Automata Status & Perhitungan PPN) ---
                machine.LanjutKeCheckout();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");

                decimal pajak = config.HitungPpn(totalSetelahDiskon);
                decimal grandTotal = totalSetelahDiskon + pajak;

                Console.WriteLine($"\n[Tagihan Akhir]");
                Console.WriteLine($"PPN (11%)          : Rp {pajak:N0}");
                Console.WriteLine($"Grand Total        : Rp {grandTotal:N0}\n");

                machine.ProsesPembayaran();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");

                machine.SelesaikanPesanan();
                Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}\n");

                // --- TEST DbC GABUNGAN (Diuji di akhir agar tidak merusak demo utama) ---
                Console.WriteLine("--- MENGUJI PERTAHANAN SISTEM (DbC) ---");
                
                Console.WriteLine("> [Test DbC Huda] Mencoba kode promo yang tidak ada...");
                kalkulator.DapatkanDiskon("CINTAREIAT", subtotal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terjadi Kesalahan (DbC Diskon Aktif): {ex.Message}");
            }

            try 
            {
                Console.WriteLine("\n> [Test DbC Reja] Memaksa sistem API mengembalikan data NULL...");
                var responError = new ResponAPI<string>(true, "Berhasil", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terjadi Kesalahan (DbC API Aktif): {ex.Message}");
            }

            // Menjalankan Performance Testing Gabungan
            Console.WriteLine("\n=== MENJALANKAN PERFORMANCE TESTING (SELURUH MODUL) ===");
            JalankanPerformanceTest();
        }

        static void JalankanPerformanceTest()
        {
            var stopwatch = new Stopwatch();

            // [TEST ILHAM]
            Console.WriteLine("\n[1] Menyiapkan 1 Juta data dummy produk untuk dihitung...");
            var keranjangTest = new KeranjangBelanja();
            for (int i = 0; i < 1000000; i++)
            {
                keranjangTest.TambahProduk(new PolaDigital($"Dummy {i}", 100, "PDF", "link"));
            }

            stopwatch.Start();
            decimal total = keranjangTest.HitungTotalHarga();
            stopwatch.Stop();
            Console.WriteLine($"Waktu Eksekusi Keranjang (Ilham): {stopwatch.ElapsedMilliseconds} milidetik.");
            stopwatch.Reset();

            // [TEST AUL]
            Console.WriteLine("\n[2] Menjalankan kalkulasi PPN sebanyak 1 Juta kali...");
            var configTest = new KonfigurasiAplikasi();
            decimal totalPajakSintetis = 0;

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                totalPajakSintetis += configTest.HitungPpn(100m);
            }
            stopwatch.Stop();
            Console.WriteLine($"Waktu Eksekusi PPN (Aul): {stopwatch.ElapsedMilliseconds} milidetik.");
            stopwatch.Reset();

            // [TEST HUDA]
            Console.WriteLine("\n[3] Menyimpan 1 Juta data ke dalam PenyimpananLokal<T>...");
            var penyimpananTest = new PenyimpananLokal<int>();

            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                penyimpananTest.Simpan(i);
            }
            stopwatch.Stop();
            Console.WriteLine($"Waktu Eksekusi Generics (Huda): {stopwatch.ElapsedMilliseconds} milidetik.");
            stopwatch.Reset();

            // [TEST DEWO]
            Console.WriteLine("\n[4] Menjalankan validasi format email sebanyak 1 Juta kali...");
            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                ValidatorInput.ValidasiEmail("test@domain.com");
            }
            stopwatch.Stop();
            Console.WriteLine($"Waktu Eksekusi Validasi (Dewo): {stopwatch.ElapsedMilliseconds} milidetik.");
            stopwatch.Reset();

            // [TEST REJA]
            Console.WriteLine("\n[5] Menjalankan wrapping ResponAPI<T> sebanyak 1 Juta kali...");
            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var responSintetis = new ResponAPI<string>(true, "OK", "Data Dummy");
            }
            stopwatch.Stop();
            Console.WriteLine($"Waktu Eksekusi Wrapper API (Reja): {stopwatch.ElapsedMilliseconds} milidetik.");
        }
    }
}