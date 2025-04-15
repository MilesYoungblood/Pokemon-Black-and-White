using System.Collections;

namespace Scripts.Source
{
    public sealed class RunAway : IBattleAction
    {
        public byte Priority => 4;

        public bool HandleSamePriority(BattleUnit user, BattleUnit opponent)
        {
            return IBattleAction.HandleSamePriorityBase(user, opponent);
        }

        public IEnumerator Use(
            BattleSystem battleSystem,
            BattleUnit user,
            BattleUnit opponent,
            BattleDialogueBox battleDialogueBox)
        {
            var opponentSpeed = opponent.Pokemon.SpeedCalc() / 4 % (byte.MaxValue + 1);
            var odds = opponent.Pokemon.SpeedCalc() * 32 + 30;
            if (opponentSpeed == 0 || odds > byte.MaxValue || UnityEngine.Random.Range(0, byte.MaxValue + 1) < odds)
            {
                yield return battleDialogueBox.TypeDialogue("Got away safely!");
                battleSystem.Terminate();
            }
            else
            {
                yield return battleDialogueBox.TypeDialogue("Couldn't get away!");
            }
        }
    }
}
