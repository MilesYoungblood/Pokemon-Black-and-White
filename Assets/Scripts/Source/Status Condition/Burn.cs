using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public override void PreTurn(BattleUnit unit, out string message, out bool canMove)
        {
            message = string.Empty;
            canMove = true;
        }

        public override IEnumerator PostTurn(
            BattleSystem battleSystem,
            BattleDialogueBox battleDialogueBox,
            BattleUnit target)
        {
            yield return target.TakeDamage(
                battleSystem,
                battleDialogueBox,
                Mathf.RoundToInt(target.Pokemon.MaxHP / 8.0f),
                new List<string> { $"{target.PokemonPrefixName} was hurt by it's burn!" }
            );
        }
    }
}
