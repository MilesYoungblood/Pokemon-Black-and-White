namespace Scripts.Source
{
    public sealed class Paralysis : StatusCondition
    {
        public override string InflictMessage => "became paralyzed! It may be unable to move!";

        public override string CureMessage => "was cured from paralysis!";

        public override ID GetID()
        {
            return ID.Paralysis;
        }

        public override void HandlePreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            if (Utility.Math.Statistics.BernoulliTrial(0.25f))
            {
                message = "is paralyzed! It can't move!";
                canMove = false;
            }
            else
            {
                message = string.Empty;
                canMove = true;
            }
        }
    }
}
