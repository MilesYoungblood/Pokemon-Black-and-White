using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Utility.Algorithm
{
    public static class Sorting
    {
        public static void Swap<T>(this IList<T> list, int a, int b)
        {
            (list[a], list[b]) = (list[b], list[a]);
        }

        public static void MergeSort<T>(this IList<T> list) where T : IComparable<T>
        {
            list.MergeSort((x, y) => x.CompareTo(y));
        }

        public static void MergeSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list.Count < 2)
            {
                return;
            }

            var midPoint = list.Count / 2;
            var left = list.Take(midPoint).ToArray();
            var right = list.Skip(midPoint).ToArray();

            left.MergeSort(comparison);
            right.MergeSort(comparison);

            list.Merge(left, right, comparison);
        }

        private static void Merge<T>(
            this IList<T> targetList,
            IReadOnlyList<T> leftList,
            IReadOnlyList<T> rightList,
            Comparison<T> comparison)
        {
            var leftIndex = 0;
            var rightIndex = 0;
            var targetIndex = 0;

            while (leftIndex < leftList.Count && rightIndex < rightList.Count)
            {
                if (comparison(leftList[leftIndex], rightList[rightIndex]) < 0)
                {
                    targetList[targetIndex++] = leftList[leftIndex++];
                }
                else
                {
                    targetList[targetIndex++] = rightList[rightIndex++];
                }
            }

            while (leftIndex < leftList.Count)
            {
                targetList[targetIndex++] = leftList[leftIndex++];
            }

            while (rightIndex < rightList.Count)
            {
                targetList[targetIndex++] = rightList[rightIndex++];
            }
        }
    }
}
