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
    public partial class VehicleHistoryForm : Form
    {
        private readonly VehicleHistory _history;
        private DataGridView dgvHistory = new DataGridView();
        private Label lblVehicle = new Label();
        private Label lblTotal = new Label();
        private Button btnClose = new Button();

        public VehicleHistoryForm(VehicleHistory history)
        {
            InitializeComponent();
            _history = history;

            string vehicleLabel = $"{history.Vehicle?.Marque} {history.Vehicle?.Modele} " +
                                  $"({history.Vehicle?.Immatriculation}) — " +
                                  $"{history.Vehicle?.Annee} — {history.Vehicle?.Kilometrage} km";

            this.Text = "Historique d'entretien";
            this.Size = new Size(850, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Label véhicule
            lblVehicle.Text = vehicleLabel;
            lblVehicle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblVehicle.AutoSize = true;
            lblVehicle.Location = new Point(12, 12);

            // DataGridView
            dgvHistory.Location = new Point(12, 50);
            dgvHistory.Size = new Size(810, 340);
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.ReadOnly = true;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.BackgroundColor = SystemColors.Window;

            // Label total coût
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblTotal.Location = new Point(12, 400);

            // Bouton fermer
            btnClose.Text = "Fermer";
            btnClose.Size = new Size(100, 35);
            btnClose.Location = new Point(722, 395);
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblVehicle);
            this.Controls.Add(dgvHistory);
            this.Controls.Add(lblTotal);
            this.Controls.Add(btnClose);

            LoadHistory();
        }

        private void LoadHistory()
        {
            if (_history.Historique == null || !_history.Historique.Any())
            {
                lblTotal.Text = "Aucun entretien enregistré pour ce véhicule.";
                return;
            }

            // Construire une liste affichable avec des noms de colonnes lisibles
            var rows = _history.Historique.Select(e => new
            {
                Date = DateTime.TryParse(e.date_etretien, out var d) ? d.ToString("dd/MM/yyyy") : e.date_etretien,
                Type = e.type_entretien,
                Kilométrage = e.kilometrage?.ToString("N0") + " km",
                Coût = e.cout?.ToString("N2") + " €",
                Notes = e.notes
            }).ToList();

            dgvHistory.DataSource = rows;

            // Calcul coût total
            float total = _history.Historique.Sum(e => e.cout ?? 0);
            int nbEntretiens = _history.Historique.Count;
            lblTotal.Text = $"{nbEntretiens} entretien(s) — Coût total : {total:N2} €";
        }
    }
}
