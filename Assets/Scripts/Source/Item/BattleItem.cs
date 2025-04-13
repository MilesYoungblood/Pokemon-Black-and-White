using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [CreateAssetMenu(menuName = "Item/Battle Item/Create new Battle Item")]
    public class BattleItem : ItemAsset, IBattleAction
    {
        [SerializeField] private Stat stat;

        [SerializeField] private int amount;

        IEnumerator IBattleAction.Use(
            BattleSystem battleSystem,
            BattleUnit user,
            BattleUnit opponent,
            BattleDialogueBox battleDialogueBox)
        {
            yield return battleDialogueBox.TypeDialogue($"You used {this.GetIndefiniteArticle()} {this}!");

            if (user.ApplyStatEffect(stat, amount))
            {
                var amplitude = Mathf.Abs(amount) switch
                {
                    2 => " sharply",
                    3 => " drastically",
                    4 => " immensely",
                    _ => string.Empty
                };

                yield return battleDialogueBox.TypeDialogue($"{user.PokemonPrefixName}'s {stat} rose{amplitude}");
            }
            else
            {
                yield return battleDialogueBox.TypeDialogue($"{user.PokemonPrefixName}'s {stat} can't go any higher!");
            }
        }
    }
}
