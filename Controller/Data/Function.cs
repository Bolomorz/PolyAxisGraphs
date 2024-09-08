using System.Runtime.Serialization;

namespace PolyAxisGraphs.Data;

[DataContract(Name = "ft")]
internal enum FunctionType { [EnumMember]NaF, [EnumMember]Line, [EnumMember]Polynomial, [EnumMember]Logarithm, [EnumMember]Power, [EnumMember]Exponential}

[DataContract(Name = "rf", IsReference = true)]
internal class RegressionFunction
{
    [DataMember]
    internal required double[] Function { get; set; }
    [DataMember]
    internal required FunctionType Type { get; set; }
    [DataMember]
    internal required bool ShowFunction { get; set; }

    internal static RegressionFunction NaF = new(){ Function = new[] { double.NaN }, Type = FunctionType.NaF, ShowFunction = false };
}