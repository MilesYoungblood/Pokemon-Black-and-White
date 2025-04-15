using System.Collections;

namespace Scripts.Source
{
    public abstract class Condition
    {
        protected int Counter { get; private set; }

        protected Condition() : this(int.MaxValue)
        {
        }

        protected Condition(int counter)
        {
            Counter = counter;
        }

        public abstract void PreTurn(BattleUnit unit, out string message, out bool canMove);

        public abstract IEnumerator PostTurn(
            BattleSystem battleSystem,
            BattleDialogueBox battleDialogueBox,
            BattleUnit target);

        public void Update()
        {
            if (Counter is not int.MaxValue)
            {
                --Counter;
            }
        }
    }
}
