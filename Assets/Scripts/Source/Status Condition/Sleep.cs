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

        public override void HandlePreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            if (Counter == 0)
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
    }
}
