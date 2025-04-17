using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Utility
{
    /// <summary>
    /// General state class.
    /// </summary>
    /// <remarks>This class will be used by the <see cref="StateMachine"/> class but it can be used elsewhere.</remarks>
    [Serializable]
    public class State : IState
    {
        [SerializeField] private UnityEvent onStrongEnter;

        [SerializeField] private UnityEvent onWeakExit;

        [SerializeField] private UnityEvent onWeakEnter;

        [SerializeField] private UnityEvent onStrongExit;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onStrongEnter">Called when this is pushed to the <see cref="StateMachine"/>.</param>
        /// <param name="onWeakExit">Called when this leaves the top of the <see cref="StateMachine"/> stack after a new state is pushed.</param>
        /// <param name="onWeakEnter">Called when this returns to the top of the <see cref="StateMachine"/> stack after a previous state is popped.</param>
        /// <param name="onStrongExit">Called when this is popped from the <see cref="StateMachine"/>.</param>
        public State(
            [CanBeNull] Action onStrongEnter,
            [CanBeNull] Action onWeakExit,
            [CanBeNull] Action onWeakEnter,
            [CanBeNull] Action onStrongExit)
        {
            this.onStrongEnter = new UnityEventAdapter(onStrongEnter);
            this.onWeakExit = new UnityEventAdapter(onWeakExit);
            this.onWeakEnter = new UnityEventAdapter(onWeakEnter);
            this.onStrongExit = new UnityEventAdapter(onStrongExit);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~State()
        {
            onStrongEnter?.RemoveAllListeners();
            onWeakExit?.RemoveAllListeners();
            onWeakEnter?.RemoveAllListeners();
            onStrongExit?.RemoveAllListeners();
        }

        /// <summary>
        /// Adds a <see cref="UnityAction"/> to the strong-enter event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void AddStrongEnterListener(UnityAction unityAction)
        {
            onStrongEnter?.AddListener(unityAction);
        }

        /// <summary>
        /// Adds a <see cref="UnityAction"/> to the weak-exit event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void AddWeakExitListener(UnityAction unityAction)
        {
            onWeakExit?.AddListener(unityAction);
        }

        /// <summary>
        /// Adds a <see cref="UnityAction"/> to the weak-enter event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void AddWeakEnterListener(UnityAction unityAction)
        {
            onWeakEnter?.AddListener(unityAction);
        }

        /// <summary>
        /// Adds a <see cref="UnityAction"/> to the strong-exit event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void AddStrongExitListener(UnityAction unityAction)
        {
            onStrongExit?.AddListener(unityAction);
        }

        /// <summary>
        /// Removes a <see cref="UnityAction"/> from the strong-enter event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void RemoveStrongEnterListener(UnityAction unityAction)
        {
            onStrongEnter?.RemoveListener(unityAction);
        }

        /// <summary>
        /// Removes a <see cref="UnityAction"/> from the weak-exit event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void RemoveWeakExitListener(UnityAction unityAction)
        {
            onWeakExit?.RemoveListener(unityAction);
        }

        /// <summary>
        /// Removes a <see cref="UnityAction"/> from the weak-enter event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void RemoveWeakEnterListener(UnityAction unityAction)
        {
            onWeakEnter?.RemoveListener(unityAction);
        }

        /// <summary>
        /// Removes a <see cref="UnityAction"/> from the strong-exit event.
        /// </summary>
        /// <param name="unityAction"></param>
        public void RemoveStrongExitListener(UnityAction unityAction)
        {
            onStrongExit?.RemoveListener(unityAction);
        }

        /// <summary>
        /// Implementation of <see cref="IState.StrongEnter"/>.
        /// </summary>
        public void StrongEnter()
        {
            onStrongEnter?.Invoke();
        }

        /// <summary>
        /// Implementation of <see cref="IState.WeakExit"/>.
        /// </summary>
        public void WeakExit()
        {
            onWeakExit?.Invoke();
        }

        /// <summary>
        /// Implementation of <see cref="IState.WeakEnter"/>.
        /// </summary>
        public void WeakEnter()
        {
            onWeakEnter?.Invoke();
        }

        /// <summary>
        /// Implementation of <see cref="IState.StrongExit"/>.
        /// </summary>
        public void StrongExit()
        {
            onStrongExit?.Invoke();
        }
    }
}
