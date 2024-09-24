using System.Runtime.Serialization;
using System;

namespace PolyAxisGraphs.Data;

[DataContract(Name = "ft")]
internal enum FunctionType { [EnumMember]NaF, [EnumMember]Line, [EnumMember]Polynomial, [EnumMember]Logarithm, [EnumMember]Power, [EnumMember]Exponential}

[DataContract(Name = "rf", IsReference = true)]
internal class RegressionFunction
{
    [DataMember] internal required double[] Function { get; set; }
    [DataMember] internal required FunctionType Type { get; set; }
    [DataMember] internal required bool ShowFunction { get; set; }

    internal static RegressionFunction NaF = new(){ Function = new[] { double.NaN }, Type = FunctionType.NaF, ShowFunction = false };

    internal double CalculateYValue(double xvalue)
    {
        switch(Type)
        {
            case FunctionType.Line:
            return Function[0] + Function[1] * xvalue;
            case FunctionType.Polynomial:
            double exp = 0;
            double y = 0;
            foreach(var coeff in Function) y += coeff * Math.Pow(xvalue, exp++);
            return y;
            case FunctionType.Exponential:
            return Function[0] * Math.Exp(Function[1] * xvalue);
            case FunctionType.Logarithm:
            return Function[0] + Function[1] * Math.Log(xvalue);
            case FunctionType.Power:
            return Function[0] * Math.Pow(xvalue, Function[1]);
            default:
            return 0;
        }
    }
}