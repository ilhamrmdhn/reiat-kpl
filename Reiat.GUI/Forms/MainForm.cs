using System.ComponentModel;
using System.Net.Http;
using Reiat.Lib;
using Reiat.GUI.Helpers;
using Reiat.GUI.Panels;

namespace Reiat.GUI.Forms
{
    public class MainForm : Form
    {
        // === SHARED STATE (identik dengan Reiat.Main) ===
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KonfigurasiAplikasi Config { get; private set; } = null!;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StatusPesananMachine Machine { get; private set; } = null!;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KeranjangBelanja Keranjang { get; private set; } = null!;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KalkulatorDiskon Kalkulator { get; private set; } = null!;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PenyimpananLokal<string> PenyimpananKategori { get; private set; } = null!;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AutentikasiMachine AuthMachine { get; set; } = null!;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HttpClient ApiClient { get; private set; } = null!;

        public string BaseUrlApi => "http://localhost:5241";
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UserEmail { get; set; } = "";
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UserRole { get; set; } = "Guest";
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PromoDipakai { get; set; } = "";
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal DiskonDidapat { get; set; } = 0;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SudahBayar { get; set; } = false;

        // === UI CONTROLS ===
        private Panel pnlHeader = null!;
        private Panel pnlSidebar = null!;
        private Panel pnlContent = null!;
        private Panel pnlStatusBar = null!;

        private Label lblLogo = null!;
        private Label lblRole = null!;
        private Label lblAuth = null!;
        private Label lblCart = null!;
        private Label lblStatus = null!;

        private Button btnHome = null!;
        private Button btnLogin = null!;
        private Button btnKatalog = null!;
        private Button btnKeranjang = null!;
        private Button btnPromo = null!;
        private Button btnKategori = null!;
        private Button btnCheckout = null!;
        private Button btnLogout = null!;

        private Button[] AllNavButtons => new[] { btnHome, btnLogin, btnKatalog, btnKeranjang, btnPromo, btnKategori, btnCheckout };

        public MainForm()
        {
            InitializeState();
            BuildUI();
            RefreshSidebar();
            RefreshHeader();
            NavigateTo("home");
        }

        private void InitializeState()
        {
            Config = new KonfigurasiAplikasi();
            Machine = new StatusPesananMachine();
            Keranjang = new KeranjangBelanja();
            Kalkulator = new KalkulatorDiskon();
            PenyimpananKategori = new PenyimpananLokal<string>();
            AuthMachine = new AutentikasiMachine();
            ApiClient = new HttpClient();

            PenyimpananKategori.Simpan("Kategori: Pakaian Fisik");
            PenyimpananKategori.Simpan("Kategori: Pola Digital");
        }

        private void BuildUI()
        {
            // Form settings
            Text = "REIAT E-Commerce";
            Size = new Size(1100, 720);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = UIHelper.ColorBackground;
            Font = UIHelper.FontBody;

            // === STATUS BAR (Bottom) ===
            pnlStatusBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = UIHelper.ColorSurface,
                Padding = new Padding(12, 0, 12, 0)
            };
            pnlStatusBar.Paint += (s, e) =>
            {
                using var pen = new Pen(UIHelper.ColorBorder);
                e.Graphics.DrawLine(pen, 0, 0, pnlStatusBar.Width, 0);
            };
            lblStatus = UIHelper.CreateLabel("Status Order: Keranjang  |  API: http://localhost:5241", UIHelper.FontSmall, UIHelper.ColorTextSecondary);
            lblStatus.Dock = DockStyle.Left;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            pnlStatusBar.Controls.Add(lblStatus);
            Controls.Add(pnlStatusBar);

            // === HEADER (Top) ===
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = UIHelper.ColorHeaderBg,
                Padding = new Padding(16, 0, 16, 0)
            };
            pnlHeader.Paint += (s, e) =>
            {
                using var pen = new Pen(UIHelper.ColorBorder);
                e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };

