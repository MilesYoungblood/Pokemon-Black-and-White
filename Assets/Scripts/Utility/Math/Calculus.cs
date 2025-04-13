using System;

namespace Scripts.Utility.Math
{
    /// <summary>
    ///     A collection of calculus functions.
    /// </summary>
    public class Calculus
    {
        /// <summary>
        ///     Calculates the numerical derivative of a function
        /// </summary>
        /// <param name="f">The function to differentiate.</param>
        /// <param name="x">The points at which to differentiate.</param>
        /// <param name="h">The small step size for the finite difference approximation.</param>
        /// <returns></returns>
        public static float NumericalDerivative(Func<float[], float> f, float[] x,
            float h = Numerics.ConvergenceTolerance)
        {
            // Create an array to store the modified x values
            var xH = (float[])x.Clone();

            // We'll calculate the partial derivative with respect to the first variable as an example
            xH[0] += h;

            // Calculate the difference quotient to approximate the derivative
            return (f(xH) - f(x)) / h;
        }

        /// <summary>
        ///     Approximates the numerical indefinite integral of a function
        /// </summary>
        /// <param name="func">The function to integrate.</param>
        /// <param name="a">Lower bound parameters.</param>
        /// <param name="n">The number of intervals for the approximation.</param>
        /// <returns></returns>
        public static Func<float[], float> NumericalAntiderivative(Func<float[], float> func, float[] a, int n = 1000)
        {
            return args => NumericalIntegral(func, a, args, n);
        }

        /// <summary>
        ///     Calculates the numerical definite integral of a function
        /// </summary>
        /// <param name="f">The function to integrate.</param>
        /// <param name="a">Lower bound parameters.</param>
        /// <param name="b">Upper bound parameters.</param>
        /// <param name="n"></param>
        /// <returns>The number of intervals for the approximation.</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static float NumericalIntegral(Func<float[], float> f, float[] a, float[] b, int n = 1000)
        {
            var h = (b[0] - a[0]) / n;
            var sum = 0.5f * (f(a) + f(b));

            for (var i = 1; i < n; ++i)
            {
                var xI = new float[a.Length];
                for (var j = 0; j < a.Length; ++j)
                {
                    xI[j] = a[j] + i * h;
                }

                sum += f(xI);
            }

            return sum * h;
        }
    }
}
