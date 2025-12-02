using System.Reflection;

namespace MyGarage
{
    public partial class MyGarage : Form
    {
        private readonly VehicleService _vehicleService;
        public MyGarage()
        {
            InitializeComponent();
            this.Load += MainWindow_Load;
            
            // On charge les vehicules au demarrage de l'app
            _vehicleService = new VehicleService("http://localhost:5119");
            dataGridView2_CellContentClick(null, null);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            label3.Text = $"Version : {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
           
        }

        private async void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var vehicles = await _vehicleService.GetVehiclesAsync("AB-123-CD");
                dataGridView2.DataSource = vehicles;

                // cacher la colonne ID
                if (dataGridView2.Columns["ID"] != null)
                {
                    dataGridView2.Columns.Remove("ID");
                    //dataGridView2.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }
    }
}