            lblLogo = new Label
            {
                Text = "🛍️  REIAT E-Commerce",
                Font = UIHelper.FontSubtitle,
                ForeColor = UIHelper.ColorPrimary,
                AutoSize = true,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var pnlHeaderRight = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 14, 0, 0)
            };

            lblRole = UIHelper.CreateLabel("Guest", UIHelper.FontBody, UIHelper.ColorTextSecondary);
            lblRole.Margin = new Padding(0, 4, 16, 0);
            lblAuth = UIHelper.CreateLabel("🔒 UNAUTHENTICATED", UIHelper.FontSmall, UIHelper.ColorError);
            lblAuth.Margin = new Padding(0, 5, 16, 0);
            lblCart = UIHelper.CreateLabel("🛒 0 Item", UIHelper.FontBody, UIHelper.ColorTextPrimary);
            lblCart.Margin = new Padding(0, 4, 0, 0);

            pnlHeaderRight.Controls.AddRange(new Control[] { lblRole, lblAuth, lblCart });
            pnlHeader.Controls.Add(pnlHeaderRight);
            pnlHeader.Controls.Add(lblLogo);
            Controls.Add(pnlHeader);

            // === SIDEBAR (Left) ===
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = UIHelper.ColorSidebar,
                Padding = new Padding(0, 10, 0, 10)
            };
            pnlSidebar.Paint += (s, e) =>
            {
                using var pen = new Pen(UIHelper.ColorBorder);
                e.Graphics.DrawLine(pen, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            btnHome = UIHelper.CreateNavButton("Dashboard");
            btnLogin = UIHelper.CreateNavButton("Login");
            btnKatalog = UIHelper.CreateNavButton("Katalog Produk");
            btnKeranjang = UIHelper.CreateNavButton("Keranjang");
            btnPromo = UIHelper.CreateNavButton("Klaim Promo");
            btnKategori = UIHelper.CreateNavButton("Kelola Kategori");
            btnCheckout = UIHelper.CreateNavButton("Checkout Pembayaran");
            btnLogout = UIHelper.CreateNavButton("Logout");
            btnLogout.ForeColor = UIHelper.ColorError;

            int y = 8;
            foreach (var btn in new[] { btnHome, btnLogin, btnKatalog, btnKeranjang, btnPromo, btnKategori, btnCheckout })
            {
                btn.Location = new Point(0, y);
                pnlSidebar.Controls.Add(btn);
                y += 46;
            }

            btnLogout.Location = new Point(0, 0);
            btnLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pnlSidebar.Controls.Add(btnLogout);

            // Nav events
            btnHome.Click += (s, e) => NavigateTo("home");
            btnLogin.Click += (s, e) => NavigateTo("login");
            btnKatalog.Click += (s, e) => NavigateTo("katalog");
            btnKeranjang.Click += (s, e) => NavigateTo("keranjang");
            btnPromo.Click += (s, e) => NavigateTo("promo");
            btnKategori.Click += (s, e) => NavigateTo("kategori");
            btnCheckout.Click += (s, e) => NavigateTo("checkout");
            btnLogout.Click += (s, e) => DoLogout();

            Controls.Add(pnlSidebar);

            // === CONTENT (Fill) ===
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.ColorBackground,
                Padding = new Padding(0)
            };
            Controls.Add(pnlContent);
            pnlContent.BringToFront();

            // Position logout at bottom after layout
            Layout += (s, e) =>
            {
                btnLogout.Location = new Point(0, pnlSidebar.ClientSize.Height - 54);
            };
        }

        // === NAVIGATION ===
        public void NavigateTo(string panel)
        {
            pnlContent.Controls.Clear();
            UserControl uc;

            switch (panel)
            {
                case "login":
                    uc = new LoginPanel(this);
                    UIHelper.SetActiveNav(btnLogin, AllNavButtons);
                    break;
                case "katalog":
                    uc = new KatalogPanel(this);
                    UIHelper.SetActiveNav(btnKatalog, AllNavButtons);
                    break;
                case "keranjang":
                    uc = new KeranjangPanel(this);
                    UIHelper.SetActiveNav(btnKeranjang, AllNavButtons);
                    break;
                case "promo":
                    uc = new PromoPanel(this);
                    UIHelper.SetActiveNav(btnPromo, AllNavButtons);
                    break;
                case "kategori":
                    uc = new KategoriPanel(this);
                    UIHelper.SetActiveNav(btnKategori, AllNavButtons);
                    break;
                case "checkout":
                    uc = new CheckoutPanel(this);
                    UIHelper.SetActiveNav(btnCheckout, AllNavButtons);
                    break;
                default:
                    uc = new DashboardPanel(this);
                    UIHelper.SetActiveNav(btnHome, AllNavButtons);
                    break;
            }

            uc.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(uc);
        }

        // === REFRESH UI ===
        public void RefreshHeader()
        {
            string roleDisplay = UserRole;
            if (!string.IsNullOrEmpty(UserEmail))
                roleDisplay += $" ({UserEmail})";
            lblRole.Text = $"👤 {roleDisplay}";

            bool isAuth = AuthMachine.StateSaatIni == AuthState.Authenticated;
            lblAuth.Text = isAuth ? "✅ AUTHENTICATED" : "🔒 UNAUTHENTICATED";
            lblAuth.ForeColor = isAuth ? UIHelper.ColorSuccess : UIHelper.ColorError;

            if (UserRole != "Admin")
                lblCart.Text = $"🛒 {Keranjang.LihatKeranjang().Count} Item";
            else
                lblCart.Text = "";

            lblStatus.Text = $"Status Order: {Machine.StateSaatIni}  |  API: {BaseUrlApi}";
            if (!string.IsNullOrEmpty(PromoDipakai))
                lblStatus.Text += $"  |  Promo: {PromoDipakai} (-Rp {DiskonDidapat:N0})";
        }

        public void RefreshSidebar()
        {
            bool isGuest = UserRole == "Guest";
            bool isCustomer = UserRole == "Customer";
            bool isAdmin = UserRole == "Admin";
            bool isLoggedIn = AuthMachine.StateSaatIni == AuthState.Authenticated;

            btnLogin.Visible = !isLoggedIn;
            btnKatalog.Visible = true;
            btnKeranjang.Visible = !isAdmin;
            btnPromo.Visible = isCustomer;
            btnKategori.Visible = isAdmin;
            btnCheckout.Visible = isCustomer;
            btnLogout.Visible = isLoggedIn;

            // Re-layout visible buttons
            int y = 8;
            foreach (var btn in new[] { btnHome, btnLogin, btnKatalog, btnKeranjang, btnPromo, btnKategori, btnCheckout })
            {
                if (btn.Visible)
                {
                    btn.Location = new Point(0, y);
                    y += 46;
                }
            }
        }

        // === ACTIONS ===
        public void OnLoginSuccess(string email, string role)
        {
            UserEmail = email;
            UserRole = role;
            RefreshSidebar();
            RefreshHeader();
            NavigateTo("home");
        }

        public void OnCartUpdated()
        {
            RefreshHeader();
        }

        public void OnPromoApplied(string kode, decimal diskon)
        {
            PromoDipakai = kode;
            DiskonDidapat = diskon;
            RefreshHeader();
        }

        public void OnPaymentComplete()
        {
            SudahBayar = true;
            RefreshHeader();
        }

        private void DoLogout()
        {
            var result = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Reset semua state
                AuthMachine = new AutentikasiMachine();
                Machine = new StatusPesananMachine();
                Keranjang = new KeranjangBelanja();
                UserEmail = "";
                UserRole = "Guest";
                PromoDipakai = "";
                DiskonDidapat = 0;
                SudahBayar = false;

                RefreshSidebar();
                RefreshHeader();
                NavigateTo("home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) ApiClient?.Dispose();
            base.Dispose(disposing);
        }
    }
}
