using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PolyAxisGraphs.Data;

internal class GraphData
{
    internal List<Series> Series { get; set; }
    internal string XAxisName { get; set; }
    internal string ChartTitle { get; set; }
    internal string DataFilePath { get; set; }
    internal double LastX { get; set; }
    internal int XMin { get; set; }
    internal int XMax { get; set; }
    internal int DefXMin { get; set; }
    internal int DefXMax { get; set; }

    internal GraphData()
    {
        Series = new();
        DataFilePath = "";
        LastX = 0;
        XMin = 0;
        XMax = 0;
        DefXMin = 0;
        DefXMax = 0;
        XAxisName = "";
        ChartTitle = "";
    }

    internal void SetChartTitle(string title)
    {
        ChartTitle = title;
    }

    internal void SetLanguage(string file)
    {

    }

    internal void SetFilePath(string file)
    {
        DataFilePath = file;
    }

    internal void ReadData()
    {
        bool exists = File.Exists(DataFilePath);
        var extension = Path.GetExtension(DataFilePath);
        if(exists && extension == ".txt") ReadDataFile(' ');
        else if(exists && extension == ".csv") ReadDataFile(';');
    }

    private void ReadDataFile(char separator)
    {
        Series.Clear();
        LastX = double.MinValue;

        int count = 0;
        foreach(var line in File.ReadLines(DataFilePath))
        {
            if(count == 0) ReadFirstLine(line, separator); else ReadSubLine(line, separator);
            count++;
        }
        var minmax = FindMinimumAndMaximum(Series[0].XValues);
        DefXMin = minmax.min;
        DefXMax = minmax.max;
        XMax = DefXMax;
        XMin = DefXMin;
    }
    private void ReadFirstLine(string line, char separator)
    {
        char[] separators = {separator};
        string[] axisnames = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        int count = 0;
        foreach(var name in axisnames)
        {
            if(count == 0) XAxisName = name;
            else
            {
                int scount = count - 1;
                if(scount < Settings.Colors.Length - 1) Series.Add(new(name, Settings.Colors[scount])); else Series.Add(new(name, Avalonia.Media.Colors.Gray));
            }
            count++;
        }
    }
    private void ReadSubLine(string line, char separator)
    {
        char[] separators = {separator};
        string[] values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        double xvalue = CustomConvert.StringToDouble(values[0]);
        if(xvalue > LastX)
        {
            for(int i = 1; i < values.Length; i++) Series[i - 1].Add(xvalue, CustomConvert.StringToDouble(values[i]));
            LastX = xvalue;
        }
    }
    private (int min, int max) FindMinimumAndMaximum(List<double> values)
    {
        double dmin = double.MaxValue;
        double dmax = double.MinValue;
        foreach(var value in values)
        {
            if(value < dmin) dmin = value;
            if(value > dmax) dmax = value;
        }
        return ((int)Math.Floor(dmin), (int)Math.Ceiling(dmax));
    }
}