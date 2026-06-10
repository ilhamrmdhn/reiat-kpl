using System;
using System.Collections.Generic;

namespace Reiat.Lib
{
    public class KalkulatorDiskon
    {
        private readonly Dictionary<string, decimal> _tabelPromo;

        public KalkulatorDiskon()
        {
            // Teknik Table-Driven Construction:
            // Menyimpan aturan/rule diskon dalam tabel memori
            _tabelPromo = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                { "REIATBARU", 0.15m },   // Diskon 15%
                { "ONGKIRGRATIS", 0.10m },// Diskon 10%
                { "DISKON50", 0.50m }     // Diskon 50%
            };
        }

        public decimal DapatkanDiskon(string kodePromo, decimal totalBelanja)
        {
            // Defensive Programming (DbC): Pre-condition
            if (string.IsNullOrWhiteSpace(kodePromo))
            {
                throw new ArgumentException("Kode promo tidak boleh kosong.");
            }
            if (totalBelanja < 0)
            {
                throw new ArgumentException("Total belanja tidak valid untuk diberi diskon.");
            }

            // Melakukan pencarian ke dalam tabel (Table-driven)
            if (_tabelPromo.TryGetValue(kodePromo, out decimal persentaseDiskon))
            {
                return totalBelanja * persentaseDiskon;
            }

            // Defensive Programming (DbC): Post-condition / Penanganan jika data tidak di tabel
            throw new ArgumentException("Kode promo tidak valid atau sudah kadaluarsa.");
        }
    }
}