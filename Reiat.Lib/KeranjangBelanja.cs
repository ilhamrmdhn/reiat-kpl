using System;
using System.Collections.Generic;

namespace Reiat.Lib
{
    public class KeranjangBelanja
    {
        private readonly List<Produk> _items;

        public KeranjangBelanja()
        {
            _items = new List<Produk>();
        }

        public void TambahProduk(Produk produkBaru)
        {
            // Defensive Programming: Pre-condition mencegah input null 
            if (produkBaru == null)
            {
                throw new ArgumentNullException(nameof(produkBaru), "Produk tidak boleh null.");
            }

            _items.Add(produkBaru);
        }

        public IReadOnlyList<Produk> LihatKeranjang()
        {
            return _items.AsReadOnly();
        }

        public decimal HitungTotalHarga()
        {
            decimal total = 0;
            foreach (var item in _items)
            {
                total += item.Harga;
            }

            // Defensive Programming: Post-condition memastikan total valid 
            if (total < 0)
            {
                throw new InvalidOperationException("Sistem Error: Total harga tidak boleh negatif.");
            }

            return total;
        }
    }
}