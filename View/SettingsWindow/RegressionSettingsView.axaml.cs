using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using PolyAxisGraphs.Data;

namespace PolyAxisGraphs.Views;

public partial class RegressionSettingsView : UserControl
{
    Series Series { get; set; }
    CanvasGraph CanvasGraph { get; set; }

    const int heightfactor = 2;
    const int controlcount = 19;
    const int width = 600;

    public RegressionSettingsView(Series series, CanvasGraph canvasgraph)
    {
        Series = series;
        CanvasGraph = canvasgraph;
        if (CanvasGraph.GraphData is null) ErrorWindow.Show("error: pag is null -> settings file not found");
        else
        {
            this.Width = width;
            var cfs = Settings.ControlFontSize;
            this.Height = heightfactor * controlcount * cfs;
            InitializeComponent();
            MainGrid.Width = width;
            MainGrid.Height = this.Height;
            LoadControls();
        }
    }

    private void LoadControls()
    {
        if (CanvasGraph.GraphData is null) ErrorWindow.Show("error: pag is null -> settings file not found");
        else
        {
            var ff = new Avalonia.Media.FontFamily("Consolas");
            var fs = Settings.ControlFontSize;

            var controlheight = this.Height / controlcount;
            var controlwidth = this.Width / 4;

            tbltitle.FontFamily = ff;
            tbltitle.FontSize = (int)fs;
            tbltitle.Text = Language.TbTitleSettingsFunc + " " + Series.Name;
            tbltitle.Height = controlheight - 10;
            tbltitle.Width = controlwidth * 3 - 10;

            btreturn.FontFamily = ff;
            btreturn.FontSize = (int)fs;
            btreturn.Content = Language.BtReturn;

            btcreate.FontFamily = ff;
            btcreate.FontSize = (int)fs;
            btcreate.Content = Language.BtCalc;

            btapplyprecision.FontFamily = ff;
            btapplyprecision.FontSize = (int)fs;
            btapplyprecision.Content = Language.BtApply;

            tbltitlecurrent.FontFamily = ff;
            tbltitlecurrent.FontSize = (int)fs;
            tbltitlecurrent.Text = Language.TbTitleSettingsCurrent;
            tbltitlecurrent.Height = controlheight - 10;
            tbltitlecurrent.Width = controlwidth * 4 - 10;

            tbltitlesettings.FontFamily = ff;
            tbltitlesettings.FontSize = (int)fs;
            tbltitlesettings.Text = Language.TbTitleSettingsS;
            tbltitlesettings.Height = controlheight - 10;
            tbltitlesettings.Width = controlwidth * 4 - 10;

            tbltitlecreate.FontFamily = ff;
            tbltitlecreate.FontSize = (int)fs;
            tbltitlecreate.Text = Language.TbTitleSettingsCreate;
            tbltitlecreate.Height = controlheight - 10;
            tbltitlecreate.Width = controlwidth * 4 - 10;

            tblfunction.FontFamily = ff;
            tblfunction.FontSize = (int)fs;
            tblfunction.Text = Language.TbFunction;
            tblfunction.Height = controlheight * 4 - 10;
            tblfunction.Width = controlwidth - 10;

            tbltype.FontFamily = ff;
            tbltype.FontSize = (int)fs;
            tbltype.Text = Language.TbFType;
            tbltype.Height = controlheight * 2 - 10;
            tbltype.Width = controlwidth - 10;

            tblprecision.FontFamily = ff;
            tblprecision.FontSize = (int)fs;
            tblprecision.Text = Language.TbPrecision;
            tblprecision.Height = controlheight * 2 - 10;
            tblprecision.Width = controlwidth - 10;

            tblorder.FontFamily = ff;
            tblorder.FontSize = (int)fs;
            tblorder.Text = Language.TbOrder;
            tblorder.Height = controlheight * 2 - 10;
            tblorder.Width = controlwidth - 10;

            tblselecttype.FontFamily = ff;
            tblselecttype.FontSize = (int)fs;
            tblselecttype.Text = Language.TbSelect;
            tblselecttype.Height = controlheight * 2 - 10;
            tblselecttype.Width = controlwidth - 10;

            tbofunction.FontFamily = ff;
            tbofunction.FontSize = (int)fs;
            tbofunction.Inlines = new Avalonia.Controls.Documents.InlineCollection();
            tbofunction.Inlines.Clear();
            var strings = Series.GetFunction();
            foreach (var functionString in strings.FunctionStrings)
            {
                if (functionString.SuperScript)
                {
                    Run run = new()
                    {
                        FontSize = (int)fs * 2 / 3,
                        FontFamily = ff,
                        BaselineAlignment = BaselineAlignment.Top,
                        Text = functionString.Function
                    };
                    tbofunction.Inlines.Add(run);
                }
                else
                {
                    Run run = new()
                    {
                        FontSize = (int)fs,
                        FontFamily = ff,
                        BaselineAlignment = BaselineAlignment.Center,
                        Text = functionString.Function
                    };
                    tbofunction.Inlines.Add(run);
                }
            }
            tbofunction.Width = controlwidth * 3 - 10;
            tbofunction.Height = controlheight * 4 - 10;

            tboorder.FontFamily = ff;
            tboorder.FontSize = (int)fs;
            tboorder.Text = "2";
            tboorder.Height = controlheight * 2 - 10;
            tboorder.Width = controlwidth * 3 - 10;

            tboprecision.FontFamily = ff;
            tboprecision.FontSize = (int)fs;
            tboprecision.Text = Series.Precision.ToString();
            tboprecision.Height = controlheight * 2 - 10;
            tboprecision.Width = controlwidth * 2 - 10;

            tbotype.FontFamily = ff;
            tbotype.FontSize = (int)fs;
            tbotype.Text = Series.RegressionFunction.Type.ToString();
            tbotype.Height = controlheight * 2 - 10;
            tbotype.Width = controlwidth * 2 - 10;

            cbactive.FontFamily = ff;
            cbactive.FontSize = (int)fs;
            cbactive.Content = Language.CbFunc;
            cbactive.IsChecked = Series.RegressionFunction.ShowFunction;

            lbselecttype.FontFamily = ff;
            lbselecttype.FontSize = (int)fs;
            lbselecttype.Items.Clear();
            lbselecttype.Items.Add(FunctionType.NaF);
            lbselecttype.Items.Add(FunctionType.Line);
            lbselecttype.Items.Add(FunctionType.Logarithm);
            lbselecttype.Items.Add(FunctionType.Polynomial);
            lbselecttype.Items.Add(FunctionType.Power);
            lbselecttype.Items.Add(FunctionType.Exponential);
            lbselecttype.SelectedIndex = 0;
            lbselecttype.Height = controlheight * 2 - 10;
            lbselecttype.Width = controlwidth * 3 - 10;
        }
    }

