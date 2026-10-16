using System.Runtime.InteropServices;

namespace MyGarage.Styles
{
    /// <summary>
    /// Petits appels Windows pour ce que WinForms ne thème pas tout seul :
    /// barre de titre sombre, barres de défilement sombres.
    /// Tout est protégé : si Windows ne supporte pas, rien ne casse.
    /// </summary>
    internal static class NativeTheme
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string? subAppName, string? subIdList);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int cx, int cy, uint flags);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_OLD = 19;   // Windows 10 avant 20H1

        public static void ApplyTitleBar(Form form)
        {
            if (!form.IsHandleCreated) return;
            try
            {
                int dark = AppTheme.IsDark ? 1 : 0;
                if (DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int)) != 0)
                    DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_OLD, ref dark, sizeof(int));

                // Force Windows à redessiner la barre de titre tout de suite
                const uint flags = 0x0001 | 0x0002 | 0x0004 | 0x0010 | 0x0020; // NOSIZE|NOMOVE|NOZORDER|NOACTIVATE|FRAMECHANGED
                SetWindowPos(form.Handle, IntPtr.Zero, 0, 0, 0, 0, flags);
            }
            catch { }
        }

        public static void ApplyScrollBars(Control control)
        {
            if (!control.IsHandleCreated) return;
            try { SetWindowTheme(control.Handle, AppTheme.IsDark ? "DarkMode_Explorer" : "Explorer", null); }
            catch { }
        }

        public static void DisableVisualStyles(Control control)
        {
            try { SetWindowTheme(control.Handle, "", ""); }
            catch { }
        }
    }
}
