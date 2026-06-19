using Reiat.GUI.Forms;
using Reiat.GUI.Helpers;

namespace Reiat.GUI.Panels
{
    public class KeranjangPanel : UserControl
    {
        private readonly MainForm _main;

        public KeranjangPanel(MainForm main)
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
                Text = "Keranjang Belanja",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.ColorTextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 24)
            };
            mainFlow.Controls.Add(lblTitle);

            if (_main.UserRole == "Admin")
            {
                var lblDenied = UIHelper.CreateLabel("🚫  Fitur ini khusus Customer.", UIHelper.FontBody, UIHelper.ColorError);
                lblDenied.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblDenied);
                return;
            }

            var items = _main.Keranjang.LihatKeranjang();

            if (items.Count == 0)
            {
                var lblEmpty = UIHelper.CreateLabel("Keranjang belanja Anda masih kosong.\nSilakan tambahkan produk dari menu Katalog.",
                    UIHelper.FontBody, UIHelper.ColorTextSecondary);
                lblEmpty.Margin = new Padding(0, 0, 0, 24);
                mainFlow.Controls.Add(lblEmpty);

                var btnToKatalog = UIHelper.CreatePrimaryButton("Lihat Katalog", 220, 46);
                btnToKatalog.Click += (s, e) => _main.NavigateTo("katalog");
                mainFlow.Controls.Add(btnToKatalog);
                return;
            }

            // Item list
            var pnlList = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 16)
            };
            mainFlow.Controls.Add(pnlList);

            int no = 1;
            foreach (var prod in items)
            {
                var card = UIHelper.CreateCard();
                card.Size = new Size(1200, 140);
                card.Margin = new Padding(0, 0, 0, 8);

                var tlp = new TableLayoutPanel
                {
                    ColumnCount = 3,
                    RowCount = 1,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Padding = new Padding(0, 4, 0, 4)
                };

                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // No
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Nama
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Harga

                var lblNo = UIHelper.CreateLabel($"{no}.", UIHelper.FontSubtitle, UIHelper.ColorTextSecondary);
                lblNo.Anchor = AnchorStyles.Left;
                tlp.Controls.Add(lblNo, 0, 0);

                var lblNama = UIHelper.CreateLabel(prod.Nama, UIHelper.FontSubtitle, UIHelper.ColorTextPrimary);
                lblNama.Anchor = AnchorStyles.Left;
                tlp.Controls.Add(lblNama, 1, 0);

                var lblHarga = UIHelper.CreateLabel($"Rp {prod.Harga:N0}", UIHelper.FontSubtitle, UIHelper.ColorPrimary);
                lblHarga.Anchor = AnchorStyles.Right;
                lblHarga.Margin = new Padding(0, 0, 24, 0);
                tlp.Controls.Add(lblHarga, 2, 0);

                card.Controls.Add(tlp);
                pnlList.Controls.Add(card);
                no++;
            }

            // Subtotal
            var cardTotal = UIHelper.CreateCard();
            cardTotal.Size = new Size(1200, 140);
            cardTotal.Margin = new Padding(0, 0, 0, 32);

            var tlpTotal = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0, 4, 0, 4)
            };

            tlpTotal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Label
            tlpTotal.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Value

            var lblSubLabel = UIHelper.CreateLabel("Subtotal:", UIHelper.FontTitle, UIHelper.ColorTextPrimary);
            lblSubLabel.Anchor = AnchorStyles.Left;
            tlpTotal.Controls.Add(lblSubLabel, 0, 0);

            var lblSubValue = UIHelper.CreateLabel($"Rp {_main.Keranjang.HitungTotalHarga():N0}",
                UIHelper.FontTitle, UIHelper.ColorPrimary);
            lblSubValue.Anchor = AnchorStyles.Right;
            lblSubValue.Margin = new Padding(0, 0, 24, 0);
            tlpTotal.Controls.Add(lblSubValue, 1, 0);

            cardTotal.Controls.Add(tlpTotal);
            mainFlow.Controls.Add(cardTotal);

            // Action buttons
            if (_main.UserRole == "Customer")
            {
                var pnlButtons = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true,
                    AutoSize = true
                };
                mainFlow.Controls.Add(pnlButtons);

                var btnCheckout = UIHelper.CreatePrimaryButton("Lanjut ke Checkout", 280, 46);
                btnCheckout.Margin = new Padding(0, 0, 16, 16);
                btnCheckout.Click += (s, e) => _main.NavigateTo("checkout");
                pnlButtons.Controls.Add(btnCheckout);

                var btnPromo = UIHelper.CreateOutlinedButton("Klaim Promo Dulu", 260, 46);
                btnPromo.Margin = new Padding(0, 0, 16, 16);
                btnPromo.Click += (s, e) => _main.NavigateTo("promo");
                pnlButtons.Controls.Add(btnPromo);
            }
        }
    }
}
