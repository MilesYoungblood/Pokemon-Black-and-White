using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Utility.Math
{
    /// <summary>
    /// A collection of numerical data.
    /// </summary>
    public static class Numerics
    {
        public const float ConvergenceTolerance = 1e-6f;

        public static float Sqrt2 => Mathf.Sqrt(2.0f);

        /// <summary>
        /// The maximum value for an unsigned 12-bit integer.
        /// </summary>
        public static readonly int UInt12MaxValue = Algebra.TwoToThe(12);

        public static void ModuloIncrement(this ref int i, int n)
        {
            i = (i + 1) % n;
        }

        public static void ModuloDecrement(this ref int i, int n)
        {
            i = (i - 1 + n) % n;
        }

        public static bool IsZero(float x)
        {
            return Mathf.Approximately(x, 0);
        }

        /// <summary>
        /// Gets either 1 or -1 based on n's parity.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>1 if n is even, and -1 if n is odd</returns>
        /// <remarks>Another way to get this is (-1)^n, but this is computationally more efficient.</remarks>
        public static int GetSignByParity(int n)
        {
            return n % 2 == 0 ? 1 : -1;
        }

        public static bool ContainsMixedSigns(ICollection<float> list)
        {
            return !list.All(float.IsNegative) && list.Any(float.IsNegative);
        }
    }
}
