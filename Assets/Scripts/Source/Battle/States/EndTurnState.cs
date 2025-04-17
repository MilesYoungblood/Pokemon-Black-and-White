using System.Collections;

namespace Scripts.Source
{
    public class EndTurnState : IBattleState
    {
        private readonly BattleUnit _firstUnit;

        private readonly BattleUnit _secondUnit;

        public EndTurnState(BattleUnit firstUnit, BattleUnit secondUnit)
        {
            _firstUnit = firstUnit;
            _secondUnit = secondUnit;
        }

        public IEnumerator Enter(BattleSystem battleSystem)
        {
            yield return battleSystem.PostTurnStatus(_firstUnit);
            yield return battleSystem.PostTurnStatus(_secondUnit);

            ++battleSystem.Turn;
            battleSystem.BattleState = new ActionSelectionState();
        }
    }
}
