using System;
using System.Collections.Generic;

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

internal static class RegressionAlgorithms
{
    /// <summary>
    /// [0] + [1]x = y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static double[]? LinearRegression(List<double> x, List<double> y)
    {
        if(x.Count != y.Count) return null;

        int n = x.Count;

        double sumX = 0;
        double sumXX = 0;
        double sumY = 0;
        double sumXY = 0;

        for(int i = 0; i < n; i++)
        {
            sumX += x[i];
            sumXX += x[i]*x[i];
            sumY += y[i];
            sumXY += x[i]*y[i];
        }

        double b = (n * sumXY - sumX * sumY)/(n * sumXX - sumX * sumX);
        double a = (sumY - b * sumX)/n;

        return [a, b];
    }

    /// <summary>
    /// [0] * x^[1] = y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static double[]? PowerRegression(List<double> x, List<double> y)
    {
        if(x.Count != y.Count) return null;

        int n = x.Count;

        double sumX = 0;
        double sumXX = 0;
        double sumY = 0;
        double sumXY = 0;

        for(int i = 0; i < n; i++)
        {
            sumX += Math.Log(x[i]);
            sumXX += Math.Log(x[i])*Math.Log(x[i]);
            sumY += Math.Log(y[i]);
            sumXY += Math.Log(x[i])*Math.Log(y[i]);
        }

        double b = (n * sumXY - sumX * sumY)/(n * sumXX - sumX * sumX);
        double a = (sumY - b * sumX)/n;

        return [Math.Exp(a), b];
    }

    /// <summary>
    /// [0] * [1]^x = y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static double[]? ExponentialRegression(List<double> x, List<double> y)
    {
        if(x.Count != y.Count) return null;

        int n = x.Count;

        double sumX = 0;
        double sumXX = 0;
        double sumY = 0;
        double sumXY = 0;

        for(int i = 0; i < n; i++)
        {
            sumX += x[i];
            sumXX += x[i]*x[i];
            sumY += Math.Log(y[i]);
            sumXY += x[i]*Math.Log(y[i]);
        }

        double b = (n * sumXY - sumX * sumY)/(n * sumXX - sumX * sumX);
        double a = (sumY - b * sumX)/n;

        return [Math.Exp(a), Math.Exp(b)];
    }

    /// <summary>
    /// [0] + [1] * ln(x) = y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static double[]? LogarithmicRegression(List<double> x, List<double> y)
    {
        if(x.Count != y.Count) return null;

        int n = x.Count;

        double sumX = 0;
        double sumXX = 0;
        double sumY = 0;
        double sumXY = 0;

        for(int i = 0; i < n; i++)
        {
            sumX += Math.Log(x[i]);
            sumXX += Math.Log(x[i])*Math.Log(x[i]);
            sumY += y[i];
            sumXY += Math.Log(x[i])*y[i];
        }

        double b = (n * sumXY - sumX * sumY)/(n * sumXX - sumX * sumX);
        double a = (sumY - b * sumX)/n;

        return [a, b];
    }

    /// <summary>
    /// [0] + [1] * x^1 + ... + [n] * x^n = y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static double[]? PolynomialRegression(int order, List<double> x, List<double> y)
    {
        if(x.Count != y.Count) return null;

        return new AugmentedCoefficientMatrix(order, x, y).GetSolution();
    }

    private class AugmentedCoefficientMatrix
    {
        private double[,] Matrix;
        private int n;
        private int m;
        public AugmentedCoefficientMatrix(int order, List<double> x, List<double> y)
        {
            m = x.Count;
            n = order + 1;
            Matrix = new double[n, n+1];
            for(int i = 1; i <=n; i++)
            {
                double sum;
                for(int j = 1; j <= i; j++)
                {
                    int k = i + j - 2;
                    sum = 0;
                    for(int l = 1; l <= m; l++) sum += Math.Pow(x[l-1], k);
                    Matrix[i-1, j-1] = sum;
                    Matrix[j-1, i-1] = sum;
                }
                sum = 0;
                for(int l = 1; l <= m; l++) sum += y[l-1] * Math.Pow(x[l-1], i-1);
                Matrix[i-1, n] = sum;
            }
        }

        public double[]? GetSolution()
        {
            try
            {
                if(m < n+1) return null;
                ApplyGaussJordanElimination();
                double[] solution = new double[n];
                for(int i = 0; i < n; i++) solution[i] = Matrix[i, n]/Matrix[i, i];
                return solution;
            }
            catch
            {
                return null;
            }
        }

        private void ApplyGaussJordanElimination()
        {
            for(int i = 0; i < n; i++)
            {
                if(Matrix[i, i] == 0) throw new DivideByZeroException();
                for(int j = 0; j < n; j++)
                {
                    if(i != j)
                    {
                        double Ratio = Matrix[j, i] / Matrix[i, i];
                        for(int k = 0; k <= n; k++) Matrix[j, k] -= Ratio * Matrix[i, k];
                    }
                }
            }
        }
    }
}