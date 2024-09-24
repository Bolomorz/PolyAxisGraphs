using System.Collections.Generic;

namespace PolyAxisGraphs.Data;

internal class XAxis
{
    internal required string Name { get; set; }
    internal required string Unit { get; set; }
    internal required double LastX { get; set; }
    internal required int XMin { get; set; }
    internal required int XMax { get; set; }
    internal required int DefXMin { get; set; }
    internal required int DefXMax { get; set; }
    internal static XAxis Create()
    {
        return new()
        {
            Name = "",
            Unit = "",
            LastX = double.MinValue,
            XMin = 0,
            DefXMin = 0,
            XMax = 0,
            DefXMax = 0
        };
    }
}

internal class ChartData
{
    internal required XAxis XAxis { get; set; }
    internal required List<Series> Series { get; set; }
    internal void Reset()
    {
        Series = new();
        XAxis = XAxis.Create();
    }
}