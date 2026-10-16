using System.Globalization;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>
    /// Champ date (jj/mm/aaaa) avec bouton calendrier.
    /// Remplace le DateTimePicker, qui reste blanc en thème sombre.
    /// </summary>
    public class DateField : InputBox
    {
        private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr-FR");
        private static readonly string[] Formats =
            { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "d/M/yy", "ddMMyyyy", "yyyy-MM-dd" };

        public DateField()
        {
            TrailingGlyph = Icons.Calendar;
            PlaceholderText = "jj/mm/aaaa";
            Inner.MaxLength = 10;
            TrailingClick += (s, e) => ShowCalendar();
            Inner.Leave += (s, e) =>
            {
                if (Value is DateTime d) Text = d.ToString("dd/MM/yyyy", Fr);
            };
        }

        /// <summary>Date saisie, ou null si le texte n'est pas une date valide.</summary>
        public DateTime? Value
        {
            get => DateTime.TryParseExact(Text.Trim(), Formats, Fr, DateTimeStyles.None, out var d) ? d : null;
            set => Text = value?.ToString("dd/MM/yyyy", Fr) ?? "";
        }

        private void ShowCalendar()
        {
            var calendar = new MonthCalendar { MaxSelectionCount = 1 };
            calendar.SetDate(Value ?? DateTime.Today);

            if (AppTheme.IsDark)
            {
                // Le calendrier natif ignore les couleurs tant que les styles visuels sont actifs
                _ = calendar.Handle;
                NativeTheme.DisableVisualStyles(calendar);
                calendar.BackColor = AppTheme.Surface;
                calendar.ForeColor = AppTheme.TextPrimary;
                calendar.TitleBackColor = AppTheme.AccentBlue;
                calendar.TitleForeColor = Color.White;
                calendar.TrailingForeColor = AppTheme.TextMuted;
                calendar.Size = calendar.SingleMonthSize;
            }

            var host = new ToolStripControlHost(calendar)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = calendar.Size
            };
            var dropDown = new ToolStripDropDown { Padding = Padding.Empty };
            dropDown.Items.Add(host);

            calendar.DateSelected += (s, e) =>
            {
                Value = e.Start;
                dropDown.Close();
                Inner.Focus();
            };
            dropDown.Closed += (s, e) => BeginInvoke(new Action(dropDown.Dispose));
            dropDown.Show(this, new Point(Width - host.Width, Height + AppTheme.S(4)));
        }
    }
}
