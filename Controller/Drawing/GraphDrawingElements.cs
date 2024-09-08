using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PolyAxisGraphs.Data;

namespace PolyAxisGraphs.Drawing;

internal class GraphDrawingElements
{
    private double CanvasWidth;
    private double CanvasHeight;
    private GraphData GraphData;
    private int SeriesCount;
    private Rectangle TitleArea, DateArea, ChartArea, LegendArea, YAxisArea, FunctionArea;
    private List<Line> Lines;
    private List<Text> Texts;
    private List<FunctionStringCollection> FunctionStrings;
    private List<SeriesData> SeriesDatas;

    internal GraphDrawingElements(double canvaswidth, double canvasheight, GraphData gd)
    {
        CanvasWidth = canvaswidth;
        CanvasHeight = canvasheight;
        GraphData = gd;
        SeriesCount = 0;
        foreach(var series in gd.Series) if(series.Active) SeriesCount++;
        Lines = new();
        Texts = new();
        FunctionStrings = new();
        SeriesDatas = new();
        TitleArea = Rectangle.Create();
        DateArea = Rectangle.Create();
        ChartArea = Rectangle.Create();
        LegendArea = Rectangle.Create();
        YAxisArea = Rectangle.Create();
        FunctionArea = Rectangle.Create();
    }

    internal SeriesData? TranslateChartPointToSeriesPoint(PointRange range)
    {
        if(range.Center.X < ChartArea.Left || range.Center.X > ChartArea.Right || range.Center.Y < ChartArea.Top || range.Center.Y > ChartArea.Bottom) return null;

        foreach(var data in SeriesDatas)
        {
            if(IsPointInRange(range, data.ChartPoint)) return data;
        }

        return null;
    }
    internal Point? TranslateSeriesPointToChartPoint(double x, double y, double x1, double x2, double y1, double y2)
    {
        if(x < x1 || x > x2) return null;

        double chartx, charty;
        if (y < y1)
        {
            double xpercent = (x - x1) / (x2 - x1);
            double chartoffsetx = (ChartArea.Right - ChartArea.Left) * xpercent;
            chartx = ChartArea.Left + chartoffsetx;

            double ypercent = (-1) * (y - y1) / (y2 - y1);
            double chartoffsety = (ChartArea.Bottom - ChartArea.Top) * ypercent;
            charty = ChartArea.Bottom + chartoffsety;
        }
        else if (y > y2)
        {
            double xpercent = (x - x1) / (x2 - x1);
            double chartoffsetx = (ChartArea.Right - ChartArea.Left) * xpercent;
            chartx = ChartArea.Left + chartoffsetx;

            double ypercent = (y - y1) / (y2 - y1) - 1;
            double chartoffsety = (ChartArea.Bottom - ChartArea.Top) * ypercent;
            charty = ChartArea.Top - chartoffsety;
        }
        else
        {
            double xpercent = (x - x1) / (x2 - x1);
            double chartoffsetx = (ChartArea.Right - ChartArea.Left) * xpercent;
            chartx = ChartArea.Left + chartoffsetx;

            double ypercent = (y - y1) / (y2 - y1);
            double chartoffsety = (ChartArea.Bottom - ChartArea.Top) * ypercent;
            charty = ChartArea.Bottom - chartoffsety;
        }

        return new Point() { X = chartx, Y = charty };
    }

    internal void ChangeTitle(string title, double fontsize)
    {
        if(Texts.Count > 0)
        {
            var text = Texts[0];
            text.Content = title;
            text.FontSize = fontsize;
        }
    }

