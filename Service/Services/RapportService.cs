using ClosedXML.Excel;
using Models.Models;

namespace MyGarage
{
    public class RapportService
    {
        public string GenererRapport(List<VehicleHistory> histories, string dossierDestination)
        {
            using var workbook = new XLWorkbook();

            foreach (var history in histories)
            {
                var v = history.Vehicle;
                if (v == null) continue;

                // Nom de l'onglet : immatriculation
                string sheetName = v.Immatriculation ?? $"Vehicule_{v.ID}";
                sheetName = sheetName.Replace("/", "-").Replace("\\", "-"); // Excel interdit certains caractères
                var ws = workbook.Worksheets.Add(sheetName);

                // ── En-tête véhicule ──────────────────────────────────────
                ws.Cell("A1").Value = "RAPPORT D'ENTRETIEN";
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Font.FontSize = 14;
                ws.Range("A1:E1").Merge();

                ws.Cell("A3").Value = "Marque"; ws.Cell("B3").Value = v.Marque;
                ws.Cell("A4").Value = "Modèle"; ws.Cell("B4").Value = v.Modele;
                ws.Cell("A5").Value = "Immatriculation"; ws.Cell("B5").Value = v.Immatriculation;
                ws.Cell("A6").Value = "Année"; ws.Cell("B6").Value = v.Annee;
                ws.Cell("A7").Value = "Kilométrage"; ws.Cell("B7").Value = $"{v.Kilometrage:N0} km";

                // Style en-tête véhicule
                foreach (var row in Enumerable.Range(3, 5))
                {
                    ws.Cell(row, 1).Style.Font.Bold = true;
                    ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                }

                // ── Tableau des entretiens ────────────────────────────────
                int startRow = 10;
                ws.Cell(startRow, 1).Value = "Date";
                ws.Cell(startRow, 2).Value = "Type";
                ws.Cell(startRow, 3).Value = "Kilométrage";
                ws.Cell(startRow, 4).Value = "Coût (€)";
                ws.Cell(startRow, 5).Value = "Notes";

                var headerRange = ws.Range(startRow, 1, startRow, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                int currentRow = startRow + 1;
                float coutTotal = 0;

                foreach (var ent in history.Historique ?? new List<Entretien>())
                {
                    ws.Cell(currentRow, 1).Value = DateTime.TryParse(ent.date_etretien, out var d)
                        ? d.ToString("dd/MM/yyyy") : ent.date_etretien;
                    ws.Cell(currentRow, 2).Value = ent.type_entretien;
                    ws.Cell(currentRow, 3).Value = ent.kilometrage;
                    ws.Cell(currentRow, 4).Value = ent.cout;
                    ws.Cell(currentRow, 5).Value = ent.notes;

                    // Alternance de couleurs
                    if (currentRow % 2 == 0)
                        ws.Range(currentRow, 1, currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#DDEEFF");

                    coutTotal += ent.cout ?? 0;
                    currentRow++;
                }

                // Bordure tableau
                ws.Range(startRow, 1, currentRow - 1, 5)
                    .Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                ws.Range(startRow, 1, currentRow - 1, 5)
                    .Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // ── Statistiques ──────────────────────────────────────────
                int statsRow = currentRow + 2;
                ws.Cell(statsRow, 1).Value = "STATISTIQUES";
                ws.Cell(statsRow, 1).Style.Font.Bold = true;
                ws.Cell(statsRow, 1).Style.Font.FontSize = 12;

                int nbEntretiens = history.Historique?.Count ?? 0;
                float coutMoyen = nbEntretiens > 0 ? coutTotal / nbEntretiens : 0;

                ws.Cell(statsRow + 1, 1).Value = "Nombre d'entretiens";
                ws.Cell(statsRow + 1, 2).Value = nbEntretiens;

                ws.Cell(statsRow + 2, 1).Value = "Coût total";
                ws.Cell(statsRow + 2, 2).Value = $"{coutTotal:N2} €";

                ws.Cell(statsRow + 3, 1).Value = "Coût moyen";
                ws.Cell(statsRow + 3, 2).Value = $"{coutMoyen:N2} €";

                var dernierEntretien = history.Historique?.FirstOrDefault();
                ws.Cell(statsRow + 4, 1).Value = "Dernier entretien";
                ws.Cell(statsRow + 4, 2).Value = dernierEntretien?.date_etretien ?? "N/A";

                foreach (var row in Enumerable.Range(statsRow + 1, 4))
                    ws.Cell(row, 1).Style.Font.Bold = true;

                ws.Columns().AdjustToContents();
            }

            // Sauvegarder le fichier
            string fileName = $"Rapport_MyGarage_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string fullPath = Path.Combine(dossierDestination, fileName);
            workbook.SaveAs(fullPath);

            return fullPath;
        }
    }
}