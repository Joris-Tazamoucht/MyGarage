using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Label transparent dont la couleur suit le thème.</summary>
    public class ThemedLabel : Label
    {
        private TextRole _role = TextRole.Primary;

        public TextRole Role
        {
            get => _role;
            set { _role = value; ApplyTheme(); }
        }

        public ThemedLabel()
        {
            AutoSize = true;
            BackColor = Color.Transparent;
            Font = AppTheme.FontNormal;
            ApplyTheme();
        }

        private void ApplyTheme() => ForeColor = AppTheme.TextColor(_role);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AppTheme.ThemeChanged += ApplyTheme;
            ApplyTheme();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            AppTheme.ThemeChanged -= ApplyTheme;
            base.OnHandleDestroyed(e);
        }
    }
}
