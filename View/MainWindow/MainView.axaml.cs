using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.IO;
using PolyAxisGraphs.Drawing;
using PolyAxisGraphs.Data;
using ReactiveUI;
using System;

namespace PolyAxisGraphs.Views;

public partial class MainView : UserControl
{
    CanvasGraph CanvasGraph { get; set; }
    Uri BaseDirectory;
    Uri InitialDirectory;
    Uri LanguageDirectory;

    public static FilePickerFileType DataFiles = new("DataFiles") { Patterns = new[] { "*.txt", "*.csv"} };
    public static FilePickerFileType Images = new("Images") { Patterns = new[] { "*.png" } };
    
    public MainView()
    {
        InitializeComponent();
        BaseDirectory = new(System.AppDomain.CurrentDomain.BaseDirectory);
        InitialDirectory = new(BaseDirectory, Settings.InitialDirectory);
        LanguageDirectory = new(BaseDirectory, "LanguageFiles");
        CheckFileSystem();
        CanvasGraph = new CanvasGraph(MainCanvas, this);
        LoadControls();
        DataContext = this;
    }

    private void CheckFileSystem()
    {
        if (!Directory.Exists(InitialDirectory.AbsolutePath)) Directory.CreateDirectory(InitialDirectory.AbsolutePath);
        if (!Directory.Exists(LanguageDirectory.AbsolutePath)) Directory.CreateDirectory(LanguageDirectory.AbsolutePath);
    }

    public void LoadControls()
    {
        if (CanvasGraph.GraphData is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        var fontfamily = new Avalonia.Media.FontFamily("Consolas");
        double fontsize = Settings.ControlFontSize;
        TBFile.FontFamily = fontfamily;
        TBFile.FontSize = (double)fontsize;
        TBFile.Text = Language.TbOpenFile;
        BTOpenFile.FontFamily = fontfamily;
        BTOpenFile.FontSize = (double)fontsize;
        BTOpenFile.Content = Language.BtOpenFile;
        TBPos.FontFamily = fontfamily;
        TBPos.FontSize = (double)fontsize;
    }

    public void CreateControls()
    {
        if (CanvasGraph.GraphData is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        ControlsGrid.Children.Clear();
        var fontfamily =  new Avalonia.Media.FontFamily("Consolas");
        var fontsize = Settings.ControlFontSize;
        try {
            Button BTSave = new Button();
            BTSave.Content = Language.BtSaveFile;
            BTSave.FontFamily = fontfamily;
            BTSave.FontSize = (double)fontsize;
            BTSave.Command = ReactiveCommand.Create(() => { SaveFileButtonClick(); });
            BTSave.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            BTSave.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(BTSave, 0);
            Grid.SetRow(BTSave, 0);
            ControlsGrid.Children.Add(BTSave);

            Button BTXaxis = new Button();
            BTXaxis.Content = "x: " + CanvasGraph.GraphData.XAxisName;
            BTXaxis.FontFamily = fontfamily;
            BTXaxis.FontSize = (double)fontsize;
            BTXaxis.Command = ReactiveCommand.Create(XAxisButtonClick);
            BTXaxis.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            BTXaxis.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(BTXaxis, 1);
            Grid.SetRow(BTXaxis, 0);
            ControlsGrid.Children.Add(BTXaxis);

            TextBox TBTitle = new TextBox();
            TBTitle.Watermark = Language.TbEnterTitle;
            TBTitle.FontFamily = fontfamily;
            TBTitle.FontSize = (double)fontsize;
            TBTitle.Focusable = true;
            TBTitle.KeyUp += TitleTextChanged;
            TBTitle.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            TBTitle.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(TBTitle, 0);
            Grid.SetRow(TBTitle, 1);
            Grid.SetColumnSpan(TBTitle, 2);
            ControlsGrid.Children.Add(TBTitle);

            int col = 2;
            int row = 0;
            foreach(var series in CanvasGraph.GraphData.Series)
            {
                Button BTY = new Button();
                BTY.Content = "y: " + series.Name;
                BTY.FontFamily = fontfamily;
                BTY.FontSize = (double)fontsize;
                BTY.Command = ReactiveCommand.Create(() => { YAxisButtonClick(series); });
                BTY.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                BTY.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                Grid.SetColumn(BTY, col);
                Grid.SetRow(BTY, row);
                ControlsGrid.Children.Add(BTY);
                if (col == 7)
                {
                    col = 2;
                    row++;
                }
                else
                {
                    col++;
                }
                if (row == 2) throw new System.Exception("too many series to properly display");
            }
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }

    private void TitleTextChanged(object? sender, KeyEventArgs e)
    {
        if (sender is not null) {
            TextBox tb = (TextBox)sender;
            if (tb.Text is not null) CanvasGraph.SetTitle(tb.Text); else ErrorWindow.Show(string.Format("error: variable is null.\ntb.Text={0}", tb.Text));
        }
        else
        {
            ErrorWindow.Show("error: sender is null for TitleTextChanged.");
        }
    }

    private async void OpenFileButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CanvasGraph.GraphData is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        try
        {
            var toplevel = TopLevel.GetTopLevel(this);
            if (toplevel is null) return;
            var ssl = await toplevel.StorageProvider.TryGetFolderFromPathAsync(InitialDirectory);
            var files = await toplevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Data File",
                AllowMultiple = false,
                FileTypeFilter = new[] { DataFiles },
                SuggestedStartLocation = ssl
            });

            if (files is not null && files.Count >= 1)
            {
                string file = files[0].Path.AbsolutePath;
                TBFile.Text = file;
                CanvasGraph.SetFile(file);
            }
        }
        catch (System.Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }

    private void OpenSettingsButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
    }

