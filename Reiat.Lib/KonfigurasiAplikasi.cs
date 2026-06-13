using System;

namespace Reiat.Lib
{
    public class KonfigurasiAplikasi
    {
        public decimal Ppn { get; private set; }

        public KonfigurasiAplikasi()
        {
            // Teknik Runtime Configuration:
            // Mengatur konfigurasi PPN ke 11% saat program diinisialisasi
            Ppn = 0.11m;
        }

        public decimal HitungPpn(decimal nominal)
        {
            // Defensive Programming (DbC): Pre-condition
            if (nominal < 0)
            {
                throw new ArgumentException("Nominal untuk dihitung PPN tidak boleh negatif.");
            }

            return nominal * Ppn;
        }
    }
}