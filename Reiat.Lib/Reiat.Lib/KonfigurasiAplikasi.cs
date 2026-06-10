using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reiat.Lib.Reiat.Lib
{
    // Teknik Runtime Configuration
    public class KonfigurasiAplikasi
    {
        public decimal Ppn { get; private set; }

        public KonfigurasiAplikasi()
        {
            // Simulasi membaca dari file config saat runtime
            Ppn = 0.11m; // 11%
        }

        public decimal HitungPpn(decimal nominal)
        {
            if (nominal < 0) throw new ArgumentException("Nominal tidak boleh negatif."); // DbC
            return nominal * Ppn;
        }
    }
}
