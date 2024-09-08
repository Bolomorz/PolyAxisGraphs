using System.Collections.Generic;

namespace PolyAxisGraphs.Drawing;

internal class FunctionString
{
    internal required string Function { get; set; }
    internal required bool SuperScript { get; set; }
    internal required Avalonia.Media.Color Color { get; set; }
}

internal class FunctionStringCollection
{
    internal List<FunctionString> FunctionStrings { get; set;} = new();
}