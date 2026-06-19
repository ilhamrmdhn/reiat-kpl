using Reiat.GUI.Forms;
using Reiat.GUI.Helpers;

namespace Reiat.GUI.Panels
{
    public class KategoriPanel : UserControl
    {
        private readonly MainForm _main;
        private ListBox lstKategori = null!;
        private TextBox txtKatBaru = null!;
        private Label lblResult = null!;

        public KategoriPanel(MainForm main)
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
                Text = "Kelola Master Kategori",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.ColorTextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 8)
            };
            mainFlow.Controls.Add(lblTitle);

            var lblDesc = UIHelper.CreateLabel("Menggunakan teknik Generics (PenyimpananLokal<T>)",
                UIHelper.FontSmall, UIHelper.ColorTextSecondary);
            lblDesc.Margin = new Padding(0, 0, 0, 24);
            mainFlow.Controls.Add(lblDesc);

            if (_main.UserRole != "Admin")
            {
                var lblDenied = UIHelper.CreateLabel("🚫  Hanya Admin yang diizinkan untuk mengelola Master Kategori.",
                    UIHelper.FontBody, UIHelper.ColorError);
                lblDenied.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblDenied);
                return;
            }

            // List card
            var card = UIHelper.CreateCard();
            card.MinimumSize = new Size(600, 360);
            card.AutoSize = true;
            card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            card.Margin = new Padding(0, 0, 0, 24);
            mainFlow.Controls.Add(card);

            var cardFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(24)
            };
            card.Controls.Add(cardFlow);

            int innerWidth = 540;

            var lblList = UIHelper.CreateLabel("Daftar Kategori Produk Saat Ini:", UIHelper.FontTitle, UIHelper.ColorTextPrimary);
            lblList.AutoSize = true;
            lblList.Margin = new Padding(0, 0, 0, 16);
            cardFlow.Controls.Add(lblList);

            lstKategori = new ListBox
            {
                Size = new Size(innerWidth, 200),
                Font = UIHelper.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = UIHelper.ColorBackground,
                Margin = new Padding(0, 0, 0, 32)
            };
            RefreshList();
            cardFlow.Controls.Add(lstKategori);

            // Add new
            var lblNew = UIHelper.CreateLabel("Tambah Kategori Baru:", UIHelper.FontTitle, UIHelper.ColorTextPrimary);
            lblNew.AutoSize = true;
            lblNew.Margin = new Padding(0, 0, 0, 16);
            cardFlow.Controls.Add(lblNew);

            var pnlInput = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 16)
            };

            txtKatBaru = UIHelper.CreateTextBox(380);
            txtKatBaru.Margin = new Padding(0, 0, 16, 0);
            pnlInput.Controls.Add(txtKatBaru);

            var btnAdd = UIHelper.CreatePrimaryButton("Tambah", 140, 46);
            btnAdd.Margin = new Padding(0);
            btnAdd.Click += BtnAdd_Click;
            pnlInput.Controls.Add(btnAdd);

            cardFlow.Controls.Add(pnlInput);

            lblResult = new Label
            {
                Size = new Size(innerWidth, 24),
                Font = UIHelper.FontSmall,
                Text = "",
                AutoSize = true,
                Margin = new Padding(0)
            };
            cardFlow.Controls.Add(lblResult);

            txtKatBaru.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAdd_Click(s, e); };
        }

        private void RefreshList()
        {
            lstKategori.Items.Clear();
            foreach (var kat in _main.PenyimpananKategori.AmbilSemua())
            {
                lstKategori.Items.Add(kat);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            string nama = txtKatBaru.Text.Trim();
            if (string.IsNullOrWhiteSpace(nama))
            {
                lblResult.ForeColor = UIHelper.ColorError;
                lblResult.Text = "❌ Nama kategori tidak boleh kosong.";
                return;
            }

            try
            {
                _main.PenyimpananKategori.Simpan($"Kategori: {nama}");
                RefreshList();
                txtKatBaru.Clear();
                lblResult.ForeColor = UIHelper.ColorSuccess;
                lblResult.Text = $"✅ Kategori \"{nama}\" berhasil disimpan (Generics).";
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = UIHelper.ColorError;
                lblResult.Text = $"❌ {ex.Message}";
            }
        }
    }
}
