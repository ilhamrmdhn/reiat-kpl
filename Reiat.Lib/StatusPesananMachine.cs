using System;

namespace Reiat.Lib
{
    public enum OrderState { Keranjang, Checkout, Dibayar, Selesai }

    public class StatusPesananMachine
    {
        public OrderState StateSaatIni { get; private set; }

        public StatusPesananMachine()
        {
            StateSaatIni = OrderState.Keranjang;
        }

        public void LanjutKeCheckout()
        {
            // Defensive Programming (DbC): Pre-condition
            if (StateSaatIni != OrderState.Keranjang)
            {
                throw new InvalidOperationException("Pesanan belum siap di-checkout atau sudah lewat fase ini.");
            }

            StateSaatIni = OrderState.Checkout;
        }

        public void ProsesPembayaran()
        {
            // Defensive Programming (DbC): Pre-condition
            if (StateSaatIni != OrderState.Checkout)
            {
                throw new InvalidOperationException("Pesanan harus di-checkout sebelum dibayar.");
            }

            StateSaatIni = OrderState.Dibayar;
        }

        public void SelesaikanPesanan()
        {
            // Defensive Programming (DbC): Pre-condition
            if (StateSaatIni != OrderState.Dibayar)
            {
                throw new InvalidOperationException("Pesanan belum dibayar, tidak bisa diselesaikan.");
            }

            StateSaatIni = OrderState.Selesai;
        }
    }
}