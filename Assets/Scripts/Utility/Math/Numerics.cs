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

        public const float Sqrt2 = 1.414f;

        /// <summary>
        /// The maximum value for an unsigned 12-bit integer.
        /// </summary>
        public const int UInt12MaxValue = 1 << 12;

        public static ref int ModuloIncrement(this ref int i, int n)
        {
            i = (i + 1) % n;
            return ref i;
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
            return n % 2 is 0 ? 1 : -1;
        }

        public static bool ContainsMixedSigns(ICollection<float> list)
        {
            return !list.All(float.IsNegative) && list.Any(float.IsNegative);
        }
    }
}
