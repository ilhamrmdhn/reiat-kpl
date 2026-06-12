using System;

namespace Reiat.Lib
{
    // Teknik Parameterization/Generics: Wrapper dinamis <T>
    public class ResponAPI<T>
    {
        public bool Sukses { get; private set; }
        public string Pesan { get; private set; }
        public T Data { get; private set; }

        public ResponAPI(bool sukses, string pesan, T data)
        {
            // Defensive Programming (DbC): Pre-condition
            // Mencegah API mengembalikan status sukses padahal datanya null/kosong
            if (sukses && data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data tidak boleh null jika status respons sukses.");
            }

            // Mencegah API mengembalikan status gagal tanpa memberikan pesan error yang jelas
            if (!sukses && string.IsNullOrWhiteSpace(pesan))
            {
                throw new ArgumentException("Pesan error harus diisi jika status respons gagal.");
            }

            Sukses = sukses;
            Pesan = pesan;
            Data = data;
        }
    }
}