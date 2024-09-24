using System;
using System.Collections.Generic;

namespace PolyAxisGraphs.FileGenerator;

internal class Series
{
    internal List<double> Values { get; set; }
    internal int Min { get; set; }
    internal int Max { get; set; }
    internal double Last { get; set; }
    internal bool Direction { get; set; }
    internal string Name { get; set; }
    internal string Unit { get; set;}
    private Random Random { get; set; }

    internal Series(int min, int max, string name, string unit, bool startatmin)
    {
        Min = min;
        Max = max;
        Name = name;
        Unit = unit;
        Values = new();
        Random = new();
        Direction = true;
        Last = startatmin ? Min : StartValue();
        Values.Add(Last);
    }

    internal bool AddNextX()
    {
        double rand = NextRandomX();
        double next = Last;

        next += rand;
        
        Last = next;
        if(next < Max) Values.Add(next);
        return next < Max;
    }

    internal void AddNextY()
    {
        double rand = NextRandomY();
        double next = Last;
        if(Direction)
        {
            next += rand;
            if(next > Max)
            {
                next -= rand * 2;
                Direction = false;
            }
        }
        else
        {
            next -= rand;
            if(next < Min)
            {
                next += rand * 2;
                Direction = true;
            }
        }
        next = Math.Round(next, 5);
        Values.Add(next);
        Last = next;
    }

    private double NextRandomY() => (double)Random.Next(int.MaxValue / 100, int.MaxValue) / (double)int.MaxValue;
    private double NextRandomX() => (double)Random.Next(0, 100) / 1000.0;
    private double StartValue() => Random.Next(Min, Max - (Max - Min)/2);
}