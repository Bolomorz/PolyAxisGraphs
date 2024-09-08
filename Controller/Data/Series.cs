using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.FlowAnalysis;
using PolyAxisGraphs.Drawing;

namespace PolyAxisGraphs.Data;

internal class Series
{
    internal List<double> XValues { get; set; }
    internal List<double> YValues { get; set; }
    internal int YMin { get; set; }
    internal int YMax { get; set; }
    internal double YSetMin { get; set; }
    internal double YSetMax { get; set; }
    internal double Interval { get; set; }
    internal string Name { get; set; }
    internal  Avalonia.Media.Color Color { get; set; }
    internal bool Active { get; set; }
    internal RegressionFunction RegressionFunction { get; set; }
    internal int Precision { get; set; }

    internal Series(string name,  Avalonia.Media.Color color)
    {
        Name = name;
        Color = color;
        XValues = new();
        YValues = new();
        YMin = int.MaxValue;
        YMax = int.MinValue;
        Active = true;
        RegressionFunction = RegressionFunction.NaF;
        Precision = 5;
    }

    internal void Add(double x, double y)
    {
        XValues.Add(x);
        YValues.Add(y);
        CompareMax(y);
        CompareMin(y);
        YSetMin = YMin;
        YSetMax = YMax;
        SetInterval();
    }
    internal void SetMax(double max)
    {
        if(max > YSetMin)
        {
            YSetMax = max;
            SetInterval();
        }
    }
    internal void SetMin(double min)
    {
        if(min < YSetMax)
        {
            YSetMin = min;
            SetInterval();
        }
    }
    internal void ResetMax()
    {
        YSetMax = YMax;
        SetInterval();
    }
    internal void ResetMin()
    {
        YSetMin = YMin;
        SetInterval();
    }
    internal FunctionStringCollection GetFunction()
    {
        List<FunctionString> functions = new();
        string str = string.Empty;

        switch(RegressionFunction.Type)
        {
            case FunctionType.Line:
                double l1 = Math.Round(RegressionFunction.Function[0], Precision);
                double l2 = Math.Round(RegressionFunction.Function[1], Precision);
                string lo = "+";
                if(l2 < 0)
                {
                    lo = "-";
                    l2 += -1;
                }
                str = string.Format("y = {0} {1} {2} * x", l1, lo, l2);
                functions.Add(new(){Function = str, SuperScript = false, Color = this.Color});
                break;
            case FunctionType.Exponential:
                double e1 = Math.Round(RegressionFunction.Function[0], Precision);
                double e2 = Math.Round(RegressionFunction.Function[1], Precision);
                str = string.Format("y = {0} * exp({1} * x)", e1, e2);
                functions.Add(new(){Function = str, SuperScript = false, Color = this.Color});
                break;
            case FunctionType.Logarithm:
                double log1 = Math.Round(RegressionFunction.Function[0], Precision);
                double log2 = Math.Round(RegressionFunction.Function[1], Precision);
                string logo = "+";
                if(log2 < 0)
                {
                    logo = "-";
                    log2 *= -1;
                }
                str = string.Format("y = {0} {2} {1} * ln(x)", log1, log2, logo);
                functions.Add(new(){Function = str, SuperScript = false, Color = this.Color});
                break;
            case FunctionType.Polynomial:
                str = string.Format("y = {0}", Math.Round(RegressionFunction.Function[0], Precision));
                functions.Add(new(){Function = str, SuperScript = false, Color = this.Color});
                for(int i = 1; i < RegressionFunction.Function.Length; i++)
                {
                    double p1 = Math.Round(RegressionFunction.Function[i], Precision);
                    string po = " + ";
                    if(p1 < 0)
                    {
                        po = " - ";
                        p1 *= -1;
                    }
                    str = po + p1 + " * x";
                    functions.Add(new(){Function = str, SuperScript = false, Color = this.Color});
                    str = i.ToString();
                    functions.Add(new(){Function = str, SuperScript = true, Color = this.Color});
                }
                break;
            case FunctionType.Power:
                double pow1 = Math.Round(RegressionFunction.Function[0], Precision);
                double pow2 = Math.Round(RegressionFunction.Function[1], Precision);
                str = string.Format("y = {0} * x", pow1);
                functions.Add(new(){Function = str, SuperScript = false, Color = this.Color});
                str = pow2.ToString();
                functions.Add(new(){Function = str, SuperScript = true, Color = this.Color});
                break;
        }
        return new(){FunctionStrings = functions};
    }

    private void SetInterval()
    {
        Interval = (double)(YSetMax - YSetMin) / (double)Settings.ChartGridIntervall;
    }
    private void CompareMax(double value)
    {
        if(value > YMax)
        {
            int val = 1;
            while(value > val) val *= 10;
            if(val == 1) val = 1; else val /= 10;
            YMax = 0;
            while(value > YMax) YMax += val;
        }
    }
    private void CompareMin(double value)
    {
        if(value < YMin)
        {
            int val;
            if(value < 0)
            {
                val = -1;
                while(value < val) val *= 10;
                if(val == -1) val = -1; else val /= 10;
                YMin = 0;
                while(value < YMin) YMin += val;
            }
            else
            {
                val = 1;
                while(value > val) val += 10;
                if(val == 1) val = 1; else val /= 10;
                YMin = 0;
                while(value > YMin + val) YMin += val;
            }
        }

        if(YMin < 0) {if((-1) * YMin < YMax / 10) YMin = (-1) * (YMax / 10);}
        else {if(YMin < YMax / 10) YMin = 0;}
    }
}