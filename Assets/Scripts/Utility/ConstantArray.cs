using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Utility
{
    [Serializable]
    public class ConstantArray<T> : IEnumerable<T>
    {
        [SerializeField] private T[] array;

        public T this[int index] => array[index];

        public ConstantArray(params T[] array)
        {
            this.array = array;
        }

        //[MustDisposeResource]
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)array).GetEnumerator();
        }

        //[MustDisposeResource]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
