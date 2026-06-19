using Reiat.GUI.Forms;
using Reiat.GUI.Helpers;

namespace Reiat.GUI.Panels
{
    public class PromoPanel : UserControl
    {
        private readonly MainForm _main;
        private TextBox txtPromo = null!;
        private Label lblResult = null!;

        public PromoPanel(MainForm main)
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
                Text = "Klaim Kode Promo",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.ColorTextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 24)
            };
            mainFlow.Controls.Add(lblTitle);

            // Access check
            if (_main.UserRole != "Customer")
            {
                var lblDenied = UIHelper.CreateLabel("🚫  Hanya Customer yang dapat menggunakan promo.",
                    UIHelper.FontBody, UIHelper.ColorError);
                lblDenied.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblDenied);
                return;
            }

            if (_main.Keranjang.LihatKeranjang().Count == 0)
            {
                var lblEmpty = UIHelper.CreateLabel("Keranjang masih kosong. Tambahkan produk terlebih dahulu.",
                    UIHelper.FontBody, UIHelper.ColorTextSecondary);
                lblEmpty.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblEmpty);
                return;
            }

            // Promo card
            var card = UIHelper.CreateCard();
            card.MinimumSize = new Size(600, 360);
            card.AutoSize = true;
            card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            card.Margin = new Padding(0, 0, 0, 24);
            mainFlow.Controls.Add(card);

            int innerWidth = 540;

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

            var lblInfo = UIHelper.CreateLabel("Masukkan kode promo untuk mendapatkan diskon:", UIHelper.FontBody, UIHelper.ColorTextPrimary);
            lblInfo.AutoSize = true;
            lblInfo.Margin = new Padding(0, 0, 0, 8);
            cardFlow.Controls.Add(lblInfo);

            var lblHint = UIHelper.CreateLabel("Kode tersedia: REIATBARU (15%), ONGKIRGRATIS (10%), DISKON50 (50%)",
                UIHelper.FontSmall, UIHelper.ColorTextSecondary);
            lblHint.MaximumSize = new Size(innerWidth, 0);
            lblHint.AutoSize = true;
            lblHint.Margin = new Padding(0, 0, 0, 32);
            cardFlow.Controls.Add(lblHint);

            var lblCode = UIHelper.CreateLabel("Kode Promo", UIHelper.FontBody, UIHelper.ColorTextPrimary);
            lblCode.AutoSize = true;
            lblCode.Margin = new Padding(0, 0, 0, 8);
            cardFlow.Controls.Add(lblCode);

            txtPromo = UIHelper.CreateTextBox(innerWidth);
            txtPromo.CharacterCasing = CharacterCasing.Upper;
            txtPromo.Margin = new Padding(0, 0, 0, 24);
            cardFlow.Controls.Add(txtPromo);

            var btnApply = UIHelper.CreatePrimaryButton("Klaim Promo", innerWidth, 46);
            btnApply.Click += BtnApply_Click;
            btnApply.Margin = new Padding(0, 0, 0, 24);
            cardFlow.Controls.Add(btnApply);

            lblResult = new Label
            {
                Size = new Size(innerWidth, 40),
                Font = UIHelper.FontBody,
                ForeColor = UIHelper.ColorSuccess,
                Text = "",
                AutoSize = true,
                Margin = new Padding(0)
            };
            cardFlow.Controls.Add(lblResult);

            // Show current promo if any
            if (!string.IsNullOrEmpty(_main.PromoDipakai))
            {
                lblResult.ForeColor = UIHelper.ColorSuccess;
                lblResult.Text = $"Promo aktif: {_main.PromoDipakai} (Diskon: Rp {_main.DiskonDidapat:N0})";
            }

            txtPromo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnApply_Click(s, e); };
        }

        private void InitializeComponent()
        {

        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            lblResult.Text = "";

            try
            {
                decimal subtotal = _main.Keranjang.HitungTotalHarga();
                decimal diskon = _main.Kalkulator.DapatkanDiskon(txtPromo.Text.Trim(), subtotal);

                _main.OnPromoApplied(txtPromo.Text.Trim().ToUpper(), diskon);

                lblResult.ForeColor = UIHelper.ColorSuccess;
                lblResult.Text = $"✅ Promo berhasil! Diskon: Rp {diskon:N0}";
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = UIHelper.ColorError;
                lblResult.Text = $"❌ {ex.Message}";

                _main.OnPromoApplied("", 0);
            }
        }
    }
}
