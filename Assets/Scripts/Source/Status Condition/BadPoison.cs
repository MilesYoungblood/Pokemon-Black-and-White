namespace Scripts.Source
{
    public sealed class BadPoison : StatusCondition
    {
        public override string InflictMessage => "was badly poisoned!";

        public override string CureMessage => "recovered from poisoning";

        public override ID GetID()
        {
            return ID.BadPoison;
        }

        public override void HandlePreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            message = string.Empty;
            canMove = true;
        }
    }
}