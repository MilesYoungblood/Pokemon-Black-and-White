using System.Collections;
using Scripts.Utility;

namespace Scripts.Source
{
    public sealed class Sleep : StatusCondition
    {
        public override string InflictMessage => " fell asleep!";

        public override string CureMessage => "woke up!";

        public Sleep() : base(new RangeInt(1, 4).RandomInt())
        {
        }

        public override ID GetID()
        {
            return ID.Sleep;
        }

        public override void PreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            if (Counter is 0)
            {
                unit.Pokemon.StatusCondition = null;
                message = CureMessage;
                canMove = true;
            }
            else
            {
                message = "is fast asleep.";
                canMove = false;
            }
        }

        public override IEnumerator PostTurn(
            BattleSystem battleSystem,
            BattleDialogueBox battleDialogueBox,
            BattleUnit target)
        {
            yield break;
        }
    }
}
