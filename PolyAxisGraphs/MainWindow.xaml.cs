using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace PolyAxisGraphs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CanvasChart cv { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            cv = new CanvasChart(canvas);
            LoadControls();
        }

        private void LoadControls()
        {
            FontFamily ff = new FontFamily(Settings1.Default.usedfont);
            rtbfile.FontFamily = ff;
            rtbfile.FontSize = Settings1.Default.usedfontsize;
            rtbfile.AppendText(cv.pag.settings.defaultlang.FindElement("tbopenfile"));
            btopenfile.FontFamily = ff;
            btopenfile.FontSize = Settings1.Default.usedfontsize;
            btopenfile.Content = cv.pag.settings.defaultlang.FindElement("btopenfile");
        }

        private void OpenFileBtClick(object sender, RoutedEventArgs e)
        {
            string path = "";
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            if (cv.pag.settings.initialdirectory is not null)
            {
                string id = System.IO.Path.GetFullPath(System.IO.Path.Combine(dir, cv.pag.settings.initialdirectory));
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Title = "Open Data File",
                    Multiselect = false,
                    InitialDirectory = id,
                    Filter = "TXT|*.txt|CSV|*.csv|XLSX|*.xlsx",
                    FilterIndex = 0,
                    RestoreDirectory = true
                };

                if(ofd.ShowDialog() == true)
                {
                    path = ofd.FileName;
                    cv.SetFilePath(path);
                    rtbfile.Document.Blocks.Clear();
                    rtbfile.AppendText(path);
                }
            }
        }
    }
}
