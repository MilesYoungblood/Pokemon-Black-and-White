using System;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace Scripts.Utility
{
    /// <summary>
    /// A struct that uses the adapter pattern to convert an <see cref="Action"/> to a <see cref="UnityEvent"/>.
    /// </summary>
    public readonly struct UnityEventAdapter
    {
        private readonly Action _action;

        /// <summary>
        /// Action Constructor.
        /// </summary>
        /// <param name="action"></param>
        public UnityEventAdapter(Action action)
        {
            _action = action;
        }

        [UsedImplicitly]
        public UnityEvent ToUnityEvent()
        {
            var unityEvent = new UnityEvent();
            unityEvent.AddListener(_action.Invoke);
            return unityEvent;
        }

        public static implicit operator UnityEvent(UnityEventAdapter unityEventAdapter)
        {
            return unityEventAdapter.ToUnityEvent();
        }
    }
}
