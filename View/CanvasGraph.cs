using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Controls.Documents;
using PolyAxisGraphs.Data;
using PolyAxisGraphs.Drawing;

namespace PolyAxisGraphs.Views;

public class CanvasGraph
{
    internal GraphData? GraphData { get; set; }
    internal Canvas Canvas { get; set; }
    internal GraphDrawingElements? DrawingElements { get; set; }
    internal MainView MainView { get; set; }

    internal CanvasGraph(Canvas canvas, MainView view)
    {
        Canvas = canvas;
        try
        {
            GraphData = new();
        }
        catch (Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
        DrawingElements = null;
        MainView = view;
    }

    public void SetTitle(string title)
    {
        try
        {
            if (GraphData is null) ErrorWindow.Show("error: pag is null -> probably settings file not found");
            else
            {
                GraphData.ChartTitle = title;
                var c = (TextBlock)Canvas.Children[0];
                c.Text = title;
            }
        }
        catch (Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }
    public void SetLanguage(string lngfile)
    {
        try
        {
            if (GraphData is null) ErrorWindow.Show("error: pag is null -> probably settings file not found");
            else GraphData.SetLanguage(lngfile);
            }
        catch (Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }
    public void SetFile(string datafile)
    {
        try
        {
            if (GraphData is null) ErrorWindow.Show("error: pag is null -> probably settings file not found");
            else
            { 
                GraphData.SetFilePath(datafile);
                GraphData.ReadData();
                DrawingElements = new GraphDrawingElements(Canvas.Width, Canvas.Height, GraphData);
                DrawGDE();
            }
        }
        catch (Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }
    public void ReDraw()
    {
        try
        {
            if (GraphData is null) ErrorWindow.Show("error: pag is null -> probably settings file not found");
            else 
            {
                DrawingElements = new GraphDrawingElements(Canvas.Width, Canvas.Height, GraphData);
                DrawGDE();
            }
        }
        catch (Exception ex)
        {
            ErrorWindow.Show(ex.ToString());
        }
    }

    private void DrawGDE()
    {
        if (DrawingElements is null || GraphData is null) return;
        Canvas.Children.Clear();
        FontFamily ff = new("Consolas") ;
        double fontsize = Settings.ChartFontSize;
        var sol = DrawingElements.CalculateChart();
        if (sol.ErrorMessage is not null) DrawText(10, ff, sol.ErrorMessage, 0, 0, Canvas.Width, Canvas.Height);
        else
        {
            if (sol.Texts is not null) foreach (var text in sol.Texts) DrawText(text.FontSize, ff, text.Content, text.Left, text.Top, text.Right, text.Bottom);
            if (sol.Lines is not null) foreach (var line in sol.Lines) DrawLine(new Avalonia.Point(line.Start.X, line.Start.Y), new Avalonia.Point(line.End.X, line.End.Y), new SolidColorBrush(line.Color), line.Thickness);
            if (sol.FunctionStrings is not null && sol.FunctionStrings.Count > 0)
            {
                double left, top, height, width, right, bottom;
                if (sol.FunctionArea is null)
                {
                    left = 0.91 * Canvas.Width;
                    top = 0.11 * Canvas.Height;
                    right = 0.99 * Canvas.Width;
                    bottom = 0.95 * Canvas.Height;
                    height = bottom - top;
                    width = right - left;
                }
                else
                {
                    var area = (PolyAxisGraphs.Drawing.Rectangle)sol.FunctionArea;
                    left = area.Left;
                    top = area.Top;
                    right = area.Right;
                    bottom = area.Bottom;
                    height = area.Height;
                    width = area.Width;
                }
                double intervall = height / sol.FunctionStrings.Count;
                foreach (var function in sol.FunctionStrings)
                {
                    if (function is not null && function.FunctionStrings.Count > 0) DrawFunctionText(function.FunctionStrings, fontsize, ff, left, top, top + intervall, right);
                    top += intervall;
                }
            }
        }
        MainView.CreateControls();
    }

    private void DrawLine(Avalonia.Point start, Avalonia.Point end, ISolidColorBrush brush, double thickness)
    {
        Avalonia.Controls.Shapes.Line line = new()
        {
            StartPoint = start,
            EndPoint = end,
            Stroke = brush,
            StrokeThickness = thickness
        };
        Canvas.Children.Add(line);
    }
    private void DrawText(double fontsize, Avalonia.Media.FontFamily fontFamily, string text, double left, double top, double right, double bottom)
    {
        TextBlock tb = new()
        {
            FontSize = fontsize,
            FontFamily = fontFamily,
            Text = text,
            Width = right - left,
            Height = bottom - top,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
        };
        Canvas.SetLeft(tb, left);
        Canvas.SetTop(tb, top);
        Canvas.SetRight(tb, right);
        Canvas.SetBottom(tb, bottom);
        Canvas.Children.Add(tb);
    }
    private void DrawFunctionText(List<FunctionString> strings, double fontsize, FontFamily fontFamily, double left, double top, double bottom, double right)
    {
        var color = new SolidColorBrush(strings[0].Color);
        TextBlock outer = new()
        {
            FontSize = fontsize,
            Foreground = color,
            FontFamily = fontFamily,
            Width = right - left,
            Height = bottom - top,
            TextWrapping = TextWrapping.WrapWithOverflow
        };
        outer.Inlines = new InlineCollection();
        foreach (var fs in strings)
        {
            if (fs.SuperScript)
            {
                Run run = new()
                {
                    FontSize = fontsize * 2 / 3,
                    FontFamily = fontFamily,
                    BaselineAlignment = BaselineAlignment.Top,
                    Text = fs.Function
                };
                outer.Inlines.Add(run);
            }
            else
            {
                Run run = new()
                {
                    FontSize = fontsize,
                    FontFamily = fontFamily,
                    BaselineAlignment = BaselineAlignment.Center,
                    Text = fs.Function
                };
                outer.Inlines.Add(run);
            }
        }
        Canvas.SetLeft(outer, left);
        Canvas.SetTop(outer, top);
        Canvas.SetBottom(outer, bottom);
        Canvas.SetRight(outer, right);
        Canvas.Children.Add(outer);
    }
    private void DrawEllipse(double width, double height, ISolidColorBrush fill, ISolidColorBrush stroke, double thickness, double left, double top)
    {
        Ellipse ellipse = new()
        {
            Width = width,
            Height = height,
            Fill = fill,
            Stroke = stroke,
            StrokeThickness = thickness
        };
        Canvas.SetLeft(ellipse, left);
        Canvas.SetTop(ellipse, top);
        Canvas.Children.Add(ellipse);
    }
    private void DrawRectangle(double width, double height, ISolidColorBrush fill, ISolidColorBrush stroke, double thickness, double left, double top)
    {
        Avalonia.Controls.Shapes.Rectangle rectangle = new()
        {
            Width = width,
            Height = height,
            Fill = fill,
            Stroke = stroke,
            StrokeThickness = thickness
        };
        Canvas.SetLeft(rectangle, left);
        Canvas.SetTop(rectangle, top);
        Canvas.Children.Add(rectangle);
    }
}