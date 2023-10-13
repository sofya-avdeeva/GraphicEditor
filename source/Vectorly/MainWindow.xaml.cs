using Core.Model;
using Core.Interfaces;
using System.Windows;
using System.Windows.Forms;

namespace Vectorly
{
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this);

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        public string SaveFileDialog(string name, string ext)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.DefaultExt = ext;
            openFileDialog.Filter = $"{name}|*.{ext}";
            DialogResult result = openFileDialog.ShowDialog();

            string path = "";
            if (result == System.Windows.Forms.DialogResult.OK)
                path = openFileDialog.FileName;

            return path;
        }

        public string OpenFileDialog(string name, string ext)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ext;
            openFileDialog.Filter = $"{name}|*.{ext}";
            DialogResult result = openFileDialog.ShowDialog();

            string path = "";
            if (result == System.Windows.Forms.DialogResult.OK)
                path = openFileDialog.FileName;

            return path;
        }
    }
}
