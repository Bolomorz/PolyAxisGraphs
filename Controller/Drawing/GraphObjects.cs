using System.Collections.Generic;
using PolyAxisGraphs.Data;

namespace PolyAxisGraphs.Drawing;

internal class Line
{
    internal required Point Start;
    internal required Point End;
    internal required Avalonia.Media.Color Color;
    internal required double Thickness;
}

internal class Text
{
    internal required double Left, Top, Right, Bottom;
    internal required string Content;
    internal required double FontSize;
}

internal class Rectangle
{
    internal required double Left, Top, Right, Bottom, Width, Height;
    internal static Rectangle Create()
    {
        return new()
        {
            Left = 0,
            Top = 0,
            Right = 0,
            Bottom = 0,
            Width = 0,
            Height = 0
        };
    }
}

internal class Point
{
    internal required double X, Y;
}

internal class PointRange
{
    internal required Point Center;
    internal required double Range;
}

internal class SeriesData
{
    internal required Series Series;
    internal required Point SeriesPoint;
    internal required Point ChartPoint;
}

internal class ChartData
{
    internal string? ErrorMessage;
    internal Rectangle? TitleArea, DateArea, ChartArea, LegendArea, YAxisArea, FunctionArea;
    internal List<Line>? Lines;
    internal List<Text>? Texts;
    internal List<FunctionStringCollection>? FunctionStrings;
}