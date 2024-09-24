using System.Collections.Generic;
using Avalonia.Media;
using System.Runtime.Serialization;

namespace PolyAxisGraphs.Data;

[DataContract(Name = "sds", IsReference = true)]
internal struct SaveDataSeries
{
    [DataMember] internal string Name { get; set; }
    [DataMember] internal string Unit { get; set; }
    [DataMember] internal List<double> X { get; set; }
    [DataMember] internal List<double> Y { get; set; }
    [DataMember] internal int Min { get; set; }
    [DataMember] internal int Max { get; set; }
    [DataMember] internal int Precision { get; set; }
    [DataMember] internal double SetMin { get; set; }
    [DataMember] internal double SetMax { get; set; }
    [DataMember] internal double Interval { get; set; }
    [DataMember] internal Color Color { get; set; }
    [DataMember] internal bool Active { get; set; }
    [DataMember] internal RegressionFunction Function { get; set; }
}

[DataContract(Name = "sdx", IsReference = true)]
internal struct SaveDataXAxis
{
    [DataMember] internal string Name { get; set; }
    [DataMember] internal string Unit { get; set; }
    [DataMember] internal double LastX { get; set; }
    [DataMember] internal int X1 { get; set; }
    [DataMember] internal int X2 { get; set; }
    [DataMember] internal int DefX1 { get; set; }
    [DataMember] internal int DefX2 { get; set; }
}

[DataContract(Name = "sdg", IsReference = true)]
internal struct SaveDataGraphData
{
    [DataMember] internal string DataFilePath { get; set; }
    [DataMember] internal string ChartTitle { get; set; }
}

[DataContract(Name = "sd", IsReference = true)]
internal class SaveData
{
    [DataMember] List<SaveDataSeries> SaveDataSeries { get; set; }
    [DataMember] SaveDataXAxis SaveDataXAxis { get; set; }
    [DataMember] SaveDataGraphData SaveDataGraphData { get; set; }
    internal SaveData()
    {
        SaveDataSeries = new();
        SaveDataGraphData = new();
        SaveDataXAxis = new();
    }

    internal static GraphData LoadFile(SaveData sd)
    {
        GraphData gd = ReadGraphData(sd.SaveDataGraphData);
        gd.ChartData.XAxis = ReadAxis(sd.SaveDataXAxis);
        foreach(var sds in sd.SaveDataSeries) gd.ChartData.Series.Add(ReadSeries(sds));
        return gd;
    }
    internal static SaveData SaveFile(GraphData gd)
    {
        SaveData sd = new();
        sd.SaveDataGraphData = SaveGraphData(gd);
        sd.SaveDataXAxis = SaveAxis(gd.ChartData.XAxis);
        foreach(var s in gd.ChartData.Series) sd.SaveDataSeries.Add(SaveSeries(s));
        return sd;
    }

    private static GraphData ReadGraphData(SaveDataGraphData sdg)
    {
        return new()
        {
            FileReader = new(){DataFilePath = sdg.DataFilePath},
            ChartTitle = sdg.ChartTitle
        };
    }
    private static SaveDataGraphData SaveGraphData(GraphData gd)
    {
        return new()
        {
            DataFilePath = gd.FileReader.DataFilePath,
            ChartTitle = gd.ChartTitle
        };
    }
    private static XAxis ReadAxis(SaveDataXAxis sdx)
    {
        return new()
        {
            Name = sdx.Name,
            Unit = sdx.Unit,
            XMin = sdx.X1,
            XMax = sdx.X2,
            DefXMin = sdx.DefX1,
            DefXMax = sdx.DefX2,
            LastX = sdx.LastX
        };
    }
    private static SaveDataXAxis SaveAxis(XAxis x)
    {
        return new()
        {
            Name = x.Name,
            Unit = x.Unit,
            X1 = x.XMin,
            DefX1 = x.DefXMin,
            X2 = x.XMax,
            DefX2 = x.DefXMax,
            LastX = x.LastX
        };
    }
    private static Series ReadSeries(SaveDataSeries sds)
    {
        return new(sds.Name, sds.Unit, sds.Color)
        {
            XValues = sds.X,
            YValues = sds.Y,
            YMin = sds.Min,
            YMax = sds.Max,
            Precision = sds.Precision,
            YSetMin = sds.SetMin,
            YSetMax = sds.SetMax,
            Interval = sds.Interval,
            Active = sds.Active,
            RegressionFunction = sds.Function
        };
    }
    private static SaveDataSeries SaveSeries(Series s)
    {
        return new()
        {
            Name = s.Name,
            Unit = s.Unit,
            X = s.XValues,
            Y = s.YValues,
            Min = s.YMin,
            Max = s.YMax,
            Precision = s.Precision,
            SetMin = s.YSetMin,
            SetMax = s.YSetMax,
            Interval = s.Interval,
            Active = s.Active,
            Function = s.RegressionFunction,
            Color = s.Color
        };
    }
}