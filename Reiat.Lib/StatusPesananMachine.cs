using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reiat.Lib
{
    // Automata
    public enum OrderState { Keranjang, Checkout, Dibayar, Selesai }

    public class StatusPesananMachine
    {
        public OrderState StateSaatIni { get; private set; } = OrderState.Keranjang;

        public void LanjutKeCheckout()
        {
            if (StateSaatIni != OrderState.Keranjang)
                throw new InvalidOperationException("Pesanan belum siap di-checkout."); // DbC
            StateSaatIni = OrderState.Checkout;
        }

        public void ProsesPembayaran()
        {
            if (StateSaatIni != OrderState.Checkout)
                throw new InvalidOperationException("Pesanan harus di-checkout sebelum dibayar."); // DbC
            StateSaatIni = OrderState.Dibayar;
        }

        public void SelesaikanPesanan()
        {
            if (StateSaatIni != OrderState.Dibayar)
                throw new InvalidOperationException("Pesanan belum dibayar."); // DbC
            StateSaatIni = OrderState.Selesai;
        }
    }
}