    private async void SaveFileButtonClick()
    {
        if (CanvasGraph.GraphData is null)
        {
            ErrorWindow.Show("error: pag is null -> settings file not found");
            return;
        }
        try
        {
            var toplevel = TopLevel.GetTopLevel(this);
            if (toplevel is null) return;
            var ssl = await toplevel.StorageProvider.TryGetFolderFromPathAsync(InitialDirectory);
            var file = await toplevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save File",
                FileTypeChoices = new[] { Images },
                ShowOverwritePrompt = true,
                DefaultExtension = ".png",
                SuggestedStartLocation = ssl
            });

            if (file is not null)
            {
                PixelSize psize = new((int)MainCanvas.Width, (int)MainCanvas.Height);
                Size size = new(MainCanvas.Width, MainCanvas.Height);
                Vector dpi = new(96, 96);
                RenderTargetBitmap rtb = new RenderTargetBitmap(psize, dpi);
                MainCanvas.Measure(size);
                MainCanvas.Arrange(new Rect(size));
                rtb.Render(MainCanvas);
                rtb.Save(file.Path.AbsolutePath);
            }
        }
        catch (System.Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }

    private void XAxisButtonClick()
    {
        SettingsWindow settings = new SettingsWindow(new XAxisSettingsView(CanvasGraph));
        settings.Show();
    }

    private void YAxisButtonClick(Series series)
    {
        SettingsWindow settings = new SettingsWindow(new YAxisSettingsView(series, CanvasGraph));
        settings.Show();
    }

    private void Canvas_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (CanvasGraph.DrawingElements is null) { TBPos.Text = "no data to display"; return; }
        if (CanvasGraph.GraphData is null) { ErrorWindow.Show("error: pag is null -> settings file not found"); return; }
        else
        {
            var pointerpoint = e.GetCurrentPoint(MainCanvas);
            var canvaspoint = pointerpoint.Position;
            var data = CanvasGraph.DrawingElements.TranslateChartPointToSeriesPoint(new PointRange() { Center = new Drawing.Point() { X = canvaspoint.X, Y = canvaspoint.Y }, Range = 10 });
            if(data is not null)
            {
                var dt = (SeriesData)data;
                TBPos.Text = string.Format("SeriesPoint: {0} (x={1} [{3}]| y={2} [{0}])", dt.Series.Name, System.Math.Round(dt.SeriesPoint.X, dt.Series.Precision), System.Math.Round(dt.SeriesPoint.Y, dt.Series.Precision), CanvasGraph.GraphData.XAxisName);
            }
            else
            {
                TBPos.Text = "no seriespoint";
            }
        }
    }
}