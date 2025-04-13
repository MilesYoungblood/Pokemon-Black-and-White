namespace Scripts.Source
{
    public sealed class Burn : StatusCondition
    {
        public override string InflictMessage => "was burned!";

        public override string CureMessage => "was cured of it's burn!";

        public override ID GetID()
        {
            return ID.Burn;
        }

        public override void HandlePreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            message = string.Empty;
            canMove = true;
        }
    }
}
