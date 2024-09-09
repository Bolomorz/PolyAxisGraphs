using Avalonia.Controls;
using PolyAxisGraphs.Data;

namespace PolyAxisGraphs.Views;

public partial class XAxisSettingsView : UserControl
{
    CanvasGraph CanvasGraph { get; set; }

    const int heightfactor = 4;
    const int controlcount = 5;
    const int width = 600;

    public XAxisSettingsView(CanvasGraph canvasgraph)
    {
        CanvasGraph = canvasgraph;
        if (CanvasGraph.GraphData is null) ErrorWindow.Show("error: pag is null -> probably settings file not found");
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
        if (CanvasGraph.GraphData is null)ErrorWindow.Show("error: pag is null -> settings file not found");
        else
        {
            var ff = new Avalonia.Media.FontFamily("Consolas");
            var fs = Settings.ControlFontSize;

                var controlheight = this.Height / controlcount - 10;
                var controlwidth = this.Width / 4;

                tbltitle.FontFamily = ff;
                tbltitle.FontSize = (int)fs;
                tbltitle.Text = Language.TbTitleSettingsX;
                tbltitle.Width = controlwidth * 4 - 10;
                tbltitle.Height = controlheight;

                tblname.FontFamily = ff;
                tblname.FontSize = (int)fs;
                tblname.Text = Language.TbNameSettingsX;
                tblname.Width = controlwidth - 10;
                tblname.Height = controlheight;
                tboname.FontFamily = ff;
                tboname.FontSize = (int)fs;
                tboname.Text = CanvasGraph.GraphData.XAxisName;
                tboname.Width = controlwidth * 3 - 10;
                tboname.Height = controlheight;

                tblmin.FontFamily = ff;
                tblmin.FontSize = (int)fs;
                tblmin.Text = Language.TbMinValue;
                tblmin.Width = controlwidth - 10;
                tblmin.Height = controlheight;
                tbomin.FontFamily = ff;
                tbomin.FontSize = (int)fs;
                tbomin.Text = CanvasGraph.GraphData.XMin.ToString();
                tbomin.Width = controlwidth * 2 - 10;
                tbomin.Height = controlheight;
                btmin.FontFamily = ff;
                btmin.FontSize = (int)fs;
                btmin.Content = Language.BtReset;

                tblmax.FontFamily = ff;
                tblmax.FontSize = (int)fs;
                tblmax.Text = Language.TbMaxValue;
                tblmax.Width = controlwidth - 10;
                tblmax.Height = controlheight;
                tbomax.FontFamily = ff;
                tbomax.FontSize = (int)fs;
                tbomax.Text = CanvasGraph.GraphData.XMax.ToString();
                tbomax.Width= controlwidth * 2 - 10;
                tbomax.Height = controlheight;
                btmax.FontFamily = ff;
                btmax.FontSize = (int)fs;
                btmax.Content = Language.BtReset;

                btapply.FontFamily = ff;
                btapply.FontSize = (int)fs;
                btapply.Content = Language.BtApply;
                btdiscard.FontFamily = ff;
                btdiscard.FontSize = (int)fs;
                btdiscard.Content = Language.BtDiscard;
        }
    }

    private void ClickApply(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CanvasGraph.GraphData is null) ErrorWindow.Show("error: pag is null -> settings file not found");
        else
        {
            if (tboname.Text is not null) CanvasGraph.GraphData.XAxisName = tboname.Text; else ErrorWindow.Show("error: tboname.Text is null in XAxisSettingsView");
            if (tbomin.Text is not null) CanvasGraph.GraphData.XMin = CustomConvert.StringToInt(tbomin.Text); else ErrorWindow.Show("error: tbomin.Text is null in XAxisSettingsView");
            if (tbomax.Text is not null) CanvasGraph.GraphData.XMax = CustomConvert.StringToInt(tbomax.Text); else ErrorWindow.Show("error: tbomax.Text is null in XAxisSettingsView");
            if (Parent is not null)
            {
                CanvasGraph.ReDraw();
                SettingsWindow sw = (SettingsWindow)Parent;
                sw.Close();
            }
            else ErrorWindow.Show("error: parent window of view XAxisSettingsView is null");
        }
    }

    private void ClickDiscard(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Parent is not null)
        {
            SettingsWindow sw = (SettingsWindow)Parent;
            sw.Close();
        }
        else ErrorWindow.Show("error: parent window of view XAxisSettingsView is null");
    }

    private void ClickResetMin(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CanvasGraph.GraphData is null) ErrorWindow.Show("error: pag is null -> settings file not found");
        else tbomin.Text = CanvasGraph.GraphData.DefXMin.ToString();
    }

    private void ClickResetMax(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CanvasGraph.GraphData is null) ErrorWindow.Show("error: pag is null -> settings file not found");
        else tbomax.Text = CanvasGraph.GraphData.DefXMax.ToString();
    }
}