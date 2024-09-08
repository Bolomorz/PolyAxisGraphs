using System.IO;

namespace PolyAxisGraphs;

internal static class Language
{
    internal static string BtOpenFile = "...";
    internal static string BtSaveFile = "save as picture";
    internal static string BtApply = "apply";
    internal static string BtDiscard = "discard";
    internal static string BtReset = "reset";
    internal static string BtReturn = "return";
    internal static string BtCalc = "calculate";
    internal static string BtFuncActive = "regression function (active)";
    internal static string BtFuncInactive = "regression function (inactive)";
    internal static string TbOpenFile = "open data file";
    internal static string TbEnterTitle = "enter chart title here";
    internal static string TbTitleSettingsX = "settings x axis";
    internal static string TbTitleSettingsY = "settings y series";
    internal static string TbTitleSettingsS = "settings for current function";
    internal static string TbTitleSettingsFunc = "settings regression function for";
    internal static string TbTitleSettingsCurrent = "current function";
    internal static string TbTitleSettingsCreate = "calculate function";
    internal static string TbNameSettingsX = "name of x axis";
    internal static string TbNameSettingsY = "name of y series";
    internal static string TbColorSettingsY = "color of series";
    internal static string TbMinValue = "min value";
    internal static string TbMaxValue = "max value";
    internal static string TbFunction = "function";
    internal static string TbFType = "function type";
    internal static string TbPrecision = "precision (decimal places)";
    internal static string TbOrder = "order";
    internal static string TbSelect = "select type";
    internal static string CbSeries = "show series";
    internal static string CbFunc = "show function";

    internal static void SetLanguageFromFile(string filepath)
    {
        if(File.Exists(filepath) && Path.GetExtension(filepath) == ".lng")
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
    private static void SetValue(string key, string value)
    {
        switch(key)
        {
            case "BtOpenFile": BtOpenFile = value; break;
            case "BtSaveFile": BtSaveFile = value; break;
            case "BtApply": BtApply = value; break;
            case "BtDiscard": BtDiscard = value; break;
            case "BtReset": BtReset = value; break;
            case "BtReturn": BtReturn = value; break;
            case "BtCalc": BtCalc = value; break;
            case "BtFuncActive": BtFuncActive = value; break;
            case "BtFuncInactive": BtFuncInactive = value; break;
            case "TbOpenFile": TbOpenFile = value; break;
            case "TbEnterTitle": TbEnterTitle = value; break;
            case "TbTitleSettingsX": TbTitleSettingsX = value; break;
            case "TbTitleSettingsY": TbTitleSettingsY = value; break;
            case "TbTitleSettingsS": TbTitleSettingsS = value; break;
            case "TbTitleSettingsFunc": TbTitleSettingsFunc = value; break;
            case "TbTitleSettingsCurrent": TbTitleSettingsCurrent = value; break;
            case "TbTitleSettingsCreate": TbTitleSettingsCreate = value; break;
            case "TbNameSettingsX": TbNameSettingsX = value; break;
            case "TbNameSettingsY": TbNameSettingsY = value; break;
            case "TbColorSettingsY": TbColorSettingsY = value; break;
            case "TbMinValue": TbMinValue = value; break;
            case "TbMaxValue": TbMaxValue = value; break;
            case "TbFunction": TbFunction = value; break;
            case "TbFType": TbFType = value; break;
            case "TbPrecision": TbPrecision = value; break;
            case "TbOrder": TbOrder = value; break;
            case "TbSelect": TbSelect = value; break;
            case "CbSeries": CbSeries = value; break;
            case "CbFunc": CbFunc = value; break;
        }
    }
}