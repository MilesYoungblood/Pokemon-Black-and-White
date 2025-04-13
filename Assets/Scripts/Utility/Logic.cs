using System;
using System.Linq;
using Scripts.Utility.Math;

namespace Scripts.Utility
{
    /// <summary>
    ///     A collection of functions pertaining to predicate logic.
    /// </summary>
    public static class Logic
    {
        /// <summary>
        ///     Tests if a statement is logically valid.
        /// </summary>
        /// <param name="premises">The set of premises.</param>
        /// <param name="statement">The logical statement.</param>
        /// <returns>Whether the statement is logically valid.</returns>
        public static bool IsValid(bool[] premises, Func<bool[], bool> statement)
        {
            return !premises.All(premise => premise) || statement(premises);
        }

        /// <summary>
        ///     Tests if a statement is logically sound.
        /// </summary>
        /// <param name="premises">The set of premises.</param>
        /// <param name="statement">The logical statement.</param>
        /// <returns>Whether the statement is logically sound.</returns>
        public static bool IsSound(bool[] premises, Func<bool[], bool> statement)
        {
            // Check if all premises are true and the statement is true
            return premises.All(premise => premise) && statement(premises);
        }

        /// <summary>
        ///     Generates a truth table for n premises. The truth table is represented as a tuple array of a Boolean array,
        ///     and a Boolean.
        /// </summary>
        /// <param name="n">Number of premises.</param>
        /// <param name="statement">The logical statement.</param>
        /// <returns></returns>
        public static (bool[], bool)[] GenerateTruthTable(uint n, Func<bool[], bool> statement)
        {
            var rows = Algebra.TwoToThe(n);
            var truthTable = new (bool[], bool)[rows];

            // Generate all combinations of truth values for the given number of variables
            for (var i = 0; i < rows; ++i)
            {
                var variables = new bool[n];
                for (var j = 0; j < n; ++j) variables[j] = (i & (1 << j)) != 0;

                // Add the current combination of variables and the result to the truth table
                truthTable[i] = (variables, statement(variables));
            }

            return truthTable;
        }
    }
}
