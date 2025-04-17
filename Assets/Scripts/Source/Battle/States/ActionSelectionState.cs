using System.Collections;

namespace Scripts.Source
{
    public sealed class ActionSelectionState : IBattleState
    {
        public IEnumerator Enter(BattleSystem battleSystem)
        {
            battleSystem.ActionSelector.SetActive(true);
            battleSystem.BattleDialogueBox.Dialogue = $"What will {battleSystem.PlayerUnit.Pokemon} do?";
            yield break;
        }
    }
}
