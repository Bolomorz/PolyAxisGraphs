namespace PolyAxisGraphs.Data;

internal class GraphData
{
    internal string ChartTitle { get; set; }
    internal FileReader FileReader { get; set; }

    internal ChartData ChartData { get; set; }
    internal GraphData()
    {
        FileReader = new();
        ChartTitle = "";
        ChartData = new()
        {
            Series = new(),
            XAxis = XAxis.Create()
        };
    }

    internal void SetChartTitle(string title)
    {
        ChartTitle = title;
    }

    internal void SetLanguage(string file)
    {

    }

    internal void ReadData()
    {
        FileReader.ReadData(ChartData);
    }
}