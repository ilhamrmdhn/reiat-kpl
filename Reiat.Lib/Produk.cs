using System;

namespace Reiat.Lib
{
    public abstract class Produk
    {
        public string Nama { get; protected set; }
        public decimal Harga { get; protected set; }

        protected Produk(string nama, decimal harga)
        {
            // Defensive Programming: Pre-condition validasi nama produk 
            if (string.IsNullOrWhiteSpace(nama))
            {
                throw new ArgumentException("Nama produk tidak boleh kosong.");
            }

            // Defensive Programming: Pre-condition validasi harga 
            if (harga < 0)
            {
                throw new ArgumentException("Harga produk tidak boleh kurang dari nol.");
            }

            Nama = nama;
            Harga = harga;
        }
    }

    public class PakaianFisik : Produk
    {
        public string Ukuran { get; private set; }
        public double BeratGram { get; private set; }

        public PakaianFisik(string nama, decimal harga, string ukuran, double beratGram)
            : base(nama, harga)
        {
            // Defensive Programming: Pre-condition validasi berat fisik 
            if (beratGram <= 0)
            {
                throw new ArgumentException("Berat pakaian fisik harus lebih dari 0 gram.");
            }

            Ukuran = ukuran;
            BeratGram = beratGram;
        }
    }

    public class PolaDigital : Produk
    {
        public string FormatFile { get; private set; }
        public string LinkDownload { get; private set; }

        public PolaDigital(string nama, decimal harga, string formatFile, string linkDownload)
            : base(nama, harga)
        {
            // Defensive Programming: Pre-condition validasi link digital 
            if (string.IsNullOrWhiteSpace(linkDownload))
            {
                throw new ArgumentException("Link download pola digital tidak boleh kosong.");
            }

            FormatFile = formatFile;
            LinkDownload = linkDownload;
        }
    }
}