    internal ChartData CalculateChart()
    {
        Lines.Clear();
        FunctionStrings.Clear();
        Texts.Clear();
        SeriesDatas.Clear();

        if(SeriesCount == 0) return new ChartData() { ErrorMessage = "no data to display." };

        var error = CalculateChartAreas();
        if(error is not null) return new ChartData() { ErrorMessage = error };

        error = AddChart(Settings.ChartGridIntervall, GraphData.XMin, GraphData.XMax, Settings.ChartFontSize);
        if(error is not null) return new ChartData() { ErrorMessage = error };

        double xarea = YAxisArea.Left;
        double xareaintervall = Settings.YAxisWidth;
        foreach(var series in GraphData.Series)
        {
            if(series.Active)
            {
                error = AddSeries(series, Settings.ChartGridIntervall, GraphData.XMin, GraphData.XMax, Settings.ChartFontSize, xarea);
                if(error is not null) return new ChartData() { ErrorMessage = error };
                xarea += xareaintervall;
            }
        }

        return new()
        {
            ErrorMessage = null,
            TitleArea = this.TitleArea,
            DateArea = this.DateArea,
            LegendArea = this.LegendArea,
            YAxisArea = this.YAxisArea,
            ChartArea = this.ChartArea,
            FunctionArea = this.FunctionArea,
            Lines = this.Lines,
            Texts = this.Texts,
            FunctionStrings = this.FunctionStrings
        };
    }

