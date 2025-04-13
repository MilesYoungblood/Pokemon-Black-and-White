using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Utility.Math
{
    /// <summary>
    ///     A collection of statistical functions.
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        ///     Calculates a binomial coefficient (n choose k).
        /// </summary>
        /// <param name="n">Number of trials.</param>
        /// <param name="k">Number of successes.</param>
        /// <returns>The binomial coefficient</returns>
        private static long BinomialCoefficient(uint n, uint k)
        {
            return Algebra.Factorial(n) / (Algebra.Factorial(k) * Algebra.Factorial(n - k));
        }

        /// <summary>
        ///     Calculates a binomial probability.
        /// </summary>
        /// <param name="n">Number of trials.</param>
        /// <param name="k">Number of successes.</param>
        /// <param name="p">Probability of success on each trial.</param>
        /// <returns></returns>
        public static float BinomialProbability(uint n, uint k, float p)
        {
            ValidateProbabilityRange(p);
            return BinomialCoefficient(n, k) * Mathf.Pow(p, k) * Mathf.Pow(1 - p, n - k);
        }

        /// <summary>
        ///     Calculates a Bernoulli probability.
        /// </summary>
        /// <param name="p">Probability of success.</param>
        /// <param name="k">Boolean representation for the number of trials (1 or 0).</param>
        /// <returns>The chances of either success or failure.</returns>
        public static float BernoulliProbability(float p, bool k)
        {
            ValidateProbabilityRange(p);
            return k ? p : 1 - p;
        }

        /// <summary>
        ///     Performs a single Bernoulli trial.
        /// </summary>
        /// <param name="p">Probability of success.</param>
        /// <returns>True on success and false on failure.</returns>
        public static bool BernoulliTrial(float p = 0.5f)
        {
            ValidateProbabilityRange(p);
            return Random.value <= p;
        }

        public static float BernoulliProbabilityMassFunction(float p, bool xIs1)
        {
            ValidateProbabilityRange(p);
            var x = xIs1 ? 1 : 0;
            return Mathf.Pow(p, x) * Mathf.Pow(1 - p, 1 - x);
        }

        /// <summary>
        ///     Tests whether p is in range for probability related functions.
        /// </summary>
        /// <param name="p">Probability.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of p is not between 0 and 1 (inclusive).</exception>
        private static void ValidateProbabilityRange(float p)
        {
            if (p is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "Invalid argument value: must be between 0 and 1");
            }
        }
    }
}