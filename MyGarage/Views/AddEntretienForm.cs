using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Models.Models;

namespace MyGarage.Views
{
    public partial class AddEntretienForm : Form
    {
        private readonly int _vehicleId;
        private readonly string _vehicleLabel;

        private DateTimePicker dtpDate = new DateTimePicker();
        private ComboBox cboType = new ComboBox();
        private TextBox txtKilometrage = new TextBox();
        private TextBox txtCout = new TextBox();
        private TextBox txtNotes = new TextBox();
        private Button btnConfirm = new Button();
        private Button btnCancel = new Button();

        public Entretien? Entretien { get; private set; }

        public AddEntretienForm(int vehicleId, string vehicleLabel)
        {
            InitializeComponent();
            _vehicleId = vehicleId;
            _vehicleLabel = vehicleLabel;

            this.Text = $"Ajouter un entretien — {vehicleLabel}";
            this.Size = new Size(420, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;

            // Types d'entretien courants
            cboType.Items.AddRange(new string[]
            {
                "Vidange", "Révision", "Freins", "Pneus", "Courroie",
                "Filtres", "Batterie", "Amortisseurs", "Autre"
            });
            cboType.DropDownStyle = ComboBoxStyle.DropDown;

            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Value = DateTime.Today;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(10)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.Controls.Add(new Label { Text = "Date :", Anchor = AnchorStyles.Left }, 0, 0);
            layout.Controls.Add(dtpDate, 1, 0);

            layout.Controls.Add(new Label { Text = "Type :", Anchor = AnchorStyles.Left }, 0, 1);
            layout.Controls.Add(cboType, 1, 1);

            layout.Controls.Add(new Label { Text = "Kilométrage :", Anchor = AnchorStyles.Left }, 0, 2);
            layout.Controls.Add(txtKilometrage, 1, 2);

            layout.Controls.Add(new Label { Text = "Coût (€) :", Anchor = AnchorStyles.Left }, 0, 3);
            layout.Controls.Add(txtCout, 1, 3);

            layout.Controls.Add(new Label { Text = "Notes :", Anchor = AnchorStyles.Left }, 0, 4);
            txtNotes.Multiline = true;
            txtNotes.Height = 60;
            layout.Controls.Add(txtNotes, 1, 4);

            btnConfirm.Text = "Ajouter";
            btnConfirm.DialogResult = DialogResult.OK;
            btnConfirm.Click += BtnConfirm_Click;

            btnCancel.Text = "Annuler";
            btnCancel.DialogResult = DialogResult.Cancel;

            var btnPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnConfirm);
            layout.Controls.Add(btnPanel, 1, 6);

            this.Controls.Add(layout);
            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboType.Text))
            {
                MessageBox.Show("Le type d'entretien est obligatoire.",
                    "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!int.TryParse(txtKilometrage.Text, out int km))
            {
                MessageBox.Show("Le kilométrage doit être un nombre entier.",
                    "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!float.TryParse(txtCout.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out float cout))
            {
                MessageBox.Show("Le coût doit être un nombre (ex: 149.90).",
                    "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            Entretien = new Entretien
            {
                vehicle_id = _vehicleId,
                date_etretien = dtpDate.Value.ToString("yyyy-MM-dd"),
                type_entretien = cboType.Text.Trim(),
                kilometrage = km,
                cout = cout,
                notes = txtNotes.Text.Trim()
            };
        }
    }
}
