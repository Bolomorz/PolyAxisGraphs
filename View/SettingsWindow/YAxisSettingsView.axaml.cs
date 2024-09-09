using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using PolyAxisGraphs.Data;

namespace PolyAxisGraphs.Views;

public partial class YAxisSettingsView : UserControl
{
    CanvasGraph CanvasGraph { get; set; }
    Series Series { get; set; }

    const int heightfactor = 4;
    const int controlcount = 6;
    const int width = 600;

    public YAxisSettingsView(Series series, CanvasGraph canvasgraph)
    {
        CanvasGraph = canvasgraph;
        Series = series;
        if (CanvasGraph.GraphData is null)ErrorWindow.Show("error: pag is null -> settings file not found");
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

            var controlheight = this.Height / controlcount - 10;
            var controlwidth = this.Width / 4;

            tbltitle.FontFamily = ff;
            tbltitle.FontSize = (int)fs;
            tbltitle.Text = Language.TbTitleSettingsY;
            tbltitle.Width = controlwidth * 4 - 10;
            tbltitle.Height = controlheight;

            btfunc.FontFamily = ff;
            btfunc.FontSize = (int)fs;
            btfunc.Content = (Series.RegressionFunction.ShowFunction) ? Language.BtFuncActive : Language.BtFuncInactive;
            btfunc.Background = (Series.RegressionFunction.ShowFunction) ? Brushes.Green : Brushes.Red;

            tblname.FontFamily = ff;
            tblname.FontSize = (int)fs;
            tblname.Text = Language.TbNameSettingsY;
            tblname.Width = controlwidth - 10;
            tblname.Height = controlheight;
            tboname.FontFamily = ff;
            tboname.FontSize = (int)fs;
            tboname.Text = Series.Name;
            tboname.Width = controlwidth * 3 - 10;
            tboname.Height = controlheight;

            cbseries.FontFamily = ff;
            cbseries.FontSize = (int)fs;
            cbseries.Content = Language.CbSeries;
            cbseries.IsChecked = (Series.Active) ? true : false;

            tblcolor.FontFamily = ff;
            tblcolor.FontSize = (int)fs;
            tblcolor.Text = Language.TbColorSettingsY;
            tblcolor.Width = controlwidth - 10;
            tblcolor.Height = controlheight;
            tbocolor.FontFamily = ff;
            tbocolor.FontSize = (int)fs;
            var c = System.Drawing.Color.FromArgb(Series.Color.A, Series.Color.R, Series.Color.G, Series.Color.B);
            tbocolor.Text = c.Name;
            tbocolor.Width = controlwidth * 3 - 10;
            tbocolor.Height = controlheight;

            tblmin.FontFamily = ff;
            tblmin.FontSize = (int)fs;
            tblmin.Text = Language.TbMinValue;
            tblmin.Width = controlwidth - 10;
            tblmin.Height = controlheight;
            tbomin.FontFamily = ff;
            tbomin.FontSize = (int)fs;
            tbomin.Text = Series.YSetMin.ToString();
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
            tbomax.Text = Series.YSetMax.ToString();
            tbomax.Width = controlwidth * 2 - 10;
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

    private void ClickFunc(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(Parent is not null)
        {
            try
            {
                SettingsWindow sw = (SettingsWindow)Parent;
                var view = new RegressionSettingsView(Series, CanvasGraph);
                sw.Content = view;
                sw.Width = view.Width;
                sw.Height = view.Height;
                sw.MaxWidth = view.Width;
                sw.MaxHeight = view.Height;
            }
            catch (Exception ex)
            {
                ErrorWindow.Show(ex.ToString());
            }
        }
        else
        {
            ErrorWindow.Show("error: parent window of view yaxissettingsview is null");
        }
    }

    private void ClickApply(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (tboname.Text is not null) Series.Name = tboname.Text; else ErrorWindow.Show("error: tboname.Text of view yaxissettingsview is null");
        if (tbomin.Text is not null) Series.SetMin(CustomConvert.StringToDouble(tbomin.Text)); else ErrorWindow.Show("error: tbomin.Text of view yaxissettingsview is null");
        if (tbomax.Text is not null) Series.SetMax(CustomConvert.StringToDouble(tbomax.Text)); else ErrorWindow.Show("error: tbomax.Text of view yaxissettingsview is null");
        if (tbocolor.Text is not null)
        {
            var c = System.Drawing.Color.FromName(tbocolor.Text); 
            Series.Color = Avalonia.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        } 
        else ErrorWindow.Show("error: tbocolor.Text of view yaxissettingsview is null");
        Series.Active = (cbseries.IsChecked is null) ? true : (bool)cbseries.IsChecked;
        if (Parent is not null)
        {
            try
            {
                CanvasGraph.ReDraw();
                SettingsWindow sw = (SettingsWindow)Parent;
                sw.Close();
            }
            catch (Exception ex)
            {
                ErrorWindow.Show(ex.ToString());
            }
        }
        else
        {
            ErrorWindow.Show("error: parent window of view yaxissettingsview is null");
        }
    }

    private void ClickDiscard(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Parent is not null)
        {
            try
            {
                SettingsWindow sw = (SettingsWindow)Parent;
                sw.Close();
            }
            catch (Exception ex)
            {
                    ErrorWindow.Show(ex.ToString());
            }
        }
        else ErrorWindow.Show("error: parent window of view yaxissettingsview is null");
    }

    private void ClickResetMin(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        tbomin.Text = Series.YMin.ToString();
        Series.ResetMin();
    }

    private void ClickResetMax(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        tbomax.Text = Series.YMax.ToString();
        Series.ResetMax();
    }
}
