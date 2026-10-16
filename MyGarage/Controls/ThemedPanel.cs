using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Panneau dont la couleur suit le thème, avec bordures optionnelles sur certains côtés.</summary>
    public class ThemedPanel : Panel
    {
        private SurfaceRole _role = SurfaceRole.Background;
        private AnchorStyles _edges = AnchorStyles.None;

        public SurfaceRole Role
        {
            get => _role;
            set { _role = value; ApplyTheme(); }
        }

        /// <summary>Côtés sur lesquels tracer une fine ligne de séparation.</summary>
        public AnchorStyles Edges
        {
            get => _edges;
            set { _edges = value; Invalidate(); }
        }

        public ThemedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = AppTheme.SurfaceColor(_role);
            Invalidate();
        }

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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_edges == AnchorStyles.None) return;
            using var pen = new Pen(AppTheme.Border);
            int w = Width - 1, h = Height - 1;
            if (_edges.HasFlag(AnchorStyles.Top)) e.Graphics.DrawLine(pen, 0, 0, w, 0);
            if (_edges.HasFlag(AnchorStyles.Bottom)) e.Graphics.DrawLine(pen, 0, h, w, h);
            if (_edges.HasFlag(AnchorStyles.Left)) e.Graphics.DrawLine(pen, 0, 0, 0, h);
            if (_edges.HasFlag(AnchorStyles.Right)) e.Graphics.DrawLine(pen, w, 0, w, h);
        }
    }
}
