using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Libellé au-dessus d'un contrôle de saisie, qui s'étire en largeur.</summary>
    public class FormField : Panel
    {
        public FormField(string caption, Control editor)
        {
            int top = AppTheme.S(22);
            var label = new ThemedLabel
            {
                Text = caption,
                Role = TextRole.Secondary,
                Font = AppTheme.FontSmallBold,
                Location = new Point(0, 0)
            };

            Width = AppTheme.S(200);
            editor.Location = new Point(0, top);
            editor.Width = Width;
            editor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Height = top + editor.Height;
            editor.SizeChanged += (s, e) => Height = top + editor.Height;

            Controls.Add(editor);
            Controls.Add(label);
        }
    }
}
