using System.IO;
using PolyAxisGraphs.Data;
using Avalonia.Media;

namespace PolyAxisGraphs;

internal static class Settings
{
    internal static string InitialDirectory = "DataFiles/";
    internal static int ControlFontSize = 15;
    internal static int ChartFontSize = 10;
    internal static int ChartTitleFontSize = 20;
    internal static int ChartGridIntervall = 20;
    internal static int YAxisWidth = 30;
    internal static Color[] Colors = 
    {  
        Avalonia.Media.Colors.Red, 
        Avalonia.Media.Colors.Blue, 
        Avalonia.Media.Colors.Green, 
        Avalonia.Media.Colors.Orange, 
        Avalonia.Media.Colors.Brown, 
        Avalonia.Media.Colors.DarkCyan, 
        Avalonia.Media.Colors.Turquoise, 
        Avalonia.Media.Colors.Purple, 
        Avalonia.Media.Colors.Yellow, 
        Avalonia.Media.Colors.Black
    };

    internal static void SetSettingsFromFile(string filepath)
    {
        if(File.Exists(filepath))
        {
            foreach(var line in File.ReadAllLines(filepath))
            {
                if(line != "")
                {
                    if(line[0] != '#' && line.Contains("="))
                    {
                        string[] pair = line.Split('=');
                        if(pair.Length == 2) SetValue(pair[0], pair[1]);
                    }
                }
            }
        }
    }
    internal static void SaveSettingsToFile(string filepath)
    {
        if(File.Exists(filepath)) File.Delete(filepath);
        using (StreamWriter sw = new(filepath, append: true))
        {
            sw.WriteLine("#Settings for PolyAxisGraphs");
            sw.WriteLine("#initial directory for input and output files");
            sw.WriteLine(string.Format("InitialDirectory=", InitialDirectory));
            sw.WriteLine("#settings for chart drawing");
            sw.WriteLine(string.Format("ControlFontSize=", ControlFontSize));
            sw.WriteLine(string.Format("ChartFontSize=", ChartFontSize));
            sw.WriteLine(string.Format("ChartTitleFontSize=", ChartTitleFontSize));
            sw.WriteLine(string.Format("ChartGridIntervall=", ChartGridIntervall));
            sw.WriteLine(string.Format("YAxisWidth=", YAxisWidth));
        }
    }
    private static void SetValue(string key, string value)
    {
        switch(key)
        {
            case "InitialDirectory": InitialDirectory = value; break;
            case "ControlFontSize": ControlFontSize = CustomConvert.StringToInt(value); break;
            case "ChartFontSize": ChartFontSize = CustomConvert.StringToInt(value); break;
            case "ChartTitleFontSize": ChartTitleFontSize =CustomConvert.StringToInt(value); break;
            case "ChartGridIntervall": ChartGridIntervall = CustomConvert.StringToInt(value); break;
            case "YAxisWidth": YAxisWidth = CustomConvert.StringToInt(value); break;
        }
    }
}