    private void CheckBoxIsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(Series.RegressionFunction.Type == FunctionType.NaF) Series.RegressionFunction.ShowFunction = false;
        if (cbactive.IsChecked is null) Series.RegressionFunction.ShowFunction = false;
        else Series.RegressionFunction.ShowFunction = (bool)cbactive.IsChecked;
    }

    private void ClickReturn(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Parent is not null)
        {
            SettingsWindow sw = (SettingsWindow)Parent;
            var view = new YAxisSettingsView(Series, CanvasGraph);
            sw.Content = view;
            sw.Width = view.Width;
            sw.Height = view.Height;
            sw.MaxHeight = view.Height;
            sw.MaxWidth = view.Width;
        }
        else ErrorWindow.Show("error: parent window of view RegressionSettingsView is null");
    }

    private void ClickApplyPrecision(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(tboprecision.Text is not null) Series.Precision = CustomConvert.StringToInt(tboprecision.Text);
        LoadControls();
    }

    private void ClickCalculate(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CanvasGraph.GraphData is null)ErrorWindow.Show("error: pag is null -> settings file not found");
        else
        {   
            if (tboorder.Text is not null && lbselecttype.SelectedItem is not null)
            {
                try
                {
                    var order = CustomConvert.StringToInt(tboorder.Text);
                    var type = (FunctionType)lbselecttype.SelectedItem;
                    Series.CalculateRegressionFunction(type, order);
                    Series.RegressionFunction.ShowFunction = true;
                    LoadControls();
                    CanvasGraph.ReDraw();
                }
                catch (System.Exception ex) {ErrorWindow.Show(ex.ToString());}
            }
            else ErrorWindow.Show(string.Format("error: value is null.\ntboorder.Text={0}\nlbselecttype.SelectedItem={1}", tboorder.Text, lbselecttype.SelectedItem));
        }
    }
}