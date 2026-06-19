namespace Reiat.GUI.Helpers
{
    public static class UIHelper
    {
        // === COLORS (Light Mode) ===
        public static readonly Color ColorPrimary = Color.FromArgb(37, 99, 235);
        public static readonly Color ColorPrimaryDark = Color.FromArgb(29, 78, 216);
        public static readonly Color ColorPrimaryLight = Color.FromArgb(219, 234, 254);
        public static readonly Color ColorAccent = Color.FromArgb(245, 158, 11);
        public static readonly Color ColorBackground = Color.FromArgb(243, 244, 246);
        public static readonly Color ColorSurface = Color.White;
        public static readonly Color ColorSidebar = Color.FromArgb(249, 250, 251);
        public static readonly Color ColorTextPrimary = Color.FromArgb(31, 41, 55);
        public static readonly Color ColorTextSecondary = Color.FromArgb(107, 114, 128);
        public static readonly Color ColorBorder = Color.FromArgb(229, 231, 235);
        public static readonly Color ColorSuccess = Color.FromArgb(16, 185, 129);
        public static readonly Color ColorError = Color.FromArgb(239, 68, 68);
        public static readonly Color ColorHover = Color.FromArgb(239, 246, 255);
        public static readonly Color ColorHeaderBg = Color.White;

        // === FONTS ===
        public static readonly Font FontTitle = new("Segoe UI", 18, FontStyle.Bold);
        public static readonly Font FontSubtitle = new("Segoe UI", 13, FontStyle.Bold);
        public static readonly Font FontHeading = new("Segoe UI", 11, FontStyle.Bold);
        public static readonly Font FontBody = new("Segoe UI", 10);
        public static readonly Font FontBodyBold = new("Segoe UI", 10, FontStyle.Bold);
        public static readonly Font FontSmall = new("Segoe UI", 9);
        public static readonly Font FontNav = new("Segoe UI", 10);

        // === FACTORY METHODS ===
        public static Button CreatePrimaryButton(string text, int w = 180, int h = 38)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(w, h),
                BackColor = ColorPrimary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = FontBodyBold,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ColorPrimaryDark;
            return btn;
        }

        public static Button CreateOutlinedButton(string text, int w = 180, int h = 38)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(w, h),
                BackColor = Color.White,
                ForeColor = ColorPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = FontBody,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = ColorPrimary;
            btn.FlatAppearance.MouseOverBackColor = ColorHover;
            return btn;
        }

        public static Button CreateNavButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(280, 44),
                BackColor = Color.Transparent,
                ForeColor = ColorTextSecondary,
                FlatStyle = FlatStyle.Flat,
                Font = FontNav,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                AutoEllipsis = false
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ColorHover;
            return btn;
        }

        public static Label CreateLabel(string text, Font font, Color color)
        {
            return new Label { Text = text, Font = font, ForeColor = color, AutoSize = true };
        }

        public static TextBox CreateTextBox(int w = 300)
        {
            return new TextBox
            {
                Size = new Size(w, 32),
                Font = FontBody,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        public static Panel CreateCard()
        {
            var p = new Panel { BackColor = Color.White, Padding = new Padding(20) };
            p.Paint += (s, e) =>
            {
                using var pen = new Pen(ColorBorder);
                e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
            return p;
        }

        public static void SetActiveNav(Button active, params Button[] allButtons)
        {
            foreach (var b in allButtons)
            {
                b.BackColor = Color.Transparent;
                b.ForeColor = ColorTextSecondary;
                b.Font = FontNav;
            }
            active.BackColor = ColorPrimaryLight;
            active.ForeColor = ColorPrimary;
            active.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        }
    }
}
