using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Scripts.Utility
{
    public static class Functional
    {
        public delegate void ActionRef<T>(ref T value);

        public static Coroutine[] InvokeStartCoroutine(this MonoBehaviour behaviour, Func<IEnumerator> eventHandler)
        {
            if (eventHandler == null)
            {
                return Array.Empty<Coroutine>();
            }

            var events = eventHandler.GetInvocationList().Cast<Func<IEnumerator>>().ToArray();
            var coroutines = new Coroutine[events.Length];
            for (var i = 0; i < coroutines.Length; ++i)
            {
                coroutines[i] = behaviour.StartCoroutine(events[i]());
            }

            return coroutines;
        }

        public static Coroutine[] InvokeStartCoroutine<T>(this MonoBehaviour behaviour, Func<T, IEnumerator> eventHandler, T parameters)
        {
            if (eventHandler == null)
            {
                return Array.Empty<Coroutine>();
            }

            var events = eventHandler.GetInvocationList().Cast<Func<T, IEnumerator>>().ToArray();
            var coroutines = new Coroutine[events.Length];
            for (var i = 0; i < coroutines.Length; ++i)
            {
                coroutines[i] = behaviour.StartCoroutine(events[i](parameters));
            }

            return coroutines;
        }

        public static IEnumerator YieldInvoke<T>(this Func<T, IEnumerator> eventHandler, T parameters)
        {
            if (eventHandler == null)
            {
                yield break;
            }

            foreach (var coroutine in eventHandler.GetInvocationList().Cast<Func<T, IEnumerator>>())
            {
                yield return coroutine(parameters);
            }
        }

        public static IEnumerator YieldInvoke(this Func<IEnumerator> eventHandler)
        {
            if (eventHandler == null)
            {
                yield break;
            }

            foreach (var coroutine in eventHandler.GetInvocationList().Cast<Func<IEnumerator>>())
            {
                yield return coroutine();
            }
        }

        public static IEnumerator WaitUntilIfNot(Func<bool> predicate)
        {
            if (!predicate())
            {
                yield return new WaitUntil(predicate);
            }
        }

        public static IEnumerator WaitUntilThenCall(Func<bool> predicate, Action uponTrue)
        {
            yield return new WaitUntil(predicate);
            uponTrue?.Invoke();
        }

        public static IEnumerator WaitUntilTimeout(Func<bool> condition, float timeout)
        {
            var startTime = Time.time;

            while (!condition() && Time.time - startTime < timeout)
            {
                yield return null;
            }
        }
    }
}
