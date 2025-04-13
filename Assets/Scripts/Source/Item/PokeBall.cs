using System;
using System.Collections;
using DG.Tweening;
using Scripts.Utility;
using Scripts.Utility.Math;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Source
{
    [CreateAssetMenu(menuName = "Item/Poke Ball/Create new Poke Ball")]
    public class PokeBall : ItemAsset, IBattleAction
    {
        [SerializeField] private Sprite battleSprite;

        [SerializeField] private GameObject pokeBallSprite;

        private float GetCatchRate(Pokemon wildPokemon, ulong turn)
        {
            switch (name)
            {
                case "Poke Ball":
                    return 1.0f;
                case "Great Ball":
                    return 1.5f;
                case "Ultra Ball":
                    return 2.0f;
                case "Master Ball":
                    return byte.MaxValue;
                case "Net Ball":
                    if (wildPokemon.HasType(Type.ID.Bug) || wildPokemon.HasType(Type.ID.Water))
                    {
                        return 3.5f;
                    }

                    return 1.0f;
                case "Dive Ball":
                    throw new NotImplementedException();
                case "Nest Ball":
                    return Mathf.Max(1.0f, (41 - wildPokemon.Level) * Numerics.UInt12MaxValue / 10.0f / Numerics.UInt12MaxValue);
                case "Repeat Ball":
                    return Pokedex.Instance.IsRegistered(wildPokemon.Asset.name) ? 3.5f : 1.0f;
                case "Timer Ball":
                    return Mathf.Min(1 + turn * (1229 / (float)Numerics.UInt12MaxValue), 4.0f);
                case "Luxury Ball":
                    return 1.0f;
                case "Premier Ball":
                    return 1.0f;
                case "Dusk Ball":
                    var now = DateTime.Now;
                    // 6:00 PM
                    var lower = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
                    // 4:00 AM
                    var upper = new DateTime(now.Year, now.Month, now.Day, 4, 0, 0).AddDays(1);

                    return lower <= now || now < upper ? 3.5f : 1.0f;
                case "Quick Ball":
                    return turn == 1 ? 5.0f : 1.0f;
                default:
                    throw new ArgumentException($"\"{name}\"'s catch rate is not found", nameof(name));
            }
        }

        private int GenerateShakes(Pokemon wildPokemon, ulong turn)
        {
            var a = 3.0f * wildPokemon.MaxHP - 2.0f * wildPokemon.HP;
            a /= 3.0f * wildPokemon.MaxHP;
            a *= Numerics.UInt12MaxValue;
            a *= wildPokemon.Asset.CatchRate;
            a *= GetCatchRate(wildPokemon, turn);

            var statusBonus = wildPokemon.StatusCondition switch
            {
                Sleep or Freeze => 2.0f,
                Paralysis or Poison or BadPoison or Burn => 1.5f,
                _ => 1.0f
            };

            a *= statusBonus;
            if (a >= byte.MaxValue)
            {
                return 4;
            }

            var b = (ushort.MaxValue + 1) / Algebra.NthRoot(a / (byte.MaxValue * Numerics.UInt12MaxValue), 4);

            for (var shakeCount = 0; shakeCount < 3; ++shakeCount)
            {
                if (Random.Range(0, ushort.MaxValue + 1) >= b)
                {
                    return shakeCount;
                }
            }

            return 4;
        }

        IEnumerator IBattleAction.Use(
            BattleSystem battleSystem,
            BattleUnit user,
            BattleUnit opponent,
            BattleDialogueBox battleDialogueBox)
        {
            ((Player)user.Battler).Inventory.UseItem<PokeBall>(this);

            yield return battleDialogueBox.TypeDialogue($"You threw {this.GetIndefiniteArticle()} {this}!");
            var pokeBall = Instantiate(pokeBallSprite, user.transform.position - new Vector3(2.0f, 0.0f), Quaternion.identity);
            if (pokeBall.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.sprite = battleSprite;
            }

            // throw the Poke Ball
            yield return spriteRenderer.transform.DOJump(
                opponent.transform.position + new Vector3(0.0f, 2.0f),
                2.0f,
                1,
                1.0f
            ).WaitForCompletion();
            yield return opponent.PlayCaptureAnimation();

            // apply gravity
            yield return spriteRenderer.transform.DOMoveY(opponent.transform.position.y - 1.5f, 0.5f).WaitForCompletion();

            // perform shakes
            var shakes = GenerateShakes(opponent.Pokemon, battleSystem.Turn);

            for (var i = 0; i < Mathf.Min(shakes, 3); ++i)
            {
                yield return new WaitForSeconds(0.5f);
                yield return pokeBall.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, 10.0f), 1.0f).WaitForCompletion();
            }

            if (shakes == 4)
            {
                // play catch animation
                {
                    var previousColor = spriteRenderer.color;
                    yield return DOTween.Sequence()
                        .Append(spriteRenderer.DOColor(Color.gray, 0.1f))
                        .Append(spriteRenderer.DOColor(previousColor, 0.1f))
                        .WaitForCompletion();
                }

                yield return battleDialogueBox.TypeDialogue($"{opponent.PokemonPrefixName} was caught!");
                yield return spriteRenderer.DOFade(0.0f, 1.5f).WaitForCompletion();

                if (Pokedex.Instance.RegisterPokemon(opponent.Pokemon.ToString()))
                {
                    yield return battleDialogueBox.TypeDialogue($"{opponent.Pokemon} was added to your Pokedex.");
                    // TODO open pokedex UI
                }

                if (user.Battler.Party.Count == Trainer.MaxPartySize)
                {
                    yield return battleDialogueBox.TypeDialogue($"{opponent.Pokemon} was added to your PC.");
                    // TODO implement PC functionality
                }
                else
                {
                    user.Battler.Party.Add(opponent.Pokemon);
                }

                battleSystem.Terminate();
            }
            else
            {
                yield return new WaitForSeconds(1.0f);
                spriteRenderer.DOFade(0.0f, 0.2f);
                yield return opponent.PlayEscapeAnimation();
                yield return battleDialogueBox.TypeDialogue($"{opponent.PokemonPrefixName} failed to be caught!");
            }

            Destroy(pokeBall);
        }
    }
}
