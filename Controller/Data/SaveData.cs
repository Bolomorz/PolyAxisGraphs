using System.Collections.Generic;
using Avalonia.Media;
using System.Runtime.Serialization;

namespace PolyAxisGraphs.Data;

[DataContract(Name = "sds", IsReference = true)]
internal struct SaveDataSeries
{
    [DataMember] internal string Name { get; set; }
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

[DataContract(Name = "sdg", IsReference = true)]
internal struct SaveDataGraphData
{
    [DataMember] internal string XAxisName { get; set; }
    [DataMember] internal string DataFilePath { get; set; }
    [DataMember] internal string ChartTitle { get; set; }
    [DataMember] internal double LastX { get; set; }
    [DataMember] internal int X1 { get; set; }
    [DataMember] internal int X2 { get; set; }
    [DataMember] internal int DefX1 { get; set; }
    [DataMember] internal int DefX2 { get; set; }
}

[DataContract(Name = "sd", IsReference = true)]
internal class SaveData
{
    [DataMember] List<SaveDataSeries> SaveDataSeries { get; set; }
    [DataMember] SaveDataGraphData SaveDataGraphData { get; set; }
    internal SaveData()
    {
        SaveDataSeries = new();
        SaveDataGraphData = new();
    }

    internal static GraphData LoadFile(SaveData sd)
    {
        GraphData gd = ReadGraphData(sd.SaveDataGraphData);
        foreach(var sds in sd.SaveDataSeries) gd.Series.Add(ReadSeries(sds));
        return gd;
    }
    internal static SaveData SaveFile(GraphData gd)
    {
        SaveData sd = new();
        sd.SaveDataGraphData = SaveGraphData(gd);
        foreach(var s in gd.Series) sd.SaveDataSeries.Add(SaveSeries(s));
        return sd;
    }

    private static GraphData ReadGraphData(SaveDataGraphData sdg)
    {
        return new()
        {
            XAxisName = sdg.XAxisName,
            DataFilePath = sdg.DataFilePath,
            ChartTitle = sdg.ChartTitle,
            LastX = sdg.LastX,
            XMin = sdg.X1,
            XMax = sdg.X2,
            DefXMin = sdg.DefX1,
            DefXMax = sdg.DefX2
        };
    }
    private static SaveDataGraphData SaveGraphData(GraphData gd)
    {
        return new()
        {
            XAxisName = gd.XAxisName,
            DataFilePath = gd.DataFilePath,
            ChartTitle = gd.ChartTitle,
            LastX = gd.LastX,
            X1 = gd.XMin,
            X2 = gd.XMax,
            DefX1 = gd.DefXMin,
            DefX2 = gd.DefXMax
        };
    }
    private static Series ReadSeries(SaveDataSeries sds)
    {
        return new(sds.Name, sds.Color)
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