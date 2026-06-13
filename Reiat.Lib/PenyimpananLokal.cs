using System;
using System.Collections.Generic;

namespace Reiat.Lib
{
    // Teknik Parameterization/Generics: Menggunakan <T>
    public class PenyimpananLokal<T>
    {
        private readonly List<T> _data;

        public PenyimpananLokal()
        {
            _data = new List<T>();
        }

        public void Simpan(T item)
        {
            // Defensive Programming (DbC): Pre-condition mencegah insert null
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Data yang disimpan tidak boleh null.");
            }

            _data.Add(item);
        }

        public IReadOnlyList<T> AmbilSemua()
        {
            return _data.AsReadOnly();
        }
    }
}