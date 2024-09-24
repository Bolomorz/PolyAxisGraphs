namespace PolyAxisGraphs.Data;

internal static class CustomConvert
{
    internal static bool IsNumeric(char val)
    {
        if (val == '0' || val == '1' || val == '2' || val == '3' || val == '4' || val == '5' || val == '6' || val == '7' || val == '8' || val == '9') return true;
        return false;
    } 
    public static int StringToInt(string val)
    {
        string newval = "";

        foreach(var c in val) if(IsNumeric(c)) newval += c;

        if (newval != "") return int.Parse(newval); else return 0;
    }
    public static double StringToDouble(string val)
    {
        string newval = "";

        foreach(var c in val)
        {
            if (IsNumeric(c)) newval += c;
            else if(c == ',' || c == '.') newval += System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
        }

        if(newval != "") return double.Parse(newval, System.Globalization.NumberStyles.AllowDecimalPoint);
        else return 0;
    }
}