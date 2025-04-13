using System.Collections;
using Scripts.Utility.Algorithm;

namespace Scripts.Source
{
    public class SwitchOut : IBattleAction
    {
        private readonly int _index;

        public byte Priority => 3;

        public SwitchOut(int index)
        {
            _index = index;
        }

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
            battleDialogueBox.EnableActionSelector(false);

            var previousPokemon = user.Pokemon;
            if (user.Pokemon.CanFight)
            {
                user.PlayExitAnimation();
                yield return battleDialogueBox.TypeDialogue($"{user.Pokemon}, return!");
            }

            user.Battler.Party.Swap(0, _index);

            yield return user.Init();

            battleDialogueBox.InitMoveSelector(user.Pokemon.MoveSet, battleSystem.OnMoveSelected, battleSystem.OnMoveCancel);
            yield return battleDialogueBox.TypeDialogue($"Go {user.Pokemon}!");

            // This will execute if the party screen was opened because the player's Pokémon fainted
            if (!previousPokemon.CanFight)
            {
                battleSystem.TriggerActionSelection();
            }
            // this will execute if the opponent's Pokémon fainted and the player chose to switch out
            else if (GameController.Instance.PartyScreen.CalledFrom == BattleSystem.State.NextPokemon)
            {
                yield return opponent.Init();
                yield return battleDialogueBox.TypeDialogue($"{opponent.Battler} sent out {opponent.Pokemon}.");
                battleSystem.TriggerActionSelection();
            }

            GameController.Instance.PartyScreen.CalledFrom = null;
        }
    }
}
