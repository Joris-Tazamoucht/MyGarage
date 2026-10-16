namespace MyGarage.Views
{
    partial class ExportSelectionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // AutoScaleDimensions retiré : il réduisait toute la fenêtre de 25 %
            // sur les écrans à 100 %. La mise à l'échelle est gérée par AppTheme.S().
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 560);
            this.Text = "Exporter un rapport";
        }

        #endregion
    }
}
