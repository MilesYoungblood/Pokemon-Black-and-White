namespace Scripts.Source
{
    public sealed class Poison : StatusCondition
    {
        public override string InflictMessage => "was poisoned!";

        public override string CureMessage => "recovered from poisoning!";

        public override ID GetID()
        {
            return ID.Poison;
        }

        public override void HandlePreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            message = string.Empty;
            canMove = true;
        }
    }
}
