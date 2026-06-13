using Microsoft.AspNetCore.Mvc;

namespace Reiat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OngkirController : ControllerBase
    {
        [HttpGet("cek")]
        public IActionResult CekOngkir(string kotaTujuan, double beratGram)
        {
            // Defensive Programming: DbC pada endpoint API 
            if (string.IsNullOrWhiteSpace(kotaTujuan))
            {
                return BadRequest("Kota tujuan tidak boleh kosong.");
            }

            if (beratGram <= 0)
            {
                return BadRequest("Berat barang tidak valid, harus lebih dari 0.");
            }

            decimal tarifPerGram = 15;
            decimal totalOngkir = (decimal)beratGram * tarifPerGram;

            return Ok(new
            {
                Tujuan = kotaTujuan,
                BeratGram = beratGram,
                TotalBiayaKirim = totalOngkir
            });
        }
    }
}