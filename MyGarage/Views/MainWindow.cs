using System.Reflection;

namespace MyGarage
{
    public partial class MyGarage : Form
    {
        public MyGarage()
        {
            InitializeComponent();
            this.Load += MainWindow_Load;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            label3.Text = $"Version : {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
