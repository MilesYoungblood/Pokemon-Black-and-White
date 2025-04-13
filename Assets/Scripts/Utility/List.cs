using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Utility
{
    public static class List
    {
        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable as ICollection<T> ?? enumerable.ToArray()).RandomElement();
        }

        public static T RandomElement<T>(this ICollection<T> collection)
        {
            return collection.ElementAt(Random.Range(0, collection.Count));
        }
    }
}
