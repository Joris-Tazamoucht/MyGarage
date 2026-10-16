using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace MyGarage.Styles
{
    /// <summary>Rôle d'une surface : détermine sa couleur de fond selon le thème.</summary>
    public enum SurfaceRole { Background, Surface, SurfaceAlt, SideBar }

    /// <summary>Rôle d'un texte : détermine sa couleur selon le thème.</summary>
    public enum TextRole { Primary, Secondary, Muted, Accent, Danger }

    public static class AppTheme
    {
        // ── État ──────────────────────────────────────────────────────────
        public static bool IsDark { get; private set; } = true;
        public static event Action? ThemeChanged;

        private static readonly string PreferencePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyGarage", "theme.txt");

        // ── Palettes ──────────────────────────────────────────────────────
        private sealed record Palette(
            Color Background, Color Surface, Color SurfaceAlt, Color SideBar,
            Color Text, Color TextSub, Color TextMuted,
            Color Border, Color BorderStrong, Color InputBack, Color Hover,
            Color GridSel, Color GridSelText, Color AccentSoft, Color AccentText);

        private static readonly Palette Dark = new(
            Background: Color.FromArgb(15, 17, 23),
            Surface: Color.FromArgb(23, 26, 35),
            SurfaceAlt: Color.FromArgb(30, 34, 45),
            SideBar: Color.FromArgb(19, 21, 29),
            Text: Color.FromArgb(234, 236, 243),
            TextSub: Color.FromArgb(152, 158, 178),
            TextMuted: Color.FromArgb(104, 110, 130),
            Border: Color.FromArgb(40, 45, 58),
            BorderStrong: Color.FromArgb(64, 70, 88),
            InputBack: Color.FromArgb(17, 19, 26),
            Hover: Color.FromArgb(35, 39, 52),
            GridSel: Color.FromArgb(33, 52, 92),
            GridSelText: Color.FromArgb(240, 244, 255),
            AccentSoft: Color.FromArgb(30, 46, 82),
            AccentText: Color.FromArgb(125, 170, 255));

        private static readonly Palette Light = new(
            Background: Color.FromArgb(243, 245, 250),
            Surface: Color.White,
            SurfaceAlt: Color.FromArgb(247, 248, 252),
            SideBar: Color.White,
            Text: Color.FromArgb(21, 25, 39),
            TextSub: Color.FromArgb(92, 99, 122),
            TextMuted: Color.FromArgb(142, 148, 168),
            Border: Color.FromArgb(223, 227, 237),
            BorderStrong: Color.FromArgb(196, 202, 218),
            InputBack: Color.White,
            Hover: Color.FromArgb(239, 242, 249),
            GridSel: Color.FromArgb(225, 235, 255),
            GridSelText: Color.FromArgb(21, 25, 39),
            AccentSoft: Color.FromArgb(232, 240, 255),
            AccentText: Color.FromArgb(37, 92, 200));

        private static Palette P => IsDark ? Dark : Light;

        // ── Couleurs communes ─────────────────────────────────────────────
        public static readonly Color AccentBlue = Color.FromArgb(64, 124, 236);
        public static readonly Color AccentBlueDark = Color.FromArgb(50, 104, 212);   // survol
        public static readonly Color AccentPressed = Color.FromArgb(40, 88, 190);
        public static readonly Color AccentBlueLight = Color.FromArgb(125, 170, 255);
        public static readonly Color DangerRed = Color.FromArgb(224, 72, 72);
        public static readonly Color DangerHover = Color.FromArgb(200, 56, 56);
        public static readonly Color SuccessGreen = Color.FromArgb(34, 168, 98);
        public static readonly Color SuccessHover = Color.FromArgb(26, 144, 82);
        public static readonly Color WarningOrange = Color.FromArgb(232, 142, 44);
        public static readonly Color Purple = Color.FromArgb(139, 92, 246);

        // ── Accesseurs dynamiques ─────────────────────────────────────────
        public static Color Background => P.Background;
        public static Color Surface => P.Surface;
        public static Color SurfaceAlt => P.SurfaceAlt;
        public static Color SideBar => P.SideBar;
        public static Color TextPrimary => P.Text;
        public static Color TextSub => P.TextSub;
        public static Color TextMuted => P.TextMuted;
        public static Color Border => P.Border;
        public static Color BorderStrong => P.BorderStrong;
        public static Color InputBack => P.InputBack;
        public static Color Hover => P.Hover;
        public static Color GridSel => P.GridSel;
        public static Color GridSelText => P.GridSelText;
        public static Color GridAlt => P.SurfaceAlt;
        public static Color AccentSoft => P.AccentSoft;
        public static Color AccentText => P.AccentText;

        public static Color SurfaceColor(SurfaceRole role) => role switch
        {
            SurfaceRole.Surface => Surface,
            SurfaceRole.SurfaceAlt => SurfaceAlt,
            SurfaceRole.SideBar => SideBar,
            _ => Background
        };

        public static Color TextColor(TextRole role) => role switch
        {
            TextRole.Secondary => TextSub,
            TextRole.Muted => TextMuted,
            TextRole.Accent => AccentText,
            TextRole.Danger => DangerRed,
            _ => TextPrimary
        };

        // ── Polices (créées une seule fois, jamais recréées) ──────────────
        public static readonly Font FontTitle = new("Segoe UI Semibold", 15F);
        public static readonly Font FontSubtitle = new("Segoe UI Semibold", 11F);
        public static readonly Font FontNormal = new("Segoe UI", 9.75F);
        public static readonly Font FontMedium = new("Segoe UI Semibold", 9.75F);
        public static readonly Font FontSmall = new("Segoe UI", 8.25F);
        public static readonly Font FontSmallBold = new("Segoe UI Semibold", 8.25F);
        public static readonly Font FontButton = new("Segoe UI Semibold", 9.75F);
        public static readonly Font FontStat = new("Segoe UI Semibold", 16F);

        // Icônes : Segoe Fluent Icons (Windows 11) ou Segoe MDL2 Assets (Windows 10)
        private static readonly string IconFamily = ResolveIconFamily();
        private static readonly Dictionary<float, Font> IconFonts = new();

        public static Font IconFont(float size)
        {
            lock (IconFonts)
            {
                if (!IconFonts.TryGetValue(size, out var font))
                {
                    font = new Font(IconFamily, size);
                    IconFonts[size] = font;
                }
                return font;
            }
        }

        private static string ResolveIconFamily()
        {
            try
            {
                using var fonts = new InstalledFontCollection();
                if (fonts.Families.Any(f => f.Name == "Segoe Fluent Icons"))
                    return "Segoe Fluent Icons";
            }
            catch { }
            return "Segoe MDL2 Assets";
        }

        // ── Mise à l'échelle DPI ──────────────────────────────────────────
        public static float ScaleFactor { get; } = ComputeScale();

        private static float ComputeScale()
        {
            try
            {
                using var g = Graphics.FromHwnd(IntPtr.Zero);
                return g.DpiX / 96f;
            }
            catch { return 1f; }
        }

        /// <summary>Convertit une taille pensée à 100 % en pixels réels.</summary>
        public static int S(int value) => (int)Math.Round(value * ScaleFactor);

        // ── Bascule et persistance ────────────────────────────────────────
        static AppTheme()
        {
            try
            {
                if (File.Exists(PreferencePath))
                    IsDark = File.ReadAllText(PreferencePath).Trim() != "light";
            }
            catch { }
        }

        public static void Toggle()
        {
            IsDark = !IsDark;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PreferencePath)!);
                File.WriteAllText(PreferencePath, IsDark ? "dark" : "light");
            }
            catch { }
            ThemeChanged?.Invoke();
        }

        // ── Outils de dessin ──────────────────────────────────────────────
        public static Color Blend(Color from, Color to, float amount)
        {
            amount = Math.Clamp(amount, 0f, 1f);
            return Color.FromArgb(
                (int)Math.Round(from.R + (to.R - from.R) * amount),
                (int)Math.Round(from.G + (to.G - from.G) * amount),
                (int)Math.Round(from.B + (to.B - from.B) * amount));
        }

        public static GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d <= 0)
            {
                path.AddRectangle(r);
                return path;
            }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>Première couleur de fond opaque en remontant les parents.</summary>
        public static Color ResolveBackColor(Control control)
        {
            for (var p = control.Parent; p != null; p = p.Parent)
                if (p.BackColor.A == 255) return p.BackColor;
            return Background;
        }
    }
}
