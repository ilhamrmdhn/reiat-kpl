using System.Net.Http;
using System.Text.Json;
using Reiat.GUI.Forms;
using Reiat.GUI.Helpers;

namespace Reiat.GUI.Panels
{
    public class CheckoutPanel : UserControl
    {
        private readonly MainForm _main;
        private TextBox txtKota = null!;
        private Label lblOngkir = null!;
        private Panel pnlNota = null!;
        private Button btnBayar = null!;

        private decimal ongkir = 0;
        private bool ongkirLoaded = false;

        public CheckoutPanel(MainForm main)
        {
            _main = main;
            BackColor = UIHelper.ColorBackground;
            Dock = DockStyle.Fill;
            AutoScroll = true;
            BuildUI();
        }

        private void BuildUI()
        {
            var mainFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(30, 24, 30, 24)
            };
            Controls.Add(mainFlow);

            var lblTitle = new Label
            {
                Text = "Checkout Pembayaran",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.ColorTextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 24)
            };
            mainFlow.Controls.Add(lblTitle);

            // Access checks
            if (_main.UserRole != "Customer")
            {
                var lblDenied = UIHelper.CreateLabel("🚫  Anda harus login sebagai Customer untuk checkout.",
                    UIHelper.FontBody, UIHelper.ColorError);
                lblDenied.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblDenied);
                return;
            }
            if (_main.Keranjang.LihatKeranjang().Count == 0)
            {
                var lblEmpty = UIHelper.CreateLabel("Tidak ada barang di keranjang untuk dicheckout.",
                    UIHelper.FontBody, UIHelper.ColorTextSecondary);
                lblEmpty.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblEmpty);
                return;
            }
            if (_main.SudahBayar)
            {
                var lblDone = UIHelper.CreateLabel("✅  Pesanan ini sudah selesai diproses.",
                    UIHelper.FontBody, UIHelper.ColorSuccess);
                lblDone.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblDone);
                return;
            }

            // === Order Summary ===
            var cardSummary = UIHelper.CreateCard();
            cardSummary.AutoSize = true;
            cardSummary.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cardSummary.MinimumSize = new Size(1200, 100);
            cardSummary.Margin = new Padding(0, 0, 0, 24);
            mainFlow.Controls.Add(cardSummary);

            var summaryFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(24)
            };
            cardSummary.Controls.Add(summaryFlow);

            var lblSumTitle = UIHelper.CreateLabel("Ringkasan Pesanan:", UIHelper.FontTitle, UIHelper.ColorTextPrimary);
            lblSumTitle.AutoSize = true;
            lblSumTitle.Margin = new Padding(0, 0, 0, 16);
            summaryFlow.Controls.Add(lblSumTitle);

            foreach (var prod in _main.Keranjang.LihatKeranjang())
            {
                var lbl = UIHelper.CreateLabel($"• {prod.Nama}  —  Rp {prod.Harga:N0}", UIHelper.FontBody, UIHelper.ColorTextPrimary);
                lbl.AutoSize = true;
                lbl.Margin = new Padding(0, 0, 0, 8);
                summaryFlow.Controls.Add(lbl);
            }

            // === Ongkir Section ===
            var cardOngkir = UIHelper.CreateCard();
            cardOngkir.AutoSize = true;
            cardOngkir.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cardOngkir.MinimumSize = new Size(1200, 160);
            cardOngkir.Margin = new Padding(0, 0, 0, 24);
            mainFlow.Controls.Add(cardOngkir);

            var ongkirFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(24)
            };
            cardOngkir.Controls.Add(ongkirFlow);

            var lblKotaLabel = UIHelper.CreateLabel("Kota Tujuan Pengiriman:", UIHelper.FontBody, UIHelper.ColorTextPrimary);
            lblKotaLabel.AutoSize = true;
            lblKotaLabel.Margin = new Padding(0, 0, 0, 12);
            ongkirFlow.Controls.Add(lblKotaLabel);

            var pnlOngkirInput = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 12)
            };

            txtKota = UIHelper.CreateTextBox(400);
            txtKota.Margin = new Padding(0, 0, 16, 0);
            pnlOngkirInput.Controls.Add(txtKota);

            var btnCekOngkir = UIHelper.CreatePrimaryButton("Hitung Ongkir", 200, 46);
            btnCekOngkir.Margin = new Padding(0);
            btnCekOngkir.Click += async (s, e) => await CekOngkirAsync();
            pnlOngkirInput.Controls.Add(btnCekOngkir);

            ongkirFlow.Controls.Add(pnlOngkirInput);

            lblOngkir = new Label
            {
                Size = new Size(1100, 30),
                Font = UIHelper.FontSmall,
                ForeColor = UIHelper.ColorTextSecondary,
                Text = "Berat barang diasumsikan 1000 gram.",
                AutoSize = true,
                Margin = new Padding(0)
            };
            ongkirFlow.Controls.Add(lblOngkir);

            // === NOTA (invoice) ===
            pnlNota = UIHelper.CreateCard();
            pnlNota.AutoSize = true;
            pnlNota.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlNota.MinimumSize = new Size(1200, 200);
            pnlNota.Margin = new Padding(0, 0, 0, 32);
            pnlNota.Visible = false;
            mainFlow.Controls.Add(pnlNota);

            // === PAY BUTTON ===
            btnBayar = UIHelper.CreatePrimaryButton("Bayar Sekarang", 1200, 50);
            btnBayar.Margin = new Padding(0, 0, 0, 40);
            btnBayar.Visible = false;
            btnBayar.Click += BtnBayar_Click;
            mainFlow.Controls.Add(btnBayar);
        }

        private async Task CekOngkirAsync()
        {
            string kota = txtKota.Text.Trim();
            if (string.IsNullOrWhiteSpace(kota))
            {
                lblOngkir.ForeColor = UIHelper.ColorError;
                lblOngkir.Text = "❌ Kota tujuan tidak boleh kosong.";
                return;
            }

            lblOngkir.ForeColor = UIHelper.ColorAccent;
            lblOngkir.Text = "⏳ Menghitung ongkir...";

            try
            {
                double beratGram = 1000;
                string endpoint = $"{_main.BaseUrlApi}/api/Ongkir/cek?kotaTujuan={kota}&beratGram={beratGram}";
                var response = await _main.ApiClient.GetAsync(endpoint);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    ongkir = root.GetProperty("totalBiayaKirim").GetDecimal();
                    string tujuan = root.GetProperty("tujuan").GetString() ?? kota;

                    lblOngkir.ForeColor = UIHelper.ColorSuccess;
                    lblOngkir.Text = $"✅ Biaya kirim ke {tujuan} = Rp {ongkir:N0}";

                    ongkirLoaded = true;
                    BuildNota();
                }
                else
                {
                    lblOngkir.ForeColor = UIHelper.ColorError;
                    lblOngkir.Text = $"❌ API Error: {json}";
                }
            }
            catch (HttpRequestException)
            {
                lblOngkir.ForeColor = UIHelper.ColorError;
                lblOngkir.Text = "❌ Gagal terhubung ke API Ongkir. Pastikan Reiat.API berjalan!";
            }
            catch (Exception ex)
            {
                lblOngkir.ForeColor = UIHelper.ColorError;
                lblOngkir.Text = $"❌ {ex.Message}";
            }
        }

        private void BuildNota()
        {
            pnlNota.Controls.Clear();
            
            var notaFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(24)
            };
            pnlNota.Controls.Add(notaFlow);

            decimal subtotal = _main.Keranjang.HitungTotalHarga();
            decimal diskon = _main.DiskonDidapat;
            decimal setelahDiskon = subtotal - diskon;
            decimal ppn = _main.Config.HitungPpn(setelahDiskon);
            decimal grandTotal = setelahDiskon + ppn + ongkir;

            var lblNotaTitle = UIHelper.CreateLabel("═══════  NOTA PEMBAYARAN  ═══════", UIHelper.FontTitle, UIHelper.ColorTextPrimary);
            lblNotaTitle.AutoSize = true;
            lblNotaTitle.Margin = new Padding(0, 0, 0, 24);
            notaFlow.Controls.Add(lblNotaTitle);

            var tlp = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 5,
                AutoSize = true,
                Width = 1100,
                Padding = new Padding(0)
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            AddNotaRowTlp(tlp, "Subtotal Belanja", $"Rp {subtotal:N0}", 0);
            AddNotaRowTlp(tlp, "Potongan Diskon", $"-Rp {diskon:N0}", 1, diskon > 0 ? UIHelper.ColorSuccess : UIHelper.ColorTextSecondary);
            AddNotaRowTlp(tlp, "Harga Setelah Diskon", $"Rp {setelahDiskon:N0}", 2);
            AddNotaRowTlp(tlp, "PPN (11%)", $"Rp {ppn:N0}", 3);
            AddNotaRowTlp(tlp, "Ongkos Kirim", $"Rp {ongkir:N0}", 4);
            
            notaFlow.Controls.Add(tlp);

            var separator = new Label
            {
                Size = new Size(1100, 2),
                BackColor = UIHelper.ColorBorder,
                Margin = new Padding(0, 16, 0, 16)
            };
            notaFlow.Controls.Add(separator);

            var tlpGrand = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Width = 1100,
                Padding = new Padding(0)
            };
            tlpGrand.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlpGrand.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblGrandLabel = UIHelper.CreateLabel("GRAND TOTAL", UIHelper.FontTitle, UIHelper.ColorTextPrimary);
            lblGrandLabel.Anchor = AnchorStyles.Left;
            tlpGrand.Controls.Add(lblGrandLabel, 0, 0);

            var lblGrandValue = UIHelper.CreateLabel($"Rp {grandTotal:N0}", UIHelper.FontTitle, UIHelper.ColorPrimary);
            lblGrandValue.Anchor = AnchorStyles.Right;
            tlpGrand.Controls.Add(lblGrandValue, 1, 0);

            notaFlow.Controls.Add(tlpGrand);

            pnlNota.Visible = true;
            btnBayar.Visible = true;
        }

        private void AddNotaRowTlp(TableLayoutPanel tlp, string label, string value, int row, Color? valueColor = null)
        {
            var lbl = UIHelper.CreateLabel(label, UIHelper.FontBody, UIHelper.ColorTextPrimary);
            lbl.Anchor = AnchorStyles.Left;
            lbl.Margin = new Padding(0, 0, 0, 12);
            tlp.Controls.Add(lbl, 0, row);

            var val = UIHelper.CreateLabel(value, UIHelper.FontBodyBold, valueColor ?? UIHelper.ColorTextPrimary);
            val.Anchor = AnchorStyles.Right;
            val.Margin = new Padding(0, 0, 0, 12);
            tlp.Controls.Add(val, 1, row);
        }

        private void BtnBayar_Click(object? sender, EventArgs e)
        {
            try
            {
                _main.Machine.LanjutKeCheckout();
                _main.Machine.ProsesPembayaran();
                _main.Machine.SelesaikanPesanan();
                _main.OnPaymentComplete();

                btnBayar.Enabled = false;
                btnBayar.Text = "✅ Sudah Dibayar";
                btnBayar.BackColor = UIHelper.ColorSuccess;

                MessageBox.Show(
                    $"Pembayaran berhasil!\n\nStatus Order: {_main.Machine.StateSaatIni}\n\nTerima kasih telah berbelanja di platform Reiat!",
                    "Pembayaran Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _main.RefreshHeader();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal checkout: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
