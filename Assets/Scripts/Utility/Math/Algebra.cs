using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Utility.Math
{
    /// <summary>
    ///     A collection of algebra functions.
    /// </summary>
    public static class Algebra
    {
        /// <summary>
        ///     Raises 2 to the power of <paramref name="n" />.
        /// </summary>
        /// <param name="n">The power.</param>
        /// <returns>2 ^ <paramref name="n" />.</returns>
        [Pure]
        public static int TwoToThe(int n)
        {
            return 1 << n;
        }

        /// <summary>
        ///     Raises 2 to the power of <paramref name="n" />.
        /// </summary>
        /// <param name="n">The power.</param>
        /// <returns>2 ^ <paramref name="n" />.</returns>
        [Pure]
        public static uint TwoToThe(uint n)
        {
            if (n >= sizeof(int) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(n), "n must be less than 32 for 32-bit integers.");
            }

            return 1U << (int)n;
        }

        /// <summary>
        ///     Raises 2 to the power of <paramref name="n" />.
        /// </summary>
        /// <param name="n">The power.</param>
        /// <returns>2 ^ <paramref name="n" />.</returns>
        [Pure]
        public static float TwoToThe(float n)
        {
            return Mathf.Pow(2, n);
        }

        /// <summary>
        ///     Raises <paramref name="n" /> to the power of 2.
        /// </summary>
        /// <param name="n">The base.</param>
        /// <returns><paramref name="n" /> squared.</returns>
        [Pure]
        public static int NSquared(int n)
        {
            return n * n;
        }

        /// <summary>
        ///     Raises <paramref name="n" /> to the power of 2.
        /// </summary>
        /// <param name="n">The base.</param>
        /// <returns><paramref name="n" /> squared.</returns>
        [Pure]
        public static float NSquared(float n)
        {
            return n * n;
        }

        /// <summary>
        ///     Raises <paramref name="n" /> to the power of 3.
        /// </summary>
        /// <param name="n">The base.</param>
        /// <returns><paramref name="n" /> cubed.</returns>
        [Pure]
        public static int NCubed(int n)
        {
            return n * n * n;
        }

        /// <summary>
        ///     Raises <paramref name="n" /> to the power of 3.
        /// </summary>
        /// <param name="n">The base.</param>
        /// <returns><paramref name="n" /> cubed.</returns>
        [Pure]
        public static float NCubed(float n)
        {
            return n * n * n;
        }

        /// <summary>
        ///     Extracts the cube root of <paramref name="x" />.
        /// </summary>
        /// <param name="x">The radicand.</param>
        /// <returns>The cube root of <paramref name="x" />.</returns>
        [Pure]
        public static float CubeRoot(float x)
        {
            return NthRoot(x, 3);
        }

        /// <summary>
        ///     Extracts the nth root of <paramref name="x" />.
        /// </summary>
        /// <param name="x">The radicand.</param>
        /// <param name="n">The degree.</param>
        /// <returns>The <paramref name="n" />th root of <paramref name="x" />.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="n" /> is 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="n" /> is even and less than 0.</exception>
        [Pure]
        public static float NthRoot(float x, float n)
        {
            if (Mathf.Approximately(n, 0.0f))
            {
                throw new ArgumentException("The root degree n cannot be zero.", nameof(n));
            }

            if (float.IsNegative(x) && n % 2.0f == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Cannot calculate the even root of a negative number.");
            }

            return Mathf.Pow(x, 1.0f / n);
        }

        /// <summary>
        ///     Calculates the factorial of <paramref name="n" />.
        /// </summary>
        /// <param name="n">A non-negative number.</param>
        /// <returns><paramref name="n" />!.</returns>
        [Pure]
        public static long Factorial(uint n)
        {
            long result = 1;
            for (uint i = 1; i <= n; ++i)
            {
                result *= i;
            }

            return result;
        }

        [Pure]
        public static int IterativeFibonacci(int n)
        {
            var f = new int[n];
            f[0] = 0;
            f[1] = 1;
            for (var i = 0; i < n; ++i)
            {
                f[i] = f[i - 1] + f[i - 2];
            }

            return f[n - 1];
        }
    }
}
