using System.Net.Http;
using System.Text.Json;
using Reiat.Lib;
using Reiat.GUI.Forms;
using Reiat.GUI.Helpers;

namespace Reiat.GUI.Panels
{
    public class KatalogPanel : UserControl
    {
        private readonly MainForm _main;
        private FlowLayoutPanel pnlProducts = null!;
        private Label lblStatusApi = null!;

        public KatalogPanel(MainForm main)
        {
            _main = main;
            BackColor = UIHelper.ColorBackground;
            Dock = DockStyle.Fill;
            AutoScroll = true;
            BuildUI();
            _ = LoadKatalogAsync();
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
                Text = "Katalog Produk",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.ColorTextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 8)
            };
            mainFlow.Controls.Add(lblTitle);

            var lblDesc = UIHelper.CreateLabel("Data produk dimuat dari Reiat.API via HTTP GET", UIHelper.FontSmall, UIHelper.ColorTextSecondary);
            lblDesc.Margin = new Padding(0, 0, 0, 16);
            mainFlow.Controls.Add(lblDesc);

            lblStatusApi = UIHelper.CreateLabel("⏳ Memuat data dari API...", UIHelper.FontSmall, UIHelper.ColorAccent);
            lblStatusApi.Margin = new Padding(0, 0, 0, 24);
            mainFlow.Controls.Add(lblStatusApi);

            // Refresh button
            var btnRefresh = UIHelper.CreateOutlinedButton("Refresh Data", 200, 42);
            btnRefresh.Margin = new Padding(0, 0, 0, 24);
            btnRefresh.Click += async (s, e) => await LoadKatalogAsync();
            mainFlow.Controls.Add(btnRefresh);

            pnlProducts = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };
            mainFlow.Controls.Add(pnlProducts);
        }

        private async Task LoadKatalogAsync()
        {
            pnlProducts.Controls.Clear();
            lblStatusApi.Text = "⏳ Memuat data dari API...";
            lblStatusApi.ForeColor = UIHelper.ColorAccent;

            try
            {
                var response = await _main.ApiClient.GetAsync($"{_main.BaseUrlApi}/api/Katalog");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    bool sukses = root.GetProperty("sukses").GetBoolean();
                    string pesan = root.GetProperty("pesan").GetString() ?? "";

                    lblStatusApi.Text = $"✅ API: {pesan}";
                    lblStatusApi.ForeColor = UIHelper.ColorSuccess;

                    foreach (var item in root.GetProperty("data").EnumerateArray())
                    {
                        int id = item.GetProperty("id").GetInt32();
                        string nama = item.GetProperty("nama").GetString() ?? "";
                        string tipe = item.GetProperty("tipe").GetString() ?? "";
                        decimal harga = item.GetProperty("harga").GetDecimal();

                        var row = CreateProductRow(id, nama, tipe, harga);
                        pnlProducts.Controls.Add(row);
                    }
                }
                else
                {
                    lblStatusApi.Text = $"❌ API Error: {response.StatusCode}";
                    lblStatusApi.ForeColor = UIHelper.ColorError;
                }
            }
            catch (HttpRequestException)
            {
                lblStatusApi.Text = "❌ Gagal terhubung ke API. Pastikan Reiat.API sedang berjalan!";
                lblStatusApi.ForeColor = UIHelper.ColorError;
            }
            catch (Exception ex)
            {
                lblStatusApi.Text = $"❌ Error: {ex.Message}";
                lblStatusApi.ForeColor = UIHelper.ColorError;
            }
        }

        private Panel CreateProductRow(int id, string nama, string tipe, decimal harga)
        {
            var card = UIHelper.CreateCard();
            card.Size = new Size(1200, 140);
            card.Margin = new Padding(0, 0, 0, 12);

            var tlp = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 1,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 8)
            };
            
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // ID
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Nama & Tipe
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Harga
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Tombol

            var lblId = UIHelper.CreateLabel($"#{id}", UIHelper.FontBodyBold, UIHelper.ColorTextSecondary);
            lblId.Anchor = AnchorStyles.Left;
            tlp.Controls.Add(lblId, 0, 0);

            var pnlName = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                WrapContents = false
            };
            var lblNama = UIHelper.CreateLabel(nama, UIHelper.FontSubtitle, UIHelper.ColorTextPrimary);
            pnlName.Controls.Add(lblNama);
            
            string tipeBadge = tipe == "Fisik" ? "📦 Fisik" : "💾 Digital";
            var lblTipe = UIHelper.CreateLabel(tipeBadge, UIHelper.FontSmall, UIHelper.ColorTextSecondary);
            pnlName.Controls.Add(lblTipe);
            tlp.Controls.Add(pnlName, 1, 0);

            var lblHarga = UIHelper.CreateLabel($"Rp {harga:N0}", UIHelper.FontSubtitle, UIHelper.ColorPrimary);
            lblHarga.Anchor = AnchorStyles.Right;
            lblHarga.Margin = new Padding(0, 0, 24, 0);
            tlp.Controls.Add(lblHarga, 2, 0);

            bool isAdmin = _main.UserRole == "Admin";
            var btnAdd = UIHelper.CreatePrimaryButton(isAdmin ? "🚫 Admin" : "Tambah", 160, 42);
            btnAdd.Anchor = AnchorStyles.Right;
            btnAdd.Enabled = !isAdmin;

            if (!isAdmin)
            {
                btnAdd.Click += (s, e) =>
                {
                    try
                    {
                        Produk produk;
                        if (tipe == "Fisik")
                            produk = new PakaianFisik(nama, harga, "M", 200);
                        else
                            produk = new PolaDigital(nama, harga, "PDF", $"https://reiat.com/dl/{id}");

                        _main.Keranjang.TambahProduk(produk);
                        _main.OnCartUpdated();
                        MessageBox.Show($"{nama} berhasil ditambahkan ke keranjang!",
                            "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Gagal: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
            }

            tlp.Controls.Add(btnAdd, 3, 0);
            card.Controls.Add(tlp);
            
            return card;
        }
    }
}
