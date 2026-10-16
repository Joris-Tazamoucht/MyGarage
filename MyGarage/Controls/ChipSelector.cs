using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Rangée de pastilles cliquables qui passent à la ligne si besoin.</summary>
    public class ChipSelector : Panel
    {
        private readonly List<ThemedButton> _chips = new();

        public event EventHandler<string>? ChipClicked;

        public ChipSelector(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                var chip = new ThemedButton
                {
                    Variant = ButtonVariant.Chip,
                    Text = item,
                    Font = AppTheme.FontNormal,
                    Height = AppTheme.S(32),
                    TabStop = false
                };
                chip.FitWidth();
                chip.Click += (s, e) => ChipClicked?.Invoke(this, item);
                _chips.Add(chip);
                Controls.Add(chip);
            }
            Height = AppTheme.S(32);
        }

        /// <summary>Met en surbrillance la pastille dont le texte correspond (insensible à la casse).</summary>
        public void SelectValue(string? text)
        {
            foreach (var chip in _chips)
                chip.Active = !string.IsNullOrWhiteSpace(text) &&
                              string.Equals(chip.Text, text.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }

        private int Arrange(int width, bool apply)
        {
            int gap = AppTheme.S(8), x = 0, y = 0, rowHeight = 0;
            foreach (var chip in _chips)
            {
                if (x > 0 && x + chip.Width > width)
                {
                    x = 0;
                    y += rowHeight + gap;
                    rowHeight = 0;
                }
                if (apply) chip.Location = new Point(x, y);
                x += chip.Width + gap;
                rowHeight = Math.Max(rowHeight, chip.Height);
            }
            return y + rowHeight;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            int h = Arrange(Width, true);
            if (h > 0 && h != Height) Height = h;
        }

        public override Size GetPreferredSize(Size proposedSize)
            => new(proposedSize.Width, Arrange(proposedSize.Width > 0 ? proposedSize.Width : Width, false));
    }
}