    private bool IsPointInRange(PointRange range, Point point)
    {
        return point.X >= range.Center.X - range.Range && point.X <= range.Center.X + range.Range && point.Y >= range.Center.Y - range.Range && point.Y <= range.Center.Y + range.Range;
    }
    private string? CalculateChartAreas()
    {
        double d = SeriesCount * Settings.YAxisWidth;
        if(d > 0.5 * CanvasWidth) return "canvas area too small to display graph";
        if(d == 0) d = 20;
        d = d/CanvasWidth;

        var lrect = CalculateRectangle(GraphDrawingSettings.HorizontalStartLeft, d, GraphDrawingSettings.VerticalStartTop, GraphDrawingSettings.VerticalEndTop);
        LegendArea = lrect;
        var trect = CalculateRectangle(d + 0.01, GraphDrawingSettings.HorizontalEndMid, GraphDrawingSettings.VerticalStartTop, GraphDrawingSettings.VerticalEndTop);
        TitleArea = trect;
        var drect = CalculateRectangle(GraphDrawingSettings.HorizontalStartRight, GraphDrawingSettings.HorizontalEndRight, GraphDrawingSettings.VerticalStartTop, GraphDrawingSettings.VerticalEndTop);
        DateArea = drect;
        var yrect = CalculateRectangle(GraphDrawingSettings.HorizontalStartLeft, d, GraphDrawingSettings.VerticalStartBottom, GraphDrawingSettings.VerticalEndBottom);
        YAxisArea = yrect;
        var crect = CalculateRectangle(d + 0.01, GraphDrawingSettings.HorizontalEndMid, GraphDrawingSettings.VerticalStartBottom, GraphDrawingSettings.VerticalEndBottom);
        ChartArea = crect;
        var farea = CalculateRectangle(GraphDrawingSettings.HorizontalStartRight, GraphDrawingSettings.HorizontalEndRight, GraphDrawingSettings.VerticalStartBottom, GraphDrawingSettings.VerticalEndBottom);
        FunctionArea = farea;

        return null;
    }
    private Rectangle CalculateRectangle(double x1, double x2, double y1, double y2)
    {
        return new()
        {
            Left = CanvasWidth * x1,
            Top = CanvasHeight * y1,
            Right = CanvasWidth * x2,
            Bottom = CanvasHeight * y2,
            Width = CanvasWidth * x2 - CanvasWidth * x1,
            Height = CanvasHeight * y2 - CanvasHeight * y1
        };
    }
    private void AddText(double left, double right, double top, double bottom, string text, double fontsize)
    {
        if(left == right || bottom - top < fontsize) return;
        Texts.Add(new()
        {
            Left = left,
            Top = top,
            Bottom = bottom,
            Right = right,
            Content = text,
            FontSize = fontsize
        });
    }
    private void AddLine(Point start, Point end, Avalonia.Media.Color color, double thickness)
    {
        if(start.X == end.X && start.Y == end.Y) return;
        Lines.Add(new()
        {
            Start = start,
            End = end,
            Color = color,
            Thickness = thickness
        });
    }
    private void AddTitle(double fontsize, string title)
    {
        double midpoint = (TitleArea.Bottom + TitleArea.Top) / 2;
        if(fontsize + 2 < TitleArea.Height) AddText(TitleArea.Left + 1, TitleArea.Right - 1, midpoint - fontsize - 1, midpoint + fontsize + 1, title, fontsize);
        else AddText(TitleArea.Left + 1, TitleArea.Right - 1, TitleArea.Top + 1, TitleArea.Bottom - 1, title, TitleArea.Height - 2);
    }
    private void AddDate(double fontsize)
    {
        double midpoint = (DateArea.Bottom + DateArea.Top) / 2;
        var today = DateTime.Today;
        if(fontsize + 2 < DateArea.Height) AddText(DateArea.Left + 1, DateArea.Right - 1, midpoint - fontsize - 1, midpoint + fontsize + 1, today.ToShortDateString(), fontsize);
        else AddText(DateArea.Left + 1, DateArea.Right - 1, DateArea.Top + 1, DateArea.Bottom - 1, today.ToShortDateString(), DateArea.Height - 2);
    }
    private string? AddChart(int gridintervallcount, int x1, int x2, double fontsize)
    {
        double xintervall = ChartArea.Width / (double)gridintervallcount;
        double yintervall = ChartArea.Height / (double)gridintervallcount;
        double numintervall = (double)(x2 - x1) / (double)gridintervallcount;
        if(xintervall < 1 || yintervall < fontsize) return string.Format("gridintervall too large to display graph. choose smaller gridintervall.\nAddChart(): yintervall {0}, fontsize {1}, xintervall {2}", yintervall, fontsize, xintervall);

        Point start = new Point() { X = ChartArea.Left, Y = ChartArea.Top};
        Point end = new Point() { X = ChartArea.Left, Y = ChartArea.Bottom};
        double text = x1;
        AddLine(start, end, Avalonia.Media.Colors.Black, 1);
        if(fontsize > (CanvasHeight - 1) - (end.Y + 1)) fontsize = (CanvasHeight - 1) - (end.Y + 1) - 1;
        AddText(start.X - xintervall/2, start.X + xintervall/2, end.Y + 1, CanvasHeight - 1, text.ToString(), fontsize);

        for(int i = 0; i < gridintervallcount; i++)
        {
            start.X += xintervall;
            end.X += xintervall;
            text += numintervall;
            AddLine(start, end, Avalonia.Media.Colors.Gray, 0.5);
            AddText(start.X - xintervall/2, start.X + xintervall/2, end.Y + 1, CanvasHeight - 1, text.ToString(), fontsize);
        }
        AddText(ChartArea.Left + ChartArea.Width / 4, ChartArea.Right - ChartArea.Width / 4, ChartArea.Bottom + fontsize + 2, ChartArea.Bottom + (fontsize + 2) * 2, GraphData.XAxisName, fontsize);

        start = new Point() { X = ChartArea.Left, Y = ChartArea.Bottom};
        end = new Point() { X = ChartArea.Right, Y = ChartArea.Bottom};
        AddLine(start, end, Avalonia.Media.Colors.Black, 1);
        for(int i = 0; i < gridintervallcount; i++)
        {
            start.Y -= yintervall;
            end.Y -= yintervall;
            AddLine(start, end, Avalonia.Media.Colors.Gray, 0.5);
        }

        return null;
    }
    private (Point A, Point B)? CalculateEdgePoints(Point start, Point end)
    {
        //start and end inside chartarea
        if(start.Y >= ChartArea.Top && start.Y <= ChartArea.Bottom && end.Y >= ChartArea.Top && end.Y <= ChartArea.Bottom) return (start, end);

        //start outside chartarea, end inside chartarea
        else if((start.Y < ChartArea.Top || start.Y > ChartArea.Bottom) && end.Y >= ChartArea.Top && end.Y <= ChartArea.Bottom)
        {
            double m = (end.Y - start.Y) / (end.X - start.X);
            double c = end.Y - m * end.X;
            double y = (start.Y < ChartArea.Top) ? ChartArea.Top : ChartArea.Bottom;
            double x = (y - c) / m;
            return new(new() { X = x, Y = y}, end);
        }

        //end outside chartarea, start inside chartarea
        else if((end.Y < ChartArea.Top || end.Y > ChartArea.Bottom) && start.Y >= ChartArea.Top && start.Y <= ChartArea.Bottom)
        {
            double m = (end.Y - start.Y) / (end.X - start.X);
            double c = end.Y - m * end.X;
            double y = (end.Y < ChartArea.Top) ? ChartArea.Top : ChartArea.Bottom;
            double x = (y - c) / m;
            return (start, new() { Y = y, X = x });
        }

        //start and end outside chartarea
        else
        {
            if(start.Y < ChartArea.Top && end.Y > ChartArea.Bottom)
            {
                double m = (end.Y - start.Y) / (end.X - start.X);
                double c = end.Y - m * end.X;
                double y = ChartArea.Top;
                double x = (y - c) / m;
                Point pstart = new(){X = x, Y = y};
                y = ChartArea.Bottom;
                x = (y - c) / m;
                Point pend = new(){X = x, Y = y};
                return (pstart, pend);
            }
            else if(start.Y > ChartArea.Bottom && end.Y < ChartArea.Top)
            {
                double m = (end.Y - start.Y) / (end.X - start.X);
                double c = end.Y - m * end.X;
                double y = ChartArea.Bottom;
                double x = (y - c) / m;
                Point pstart = new(){X = x, Y = y};
                y = ChartArea.Top;
                x = (y - c) / m;
                Point pend = new(){X = x, Y = y};
                return (pstart, pend);
            }
            else return null;
        }        
    }
    private string? AddSeries(Series series, int gridintervallcount, int x1, int x2, double fontsize, double xarea)
    {
        double yintervall = YAxisArea.Height / gridintervallcount;
        double numintervall = (series.YSetMax - series.YSetMin) / gridintervallcount;
        if(yintervall < fontsize) return String.Format("gridintervall too large to display graph. choose smaller chartgridintervall.\nAddSeries(): yintervall {0}, fontsize {1}",yintervall, fontsize);

        //Add Y Axis
        Point start = new() { X = xarea, Y = YAxisArea.Bottom };
        Point end = new() { X = xarea, Y = YAxisArea.Top };
        AddLine(start, end, series.Color, 1);

        //Add Y Axis Grid + Text
        double length = (double)Settings.YAxisWidth/4;
        start = new() { X = xarea, Y = YAxisArea.Bottom };
        end = new() { X = xarea + length, Y = YAxisArea.Bottom };
        double text = Math.Round(series.YSetMin, 2);
        while (start.Y >= YAxisArea.Top)
        {
            AddLine(start, end, series.Color, 0.5);
            AddText(start.X, start.X + length*4, start.Y - fontsize, start.Y, text.ToString(), fontsize);
            start.Y -= yintervall;
            end.Y -= yintervall;
            text += numintervall;
            text = Math.Round(text, 2);
        }

        //Add Legend
        start = new() { X = xarea, Y = LegendArea.Bottom };
        end = new() { X = xarea, Y = LegendArea.Top };
        AddLine(start, end, series.Color, 1);
        AddText(xarea, start.X + length * 4, LegendArea.Top, LegendArea.Bottom, series.Name, fontsize);

        //Add Functionstring
        var function = series.GetFunction();
        FunctionStrings.Add(function);

        //Draw Series
        List<Point> seriespoints = new();
        for(int i = 0; i < series.XValues.Count; i++)
        {
            double xval = series.XValues[i];
            double yval = series.YValues[i];
            if(xval >= x1 && xval <= x2) seriespoints.Add(new() { X = xval, Y = yval });
        }
        if(seriespoints.Count > 0)
        {
            Point? seriesstart = null;
            int i = 0;
            while(seriesstart is null && i < seriespoints.Count)
            {
                var newpoint = TranslateSeriesPointToChartPoint(seriespoints[i].X, seriespoints[i].Y, x1, x2, series.YSetMin, series.YSetMax);
                if (newpoint is null) seriesstart = null;
                else
                {
                    Point np = (Point)newpoint;
                    if (np.Y < ChartArea.Top || np.Y > ChartArea.Bottom) seriesstart = null;
                    else seriesstart = np;
                }
                i++;
            }
                
            if (seriesstart is not null)
            {
                SeriesDatas.Add(new() { Series = series, SeriesPoint = seriespoints[i-1], ChartPoint = (Point)seriesstart });
                while (i < seriespoints.Count)
                {
                    var seriesend = TranslateSeriesPointToChartPoint(seriespoints[i].X, seriespoints[i].Y, x1, x2, series.YSetMin, series.YSetMax);
                    if (seriesend is not null)
                    {
                        Point s = (Point)seriesstart;
                        Point e = (Point)seriesend;
                        var edgepoints = CalculateEdgePoints(s, e);
                        if(edgepoints is not null)
                        {
                            (Point A, Point B) ep = ((Point A, Point B))edgepoints;
                            AddLine(ep.A, ep.B, series.Color, 1);
                            seriesstart = seriesend;
                            SeriesDatas.Add(new() { Series = series, SeriesPoint = seriespoints[i], ChartPoint = (Point)seriesstart });
                        }
                        else
                        {
                            seriesstart = seriesend;
                        }
                    }
                    i++;
                }
            }
        }

        //Draw Regressionfunction
        if (series.RegressionFunction.ShowFunction && series.RegressionFunction.Type != FunctionType.NaF)
        {
            List<Point> functionpoints = new List<Point>();
            double xintervall = (double)(x2 - x1) / 100.0d;
            double xval = x1;
            while (xval <= x2)
            {
                double yval = GraphData.CalculateYValue(xval, series.RegressionFunction);
                if (xval >= x1 && xval <= x2) functionpoints.Add(new() { X = xval, Y = yval });
                xval += xintervall;
            }

            if(functionpoints.Count > 0)
            {
                Point? functionstart = null;
                int i = 0;
                while (functionstart is null && i < functionpoints.Count)
                {
                    var newpoint = TranslateSeriesPointToChartPoint(functionpoints[i].X, functionpoints[i].Y, x1, x2, series.YSetMin, series.YSetMax);
                    if (newpoint is null) functionstart = null;
                    else
                    {
                        Point np = (Point)newpoint;
                        if (np.Y < ChartArea.Top || np.Y > ChartArea.Bottom) functionstart = null;
                        else functionstart = np;
                    }
                    i++;
                }
                if (functionstart is not null)
                {
                    bool draw = true;
                    SeriesDatas.Add(new() { Series = series, SeriesPoint = functionpoints[i - 1], ChartPoint = (Point)functionstart });
                    while (i < functionpoints.Count)
                    {
                        var functionend = TranslateSeriesPointToChartPoint(functionpoints[i].X, functionpoints[i].Y, x1, x2, series.YSetMin, series.YSetMax);
                        if (functionend is not null && draw)
                        {
                            Point s = (Point)functionstart;
                            Point e = (Point)functionend;
                            var edgepoints = CalculateEdgePoints(s, e);
                            if (edgepoints is not null)
                            {
                                (Point A, Point B) ep = ((Point A, Point B))edgepoints;
                                AddLine(ep.A, ep.B, series.Color, 1);
                                functionstart = functionend;
                                SeriesDatas.Add(new() { Series = series, SeriesPoint = functionpoints[i], ChartPoint = (Point)functionstart });
                            }
                            else
                            {
                                functionstart = functionend;
                            }
                        }
                        else if (functionend is not null && !draw)
                        {
                            draw = true;
                            Point s = (Point)functionstart;
                            Point e = (Point)functionend;
                            var edgepoints = CalculateEdgePoints(s, e);
                            if (edgepoints is not null)
                            {
                                (Point A, Point B) ep = ((Point A, Point B))edgepoints;
                                functionstart = functionend;
                                SeriesDatas.Add(new() { Series = series, SeriesPoint = functionpoints[i], ChartPoint = (Point)functionstart });
                            }
                            else
                            {
                                functionstart = functionend;
                            }
                        }
                        i++;
                    }
                }
            }
        }

        return null;
    }
}