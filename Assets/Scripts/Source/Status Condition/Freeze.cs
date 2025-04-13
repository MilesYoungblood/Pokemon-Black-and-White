namespace Scripts.Source
{
    public sealed class Freeze : StatusCondition
    {
        public override string InflictMessage => "became frozen solid!";

        public override string CureMessage => "thawed out!";

        public override ID GetID()
        {
            return ID.Freeze;
        }

        public override void HandlePreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            if (Utility.Math.Statistics.BernoulliTrial(0.2f))
            {
                unit.Pokemon.StatusCondition = null;
                message = CureMessage;
                canMove = true;
            }
            else
            {
                message = "is frozen solid!";
                canMove = false;
            }
        }
    }
}
