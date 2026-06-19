using Reiat.Lib;
using Reiat.GUI.Forms;
using Reiat.GUI.Helpers;

namespace Reiat.GUI.Panels
{
    public class DashboardPanel : UserControl
    {
        private readonly MainForm _main;

        public DashboardPanel(MainForm main)
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

            // Greeting
            string greeting = _main.UserRole switch
            {
                "Admin" => "Panel Administrator",
                "Customer" => "Selamat Datang, Customer!",
                _ => "Selamat Datang di REIAT!"
            };

            var lblGreeting = new Label
            {
                Text = greeting,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.ColorTextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 8)
            };
            mainFlow.Controls.Add(lblGreeting);

            string subText = _main.UserRole switch
            {
                "Admin" => "Kelola kategori produk dari panel ini.",
                "Customer" => $"Login sebagai {_main.UserEmail}",
                _ => "Silakan login untuk akses penuh, atau jelajahi katalog sebagai Guest."
            };

            var lblSub = new Label
            {
                Text = subText,
                Font = UIHelper.FontBody,
                ForeColor = UIHelper.ColorTextSecondary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 32)
            };
            mainFlow.Controls.Add(lblSub);

            // === STATUS CARDS ===
            var pnlCards = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 40)
            };
            mainFlow.Controls.Add(pnlCards);

            if (_main.UserRole != "Admin")
            {
                var cardCart = CreateStatusCard("Keranjang",
                    $"{_main.Keranjang.LihatKeranjang().Count} Item",
                    UIHelper.ColorPrimary);
                pnlCards.Controls.Add(cardCart);
            }

            var cardOrder = CreateStatusCard("Status Order",
                _main.Machine.StateSaatIni.ToString(),
                UIHelper.ColorAccent);
            pnlCards.Controls.Add(cardOrder);

            bool isAuth = _main.AuthMachine.StateSaatIni == AuthState.Authenticated;
            var cardAuth = CreateStatusCard("Autentikasi",
                isAuth ? "Authenticated" : "Unauthenticated",
                isAuth ? UIHelper.ColorSuccess : UIHelper.ColorError);
            pnlCards.Controls.Add(cardAuth);

            if (_main.UserRole == "Customer" && !string.IsNullOrEmpty(_main.PromoDipakai))
            {
                var cardPromo = CreateStatusCard("Promo Aktif",
                    $"{_main.PromoDipakai}\n-Rp {_main.DiskonDidapat:N0}",
                    UIHelper.ColorSuccess);
                pnlCards.Controls.Add(cardPromo);
            }

            // === QUICK ACTIONS ===
            var lblActions = new Label
            {
                Text = "Aksi Cepat",
                Font = UIHelper.FontSubtitle,
                ForeColor = UIHelper.ColorTextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 16)
            };
            mainFlow.Controls.Add(lblActions);

            var pnlButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true
            };
            mainFlow.Controls.Add(pnlButtons);

            int btnW = 280, btnH = 46;

            var btnKatalog = UIHelper.CreatePrimaryButton("Lihat Katalog", btnW, btnH);
            btnKatalog.Margin = new Padding(0, 0, 16, 16);
            btnKatalog.Click += (s, e) => _main.NavigateTo("katalog");
            pnlButtons.Controls.Add(btnKatalog);

            if (_main.UserRole != "Admin")
            {
                var btnCart = UIHelper.CreateOutlinedButton("Lihat Keranjang", btnW, btnH);
                btnCart.Margin = new Padding(0, 0, 16, 16);
                btnCart.Click += (s, e) => _main.NavigateTo("keranjang");
                pnlButtons.Controls.Add(btnCart);
            }

            if (_main.UserRole == "Guest")
            {
                var btnLogin = UIHelper.CreateOutlinedButton("Login Sekarang", btnW, btnH);
                btnLogin.Margin = new Padding(0, 0, 16, 16);
                btnLogin.Click += (s, e) => _main.NavigateTo("login");
                pnlButtons.Controls.Add(btnLogin);
            }

            if (_main.UserRole == "Admin")
            {
                var btnKat = UIHelper.CreateOutlinedButton("Kelola Kategori", btnW, btnH);
                btnKat.Margin = new Padding(0, 0, 16, 16);
                btnKat.Click += (s, e) => _main.NavigateTo("kategori");
                pnlButtons.Controls.Add(btnKat);
            }
        }

        private Panel CreateStatusCard(string title, string value, Color accentColor)
        {
            var card = UIHelper.CreateCard();
            card.AutoSize = true;
            card.MinimumSize = new Size(280, 120);
            card.Margin = new Padding(0, 0, 16, 16);

            var lblTitle = new Label
            {
                Text = title,
                Font = UIHelper.FontSmall,
                ForeColor = UIHelper.ColorTextSecondary,
                Location = new Point(20, 20),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = UIHelper.FontSubtitle,
                ForeColor = accentColor,
                Location = new Point(20, 50),
                AutoSize = true
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            return card;
        }
    }
}
