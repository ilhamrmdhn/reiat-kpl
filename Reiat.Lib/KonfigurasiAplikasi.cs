using System;
using System.IO;
using System.Text.Json;

namespace Reiat.Lib
{
    public class KonfigurasiAplikasi
    {
        public decimal Ppn { get; private set; }

        // Class bantuan untuk menampung format JSON
        private class ConfigModel
        {
            public decimal Ppn { get; set; }
        }

        public KonfigurasiAplikasi()
        {
            string configFilePath = "config.json";

            // Teknik Runtime Configuration: Membaca setting dari file JSON di luar kode program
            if (File.Exists(configFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(configFilePath);
                    var config = JsonSerializer.Deserialize<ConfigModel>(jsonString);
                    Ppn = config.Ppn;
                }
                catch
                {
                    // Fallback jika isi JSON rusak/error
                    Ppn = 0.11m;
                }
            }
            else
            {
                // Jika file config.json belum ada, program akan membuatnya secara otomatis
                Ppn = 0.11m;
                var defaultConfig = new ConfigModel { Ppn = 0.11m };

                // Menyimpan file JSON dengan format yang rapi
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(configFilePath, JsonSerializer.Serialize(defaultConfig, options));
            }
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