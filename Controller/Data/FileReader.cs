using System;
using System.Collections.Generic;
using System.IO;

namespace PolyAxisGraphs.Data;

internal class FileReader
{
    internal string DataFilePath { get; set; }

    internal FileReader()
    {
        DataFilePath = "";
    }

    internal void SetFilePath(string file)
    {
        DataFilePath = file;
    }

    internal void ReadData(ChartData cd)
    {
        bool exists = File.Exists(DataFilePath);
        var extension = Path.GetExtension(DataFilePath);
        if(exists && extension == ".txt") ReadDataFile(' ', cd);
        else if(exists && extension == ".csv") ReadDataFile(';', cd);
    }

    private void ReadDataFile(char separator, ChartData cd)
    {
        cd.Reset();

        int count = 0;
        foreach(var line in File.ReadLines(DataFilePath)) if(count++ == 0) ReadNames(line, separator, cd); else ReadValues(line, separator, cd);

        var minmax = FindMinimumAndMaximum(cd.Series[0].XValues);
        cd.XAxis.DefXMin = minmax.min;
        cd.XAxis.DefXMax = minmax.max;
        cd.XAxis.XMax = cd.XAxis.DefXMax;
        cd.XAxis.XMin = cd.XAxis.DefXMin;
    }
    private void ReadNames(string line, char separator, ChartData cd)
    {
        char[] separators = {separator};
        string[] namesandunits = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        int count = 0;
        foreach(var nameandunit in namesandunits)
        {
            string[] nu = nameandunit.Split(new char[]{'[', ']'}, StringSplitOptions.RemoveEmptyEntries);
            if(nu.Length < 1) continue;
            string name = nu[0];
            string unit = nu.Length < 2 ? nu[0] : nu[1];
            if(count == 0) 
            {
                cd.XAxis.Name = name;
                cd.XAxis.Unit = unit;
            }
            else
            {
                int scount = count - 1;
                if(scount < Settings.Colors.Length - 1) cd.Series.Add(new(name, unit, Settings.Colors[scount])); else cd.Series.Add(new(name, unit, Avalonia.Media.Colors.Gray));
            }
            count++;
        }
    }
    private void ReadValues(string line, char separator, ChartData cd)
    {
        char[] separators = {separator};
        string[] values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        double xvalue = CustomConvert.StringToDouble(values[0]);
        if(xvalue > cd.XAxis.LastX)
        {
            for(int i = 1; i < values.Length; i++) cd.Series[i - 1].Add(xvalue, CustomConvert.StringToDouble(values[i]));
            cd.XAxis.LastX = xvalue;
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