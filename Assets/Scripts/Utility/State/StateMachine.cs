using System.Collections.Generic;
using JetBrains.Annotations;

namespace Scripts.Utility
{
    [UsedImplicitly]
    public class StateMachine
    {
        private readonly Stack<IState> _states = new();

        [UsedImplicitly]
        public void Push(IState state)
        {
            if (_states.TryPeek(out var current))
            {
                current.WeakExit();
            }

            state.StrongEnter();
            _states.Push(state);
        }

        [UsedImplicitly]
        public void Pop()
        {
            if (_states.TryPop(out var previous))
            {
                previous.StrongExit();
            }

            if (_states.TryPeek(out var current))
            {
                current.WeakEnter();
            }
        }
    }
}
