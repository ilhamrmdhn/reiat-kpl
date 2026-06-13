using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Reiat.Lib;

namespace Reiat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KatalogController : ControllerBase
    {
        // Teknik API: Endpoint GET data produk
        [HttpGet]
        public IActionResult GetKatalog()
        {
            // Simulasi data dari database
            var produk = new List<object>
            {
                new { Id = 1, Nama = "Kemeja Reiat Basic", Tipe = "Fisik", Harga = 150000 },
                new { Id = 2, Nama = "Pola Jahit Celana Cargo", Tipe = "Digital", Harga = 45000 }
            };

            // Menggunakan Generic Wrapper buatan Reja agar rapi
            var respon = new ResponAPI<List<object>>(true, "Berhasil memuat katalog Reiat", produk);

            return Ok(respon);
        }
    }
}