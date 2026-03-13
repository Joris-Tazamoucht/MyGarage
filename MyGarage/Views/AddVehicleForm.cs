using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MyGarage;

using Models.Models;

namespace MyGarage.Views
{
    public partial class AddVehicleForm : Form
    {
        private TextBox txtMarque = new TextBox();
        private TextBox txtModele = new TextBox();
        private TextBox txtImmatriculation = new TextBox();
        private TextBox txtKilometrage = new TextBox();
        private TextBox txtAnnee = new TextBox();
        private Button btnConfirm = new Button();
        private Button btnCancel = new Button();

        public Vehicle? Vehicle { get; private set; }

        public AddVehicleForm()
        {
            this.Text = "Ajouter un véhicule";
            this.Size = new Size(350, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10)
            };

            // Champs
            layout.Controls.Add(new Label { Text = "Marque :", Anchor = AnchorStyles.Left }, 0, 0);
            layout.Controls.Add(txtMarque, 1, 0);

            layout.Controls.Add(new Label { Text = "Modèle :", Anchor = AnchorStyles.Left }, 0, 1);
            layout.Controls.Add(txtModele, 1, 1);

            layout.Controls.Add(new Label { Text = "Immatriculation :", Anchor = AnchorStyles.Left }, 0, 2);
            layout.Controls.Add(txtImmatriculation, 1, 2);

            layout.Controls.Add(new Label { Text = "Kilométrage :", Anchor = AnchorStyles.Left }, 0, 3);
            layout.Controls.Add(txtKilometrage, 1, 3);

            layout.Controls.Add(new Label { Text = "Année :", Anchor = AnchorStyles.Left }, 0, 4);
            layout.Controls.Add(txtAnnee, 1, 4);

            // Boutons
            btnConfirm.Text = "Ajouter";
            btnConfirm.DialogResult = DialogResult.OK;
            btnConfirm.Click += BtnConfirm_Click;

            btnCancel.Text = "Annuler";
            btnCancel.DialogResult = DialogResult.Cancel;

            var btnPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft };
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnConfirm);

            layout.Controls.Add(btnPanel, 1, 5);

            this.Controls.Add(layout);
            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            // Validation basique
            if (string.IsNullOrWhiteSpace(txtMarque.Text) ||
                string.IsNullOrWhiteSpace(txtModele.Text) ||
                string.IsNullOrWhiteSpace(txtImmatriculation.Text))
            {
                MessageBox.Show("Marque, modèle et immatriculation sont obligatoires.",
                    "Champs manquants", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None; // Empêche la fermeture
                return;
            }

            if (!int.TryParse(txtKilometrage.Text, out int km))
            {
                MessageBox.Show("Le kilométrage doit être un nombre entier.",
                    "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!int.TryParse(txtAnnee.Text, out int annee))
            {
                MessageBox.Show("L'année doit être un nombre entier.",
                    "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            Vehicle = new Vehicle
            {
                Marque = txtMarque.Text.Trim(),
                Modele = txtModele.Text.Trim(),
                Immatriculation = txtImmatriculation.Text.Trim().ToUpper(),
                Kilometrage = km,
                Annee = annee
            };
        }
    }
}
