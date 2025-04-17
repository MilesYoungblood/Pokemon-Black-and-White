using System.Collections;

namespace Scripts.Source
{
    public sealed class RunTurnState : IBattleState
    {
        private readonly IBattleAction playerAction;

        private readonly IBattleAction opponentAction;

        public RunTurnState(IBattleAction playerAction, IBattleAction opponentAction)
        {
            this.playerAction = playerAction;
            this.opponentAction = opponentAction;
        }

        public IEnumerator Enter(BattleSystem battle)
        {
            battle.ActionSelector.SetActive(false);

            var playerFirst = playerAction.Priority >= opponentAction.Priority;

            var firstAction = playerFirst ? playerAction : opponentAction;
            var secondAction = playerFirst ? opponentAction : playerAction;

            var firstUnit = playerFirst ? battle.PlayerUnit : battle.OpponentUnit;
            var secondUnit = playerFirst ? battle.OpponentUnit : battle.PlayerUnit;

            yield return firstAction.Use(battle, firstUnit, secondUnit, battle.BattleDialogueBox);

            if (secondUnit.Battler.CanFight)
            {
                yield return secondAction.Use(battle, secondUnit, firstUnit, battle.BattleDialogueBox);
            }

            battle.BattleState = new EndTurnState(firstUnit, secondUnit);
        }
    }
}