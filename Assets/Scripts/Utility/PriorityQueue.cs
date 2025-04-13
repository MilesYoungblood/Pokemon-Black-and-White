using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility.Algorithm;

namespace Scripts.Utility
{
    public class PriorityQueue<T> : IEnumerable<T>
    {
        private readonly List<(T item, float priority)> _heap = new();

        public void Enqueue(T item, float priority)
        {
            _heap.Add((item, priority));
            HeapifyUp(_heap.Count - 1);
        }

        public T Dequeue()
        {
            if (!_heap.Any()) throw new InvalidOperationException("Priority queue is empty");

            var minItem = _heap[0].item;
            _heap[0] = _heap[^1];
            _heap.RemoveAt(_heap.Count - 1);
            HeapifyDown(0);
            return minItem;
        }

        public bool Remove(T item)
        {
            int index = _heap.FindIndex(x => EqualityComparer<T>.Default.Equals(x.item, item));
            if (index == -1) return false;

            _heap.Swap(index, _heap.Count - 1);
            _heap.RemoveAt(_heap.Count - 1);
            if (index < _heap.Count)
            {
                HeapifyDown(index);
                HeapifyUp(index);
            }

            return true;
        }

        public void UpdatePriority(T item, float newPriority)
        {
            if (Remove(item))
            {
                Enqueue(item, newPriority);
            }
        }

        private void HeapifyDown(int i)
        {
            while (true)
            {
                var left = 2 * i + 1;
                var right = 2 * i + 2;
                var smallest = i;
                if (CanSet(left, smallest))
                {
                    smallest = left;
                }

                if (CanSet(right, smallest))
                {
                    smallest = right;
                }

                if (smallest == i)
                {
                    return;
                }

                _heap.Swap(i, smallest);
                i = smallest;
            }
        }

        private void HeapifyUp(int i)
        {
            while (i > 0 && _heap[i].priority < _heap[(i - 1) / 2].priority)
            {
                var parent = (i - 1) / 2;
                _heap.Swap(i, parent);
                i = parent;
            }
        }

        private bool CanSet(int i, int smallest)
        {
            return i < _heap.Count && _heap[i].priority < _heap[smallest].priority;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _heap.Select(value => value.item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}