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

    internal RegressionFunction PolynomialRegression(int order) => RegressionFunction.NaF;
    internal RegressionFunction LinearRegression() => RegressionFunction.NaF;
    internal RegressionFunction PowerRegression() => RegressionFunction.NaF;
    internal RegressionFunction LogarithmicRegression() => RegressionFunction.NaF;
    internal RegressionFunction ExponentialRegression() => RegressionFunction.NaF;
}