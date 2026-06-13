using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Reiat.Lib;

namespace Reiat.Main
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // --- INI SIFATNYA GLOBAL (STATE DI-MAINTAIN SELAMA APLIKASI JALAN) ---
            var config = new KonfigurasiAplikasi();
            var machine = new StatusPesananMachine();
            var keranjang = new KeranjangBelanja();
            var kalkulator = new KalkulatorDiskon();
            var penyimpananKategori = new PenyimpananLokal<string>();
            var authMachine = new AutentikasiMachine();

            // Client untuk menembak API secara HTTP
            using HttpClient client = new HttpClient();

            string baseUrlApi = "https://localhost:7235";

            // Variabel bantu untuk menyimpan state interaktif & Hak Akses
            string userEmail = "";
            string userRole = "Guest";
            string promoDipakai = "";
            decimal diskonDidapat = 0;
            bool sudahBayar = false;

            // Memasukkan beberapa kategori bawaan ke penyimpanan generik di awal
            penyimpananKategori.Simpan("Kategori: Pakaian Fisik");
            penyimpananKategori.Simpan("Kategori: Pola Digital");

            // Konfigurasi agar pembacaan JSON tidak terpaku huruf besar/kecil
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("======================================================");
                Console.WriteLine("                   E-COMMERCE REIAT                   ");
                Console.WriteLine("======================================================");
                Console.WriteLine($"[Akses Role  ]: {userRole} {(string.IsNullOrEmpty(userEmail) ? "" : $"({userEmail})")}");

                string authDisplay = (authMachine.StateSaatIni.ToString() == "Authenticated") ? "AUTHENTICATED" : "UNAUTHENTICATED";
                Console.WriteLine($"[Status Auth ]: {authDisplay}");

                Console.WriteLine($"[Status Order]: {machine.StateSaatIni}");

                if (userRole != "Admin")
                {
                    Console.WriteLine($"[Keranjang   ]: {keranjang.LihatKeranjang().Count} Item");
                    if (!string.IsNullOrEmpty(promoDipakai)) Console.WriteLine($"[Promo Aktif ]: {promoDipakai} (Diskon: Rp {diskonDidapat:N0})");
                }

                Console.WriteLine("======================================================");
                Console.WriteLine("1. Login / Autentikasi User          (Dewo)");
                Console.WriteLine("2. Lihat Katalog Produk via API      (Reja)");
                Console.WriteLine("3. Tambah Produk ke Keranjang        (Ilham)");
                Console.WriteLine("4. Lihat Isi Keranjang Belanja       (Ilham)");
                Console.WriteLine("5. Klaim Kode Promo Diskon           (Huda)");
                Console.WriteLine("6. Kelola Master Kategori - Generics (Huda) [ADMIN ONLY]");
                Console.WriteLine("7. Proses Checkout & Pembayaran      (Aul & Ilham)");
                Console.WriteLine("0. Keluar");
                Console.WriteLine("======================================================");
                Console.Write("Pilih menu (0-7): ");

                string pilihan = Console.ReadLine();
                Console.WriteLine("\n------------------------------------------------------");

                switch (pilihan)
                {
                    case "1": // --- BAGIAN DEWO (Autentikasi) ---
                        if (authMachine.StateSaatIni.ToString() == "Authenticated")
                        {
                            Console.WriteLine($"Anda sudah login sebagai {userRole}!");
                            break;
                        }

                        try
                        {
                            authMachine.TriggerLogin();
                            Console.WriteLine("(Gunakan email 'admin@reiat.com' & sandi 'admin123' untuk akses Admin)");

                            Console.Write("Masukkan Email Anda : ");
                            string inputEmail = Console.ReadLine();
                            ValidatorInput.ValidasiEmail(inputEmail);

                            Console.Write("Masukkan Password   : ");
                            string inputPassword = Console.ReadLine();
                            ValidatorInput.ValidasiPassword(inputPassword);

                            userEmail = inputEmail;

                            if (userEmail == "admin@reiat.com")
                            {
                                if (inputPassword == "admin123")
                                    userRole = "Admin";
                                else
                                    throw new Exception("Password Admin salah! Akses ditolak.");
                            }
                            else
                            {
                                userRole = "Customer";
                            }

                            authMachine.SuksesLogin();
                            Console.WriteLine($"\n[SUKSES] Login berhasil! Anda masuk sebagai: {userRole}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\n[GAGAL LOGIN - DbC Menolak]: {ex.Message}");
                            authMachine = new AutentikasiMachine();
                            userEmail = "";
                            userRole = "Guest";
                        }
                        break;

                    case "2": // --- BAGIAN REJA (Katalog & ResponAPI<T> VIA HTTP) ---
                        Console.WriteLine("> Mengirim HTTP GET Request ke API Katalog...");
                        try
                        {
                            // Pemanggilan API
                            HttpResponseMessage response = await client.GetAsync($"{baseUrlApi}/api/Katalog");

                            if (response.IsSuccessStatusCode)
                            {
                                string jsonResponse = await response.Content.ReadAsStringAsync();

                                // Parse JSON (menggunakan huruf kecil sesuai standar web camelCase)
                                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                                var root = doc.RootElement;

                                bool isSukses = root.GetProperty("sukses").GetBoolean();
                                Console.WriteLine($"[API Status] : {(isSukses ? "SUKSES" : "GAGAL")}");
                                Console.WriteLine($"[API Message]: {root.GetProperty("pesan").GetString()}");
                                Console.WriteLine("[Katalog Tersedia]:");

                                foreach (var item in root.GetProperty("data").EnumerateArray())
                                {
                                    int id = item.GetProperty("id").GetInt32();
                                    string tipe = item.GetProperty("tipe").GetString();
                                    string nama = item.GetProperty("nama").GetString();
                                    decimal harga = item.GetProperty("harga").GetDecimal();

                                    Console.WriteLine($"  {id}. [{tipe}] {nama} - Rp {harga:N0}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[GAGAL] API mengembalikan status: {response.StatusCode}");
                            }
                        }
                        catch (HttpRequestException)
                        {
                            Console.WriteLine("[SYSTEM ERROR] Gagal terhubung ke API. Pastikan server Reiat.API sedang menyala (di-run)!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[SYSTEM ERROR] Terjadi kesalahan: {ex.Message}");
                        }
                        break;

                    case "3": // --- BAGIAN ILHAM (Tambah Produk ke Keranjang) ---
                        if (userRole == "Admin")
                        {
                            Console.WriteLine("[AKSES DITOLAK] Admin tidak bisa berbelanja. Menu ini khusus Customer.");
                            break;
                        }

                        Console.WriteLine("Pilih produk yang ingin ditambahkan:");
                        Console.WriteLine("1. Kemeja Reiat Basic (Rp 150.000)");
                        Console.WriteLine("2. Pola Celana Cargo (Rp 45.000)");
                        Console.Write("Pilihan Anda: ");
                        string pProduk = Console.ReadLine();

                        try
                        {
                            if (pProduk == "1")
                            {
                                keranjang.TambahProduk(new PakaianFisik("Kemeja Reiat Basic", 150000, "M", 200));
                                Console.WriteLine("\n[SUKSES] Kemeja Reiat Basic dimasukkan ke keranjang.");
                            }
                            else if (pProduk == "2")
                            {
                                keranjang.TambahProduk(new PolaDigital("Pola Celana Cargo", 45000, "PDF", "https://reiat.com/dl/cargo"));
                                Console.WriteLine("\n[SUKSES] Pola Celana Cargo dimasukkan ke keranjang.");
                            }
                            else
                            {
                                Console.WriteLine("\nPilihan tidak valid.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nGagal menambahkan (DbC): {ex.Message}");
                        }
                        break;

                    case "4": // --- BAGIAN ILHAM (Lihat Keranjang) ---
                        if (userRole == "Admin")
                        {
                            Console.WriteLine("[AKSES DITOLAK] Fitur ini khusus Customer.");
                            break;
                        }

                        var listKeranjang = keranjang.LihatKeranjang();
                        if (listKeranjang.Count == 0)
                        {
                            Console.WriteLine("Keranjang belanja Anda masih kosong.");
                            break;
                        }

                        Console.WriteLine("[Isi Keranjang Belanja Anda]:");
                        foreach (var prod in listKeranjang)
                        {
                            Console.WriteLine($"- {prod.Nama} (Rp {prod.Harga:N0})");
                        }
                        Console.WriteLine($"\nSubtotal Harga: Rp {keranjang.HitungTotalHarga():N0}");
                        break;

                    case "5": // --- BAGIAN HUDA (Table-Driven Diskon) ---
                        if (userRole != "Customer")
                        {
                            Console.WriteLine("[AKSES DITOLAK] Hanya Customer yang dapat menggunakan promo.");
                            break;
                        }

                        if (keranjang.LihatKeranjang().Count == 0)
                        {
                            Console.WriteLine("Keranjang masih kosong, tidak bisa memproses diskon.");
                            break;
                        }

                        Console.Write("Masukkan Kode Promo (Contoh: REIATBARU): ");
                        string inputPromo = Console.ReadLine()?.ToUpper();

                        try
                        {
                            decimal currentSubtotal = keranjang.HitungTotalHarga();
                            diskonDidapat = kalkulator.DapatkanDiskon(inputPromo, currentSubtotal);
                            promoDipakai = inputPromo;
                            Console.WriteLine($"\n[SUKSES] Kode promo cocok! Anda mendapatkan diskon sebesar Rp {diskonDidapat:N0}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\n[PROMO GAGAL - DbC Aktif]: {ex.Message}");
                            promoDipakai = "";
                            diskonDidapat = 0;
                        }
                        break;

                    case "6": // --- BAGIAN HUDA (Generics Penyimpanan - ADMIN ONLY) ---
                        if (userRole != "Admin")
                        {
                            Console.WriteLine("[AKSES DITOLAK] Hanya Admin yang diizinkan untuk mengelola Master Kategori.");
                            break;
                        }

                        Console.WriteLine("[Daftar Kategori Produk Saat Ini]:");
                        foreach (var kat in penyimpananKategori.AmbilSemua())
                        {
                            Console.WriteLine($"  {kat}");
                        }

                        Console.Write("\nIngin menambah kategori baru? (y/n): ");
                        if (Console.ReadLine()?.ToLower() == "y")
                        {
                            Console.Write("Masukkan nama kategori baru: ");
                            string katBaru = Console.ReadLine();
                            penyimpananKategori.Simpan($"Kategori: {katBaru}");
                            Console.WriteLine("\n[SUKSES] Kategori baru berhasil disimpan menggunakan teknik Generics.");
                        }
                        break;

                    case "7": // --- BAGIAN AUL & ILHAM (Checkout, Ongkir VIA HTTP, PPN) ---
                        if (userRole != "Customer")
                        {
                            Console.WriteLine("[AKSES DITOLAK] Anda harus login sebagai Customer untuk melakukan checkout.");
                            break;
                        }
                        if (keranjang.LihatKeranjang().Count == 0)
                        {
                            Console.WriteLine("Tidak ada barang di keranjang untuk dicheckout.");
                            break;
                        }
                        if (sudahBayar)
                        {
                            Console.WriteLine("Pesanan ini sudah selesai diproses.");
                            break;
                        }

                        try
                        {
                            machine.LanjutKeCheckout();
                            Console.WriteLine($"[State] Status berubah menjadi: {machine.StateSaatIni}\n");

                            // --- MENGAMBIL DATA ONGKIR VIA JARINGAN ---
                            Console.WriteLine("> Mengirim HTTP GET Request ke API Ongkir...");
                            Console.Write("  Masukkan Kota Tujuan Pengiriman: ");
                            string kotaTujuan = Console.ReadLine();
                            double beratTotalGram = 1000;
                            Console.WriteLine($"  [Info] Berat barang diasumsikan {beratTotalGram} gram.");

                            decimal ongkir = 0;

                            try
                            {
                                // Menyusun URL Endpoint Ongkir
                                string endpoint = $"{baseUrlApi}/api/Ongkir/cek?kotaTujuan={kotaTujuan}&beratGram={beratTotalGram}";
                                HttpResponseMessage responseOngkir = await client.GetAsync(endpoint);

                                string jsonOngkir = await responseOngkir.Content.ReadAsStringAsync();

                                if (responseOngkir.IsSuccessStatusCode)
                                {
                                    using JsonDocument doc = JsonDocument.Parse(jsonOngkir);
                                    var root = doc.RootElement;

                                    ongkir = root.GetProperty("totalBiayaKirim").GetDecimal();
                                    string namaTujuan = root.GetProperty("tujuan").GetString();

                                    Console.WriteLine($"  [API SUKSES] Biaya kirim ke {namaTujuan} = Rp {ongkir:N0}\n");
                                }
                                else if (responseOngkir.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                {
                                    // Melempar error sesuai dengan DbC di API
                                    throw new Exception($"Validasi API Ditolak - {jsonOngkir}");
                                }
                                else
                                {
                                    throw new Exception($"Server mengembalikan status: {responseOngkir.StatusCode}");
                                }
                            }
                            catch (HttpRequestException)
                            {
                                throw new Exception("Gagal terhubung ke API. Pastikan server Reiat.API sedang berjalan.");
                            }

                            // --- MENGHITUNG FINAL ---
                            decimal subtotalAkhir = keranjang.HitungTotalHarga();
                            decimal setelahDiskon = subtotalAkhir - diskonDidapat;
                            decimal pajak = config.HitungPpn(setelahDiskon);
                            decimal grandTotal = setelahDiskon + pajak + ongkir;

                            Console.WriteLine("============= NOTA PEMBAYARAN =============");
                            Console.WriteLine($"Subtotal Belanja : Rp {subtotalAkhir:N0}");
                            Console.WriteLine($"Potongan Diskon  : -Rp {diskonDidapat:N0}");
                            Console.WriteLine($"Harga Setelah Disc: Rp {setelahDiskon:N0}");
                            Console.WriteLine($"PPN              : Rp {pajak:N0}");
                            Console.WriteLine($"Ongkos Kirim     : Rp {ongkir:N0}");
                            Console.WriteLine($"Grand Total      : Rp {grandTotal:N0}");
                            Console.WriteLine("===========================================");

                            Console.Write("\nTekan ENTER untuk melakukan pembayaran digital...");
                            Console.ReadLine();

                            machine.ProsesPembayaran();
                            Console.WriteLine($"\n-> Berpindah ke: {machine.StateSaatIni}");

                            machine.SelesaikanPesanan();
                            Console.WriteLine($"-> Berpindah ke: {machine.StateSaatIni}");
                            Console.WriteLine("\n[SUKSES] Terima kasih telah berbelanja di platform Reiat!");

                            sudahBayar = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nGagal checkout: {ex.Message}");
                        }
                        break;

                    case "0":
                        Console.WriteLine("Keluar dari aplikasi. Sampai jumpa!");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Pilihan menu salah. Silakan masukkan angka 0-7.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nTekan ENTER untuk kembali ke menu utama...");
                    Console.ReadLine();
                }
            }
        }
    }
}