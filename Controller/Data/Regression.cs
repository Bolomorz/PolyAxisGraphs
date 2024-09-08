using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PolyAxisGraphs.Data;

internal class Regression
{
    protected List<double> XValues { get; set; }
    protected List<double> YValues { get; set;}

    internal Regression(List<double> xvalues, List<double> yvalues)
    {
        XValues = xvalues;
        YValues = yvalues;
    }

    internal double[] PolynomialRegression(int order) => RegressionFunction.NaF.Function;
    internal double[] LinearRegression() => RegressionFunction.NaF.Function;
    internal double[] PowerRegression() => RegressionFunction.NaF.Function;
    internal double[] LogarithmicRegression() => RegressionFunction.NaF.Function;
    internal double[] ExponentialRegression() => RegressionFunction.NaF.Function;
}