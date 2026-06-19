using Reiat.Lib;
using Reiat.GUI.Forms;
using Reiat.GUI.Helpers;

namespace Reiat.GUI.Panels
{
    public class LoginPanel : UserControl
    {
        private readonly MainForm _main;
        private TextBox txtEmail = null!;
        private TextBox txtPassword = null!;
        private Label lblMessage = null!;

        public LoginPanel(MainForm main)
        {
            _main = main;
            BackColor = UIHelper.ColorBackground;
            Dock = DockStyle.Fill;
            AutoScroll = true;
            BuildUI();
        }

        private void BuildUI()
        {
            // Centering card
            var card = UIHelper.CreateCard();
            card.MinimumSize = new Size(520, 420);
            card.AutoSize = true;
            card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            card.Anchor = AnchorStyles.None;
            Controls.Add(card);

            // Re-center on resize
            Layout += (s, e) =>
            {
                card.Location = new Point(
                    (ClientSize.Width - card.Width) / 2,
                    (ClientSize.Height - card.Height) / 2 - 20
                );
            };

            int y = 24;
            int innerWidth = 460;

            var lblTitle = UIHelper.CreateLabel("🔐  Login Akun", UIHelper.FontSubtitle, UIHelper.ColorTextPrimary);
            lblTitle.Location = new Point(24, y);
            card.Controls.Add(lblTitle);
            y += 40;

            var lblDesc = UIHelper.CreateLabel("Masuk untuk mengakses fitur lengkap", UIHelper.FontSmall, UIHelper.ColorTextSecondary);
            lblDesc.Location = new Point(24, y);
            card.Controls.Add(lblDesc);
            y += 36;

            // Email
            var lblEmail = UIHelper.CreateLabel("Email", UIHelper.FontBody, UIHelper.ColorTextPrimary);
            lblEmail.Location = new Point(24, y);
            card.Controls.Add(lblEmail);
            y += 24;

            txtEmail = UIHelper.CreateTextBox(innerWidth);
            txtEmail.Location = new Point(24, y);
            card.Controls.Add(txtEmail);
            y += 42;

            // Password
            var lblPwd = UIHelper.CreateLabel("Password", UIHelper.FontBody, UIHelper.ColorTextPrimary);
            lblPwd.Location = new Point(24, y);
            card.Controls.Add(lblPwd);
            y += 24;

            txtPassword = UIHelper.CreateTextBox(innerWidth);
            txtPassword.Location = new Point(24, y);
            txtPassword.UseSystemPasswordChar = true;
            card.Controls.Add(txtPassword);
            y += 48;

            // Login button
            var btnLogin = UIHelper.CreatePrimaryButton("Login", innerWidth, 44);
            btnLogin.Location = new Point(24, y);
            btnLogin.Click += BtnLogin_Click;
            card.Controls.Add(btnLogin);
            y += 52;

            // Hint
            var lblHint = UIHelper.CreateLabel(
                "💡 Admin: admin@reiat.com / admin123\n     Selain itu = Customer",
                UIHelper.FontSmall, UIHelper.ColorTextSecondary);
            lblHint.Location = new Point(24, y);
            card.Controls.Add(lblHint);
            y += 40;

            // Message
            lblMessage = new Label
            {
                Location = new Point(24, y),
                Size = new Size(innerWidth, 30),
                Font = UIHelper.FontSmall,
                ForeColor = UIHelper.ColorError,
                Text = "",
                AutoSize = true
            };
            card.Controls.Add(lblMessage);

            // Add bottom padding to card via a dummy invisible label or just relying on Panel padding
            var lblSpacer = new Label { Location = new Point(24, y + 20), Size = new Size(1, 20) };
            card.Controls.Add(lblSpacer);

            // Enter key triggers login
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(s, e); };
            txtEmail.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (_main.AuthMachine.StateSaatIni == AuthState.Authenticated)
            {
                lblMessage.ForeColor = UIHelper.ColorAccent;
                lblMessage.Text = "Anda sudah login!";
                return;
            }

            try
            {
                _main.AuthMachine.TriggerLogin();

                string inputEmail = txtEmail.Text.Trim();
                ValidatorInput.ValidasiEmail(inputEmail);

                string inputPassword = txtPassword.Text;
                ValidatorInput.ValidasiPassword(inputPassword);

                string role;
                if (inputEmail == "admin@reiat.com")
                {
                    if (inputPassword == "admin123")
                        role = "Admin";
                    else
                        throw new Exception("Password Admin salah! Akses ditolak.");
                }
                else
                {
                    role = "Customer";
                }

                _main.AuthMachine.SuksesLogin();

                MessageBox.Show($"Login berhasil! Anda masuk sebagai: {role}",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _main.OnLoginSuccess(inputEmail, role);
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = UIHelper.ColorError;
                lblMessage.Text = ex.Message;

                // Reset auth state
                _main.AuthMachine = new AutentikasiMachine();
            }
        }
    }
}
