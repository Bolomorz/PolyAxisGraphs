using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyAxisGraphs_Backend;

namespace PolyAxisGraphs
{
    internal class CanvasChart
    {
        public PolyAxisGraph pag { get; set; }
        public System.Windows.Controls.Canvas canvas { get; set; }

        public CanvasChart(System.Windows.Controls.Canvas _canvas)
        {
            canvas = _canvas;
            pag = new PolyAxisGraph(new Settings(@"..\..\..\Settings.txt"));
        }

        public void SetFilePath(string path)
        {
            pag.SetFilePath(path);
            pag.ReadData();
        }

        public void SetLanguageFile(string file)
        {
            pag.SetLanguage(file);
        }
    }
}
