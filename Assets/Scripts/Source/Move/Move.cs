using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using RangeInt = Scripts.Utility.RangeInt;

namespace Scripts.Source
{
    [Serializable]
    public sealed class Move : IBattleAction
    {
        public enum Category
        {
            Physical,
            Special,
            Status
        }

        private const int MinPP = 0;

        [SerializeField] private MoveAsset asset;

        private int _pp;

        private int _maxPP;

        public MoveAsset Asset
        {
            get => asset;
            private set => asset = value;
        }

        public int PP
        {
            get => _pp;
            set => _pp = Mathf.Clamp(value, MinPP, MaxPP);
        }

        public int MaxPP
        {
            get => _maxPP;
            set => _maxPP = Mathf.Clamp(value, Asset.PP, Asset.MaxPP);
        }

        public MoveSaveData SaveData => new()
        {
            name = Asset.name,
            maxPp = MaxPP,
            pp = PP
        };

        public byte Priority => 1;

        public Move()
        {
            try
            {
                Asset = MoveAsset.GetBaseByName("Struggle");
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e);
            }

            PP = int.MaxValue;
        }

        public Move(Move move) : this(move.SaveData)
        {
        }

        public Move(MoveSaveData saveData)
        {
            Asset = MoveAsset.GetBaseByName(saveData.name);
            MaxPP = saveData.maxPp;
            PP = saveData.pp;
        }

        public void Init()
        {
            MaxPP = Asset.PP;
            PP = MaxPP;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanUse()
        {
            return PP > MinPP;
        }

        private static bool PreTurnStatus(
            BattleSystem battleSystem,
            BattleDialogueBox battleDialogueBox,
            BattleUnit attacker,
            out List<IEnumerator> enumerators)
        {
            enumerators = new List<IEnumerator>();
            var canMove = false;
            if (attacker.Pokemon.StatusCondition is not null)
            {
                attacker.Pokemon.StatusCondition.PreTurn(attacker, out var message, out canMove);
                if (message is not "")
                {
                    enumerators.Add(battleDialogueBox.TypeDialogue($"{attacker} {message}"));
                }
            }

            if (canMove && attacker.HasVolatileStatusCondition(VolatileStatusCondition.Flinch))
            {
                enumerators.Add(battleDialogueBox.TypeDialogue($"{attacker.PokemonPrefixName} flinched!"));
                canMove = false;
            }

            if (!canMove || !attacker.HasVolatileStatusCondition(VolatileStatusCondition.Confusion))
            {
                return canMove;
            }

            if (attacker.GetVolatileStatusCount(VolatileStatusCondition.Confusion) is 0)
            {
                enumerators.Add(battleDialogueBox.TypeDialogue(
                    $"{attacker.PokemonPrefixName} snapped out of confusion!"
                ));
            }
            else
            {
                enumerators.Add(battleDialogueBox.TypeDialogue($"{attacker.PokemonPrefixName} is confused!"));

                if (Utility.Math.Statistics.BernoulliTrial())
                {
                    return true;
                }

                enumerators.Add(attacker.TakeDamage(
                    battleSystem,
                    battleDialogueBox,
                    Mathf.FloorToInt(IAttack.CalculateDamage(attacker, attacker, Category.Physical, 40)),
                    new List<string> { "It hit itself in confusion" }
                ));

                canMove = false;
            }

            return canMove;
        }

        private bool AccuracyCheck(BattleUnit attacker, BattleUnit defender)
        {
            return Asset.SureHit || UnityEngine.Random.value <= Asset.Accuracy * Mathf.Round(attacker[Stat.Accuracy].Multiplier * defender[Stat.Evasiveness].Multiplier);
        }

        public (int, float, float) PerformCalculations(BattleUnit attacker, BattleUnit defender)
        {
            // calculate core damage
            var damage = IAttack.CalculateDamage(attacker, defender, Asset.Category, Asset.Power);

            // calculate critical hit
            var critical = new RangeInt(1, 17).RandomInt() == 1 ? 2.0f : 1.0f;

            // calculate stab (same type attack bonus)
            var stab = attacker.Pokemon.HasType(Asset.Type) ? 1.5f : 1.0f;

            // calculate type effectiveness
            var typeEff = Type.GetEffectiveness(Asset.Type, defender.Pokemon.Asset.Type1) *
                      Type.GetEffectiveness(Asset.Type, defender.Pokemon.Asset.Type2);

            var burn = Asset.Category is Category.Physical && attacker.Pokemon.StatusCondition is Burn ? 0.5f : 1.0f;

            return (Mathf.FloorToInt(damage * critical * stab * typeEff * burn), typeEff, critical);
        }

        private static List<string> GetDamageDetails(
            BattleUnit defender,
            float typeEff,
            float critical)
        {
            var messages = new List<string>();
            switch (typeEff)
            {
                case Type.NoEffect:
                    messages.Add($"It doesn't affect {defender.PokemonPrefixName}...");
                    break;
                case > Type.RegularEffect:
                    messages.Add("It's super effective!");
                    break;
                case < Type.RegularEffect:
                    messages.Add("It's not very effective...");
                    break;
            }

            if (Mathf.Approximately(critical, 2.0f))
            {
                messages.Add("A critical hit!");
            }

            return messages;
        }

        private IEnumerator ApplyStatEffects(
            BattleDialogueBox battleDialogueBox,
            BattleUnit attacker,
            BattleUnit defender)
        {
            foreach (var statEffect in Asset.StatEffects)
            {
                if (UnityEngine.Random.value > statEffect.Prob)
                {
                    continue;
                }

                var target = statEffect.Self ? attacker : defender;
                if (target[statEffect.Stat].Adjust(statEffect.Amount))
                {
                    yield return battleDialogueBox.TypeDialogue(
                        $"{target.PokemonPrefixName}'s {statEffect.Stat} {(statEffect.Amount > 0 ? "rose" : "fell")}{statEffect.AmountText}!"
                    );
                }
                else
                {
                    yield return battleDialogueBox.TypeDialogue(
                        $"{target.PokemonPrefixName}'s {statEffect.Stat} cannot go any {(statEffect.Amount > 0 ? "higher" : "lower")}!"
                    );
                }
            }
        }

        public bool HandleSamePriority(BattleUnit user, BattleUnit opponent)
        {
            if (Asset.Priority == opponent.CurrentMove.Asset.Priority)
            {
                return IBattleAction.HandleSamePriorityBase(user, opponent);
            }

            if (user.Pokemon.RivalsInSpeed(opponent.Pokemon))
            {
                return Asset.Priority > opponent.CurrentMove.Asset.Priority;
            }

            return user.Pokemon.IsFasterThan(opponent.Pokemon);
        }

        public IEnumerator Use(
            BattleSystem battleSystem,
            BattleUnit attacker,
            BattleUnit defender,
            BattleDialogueBox battleDialogueBox)
        {
            // pre-turn status condition check
            var canMove = PreTurnStatus(
                battleSystem,
                battleDialogueBox,
                attacker,
                out var enumerators
            );
            foreach (var enumerator in enumerators)
            {
                yield return enumerator;
            }

            if (!canMove)
            {
                yield break;
            }

            --PP;
            yield return battleDialogueBox.TypeDialogue($"{attacker.PokemonPrefixName} used {this}!");

            if (Asset.Category is Category.Physical or Category.Special)
            {
                if (AccuracyCheck(attacker, defender))
                {
                    attacker.PlayAttackAnimation();
                    yield return new WaitForSeconds(1.0f);

                    var (damage, typeEff, critical) = PerformCalculations(attacker, defender);
                    yield return defender.TakeDamage(
                        battleSystem,
                        battleDialogueBox,
                        damage,
                        GetDamageDetails(defender, typeEff, critical)
                    );
                    if (!defender.Pokemon.CanFight)
                    {
                        yield break;
                    }
                }
                else
                {
                    yield return battleDialogueBox.TypeDialogue($"{defender.PokemonPrefixName} avoided the attack!");
                    yield break;
                }
            }

            yield return ApplyStatEffects(battleDialogueBox, attacker, defender);

            if (Asset.StatusEffect.StatusCondition is StatusCondition.ID.None)
            {
                yield break;
            }

            // A status condition can only be applied if the Pokémon doesn't currently have one
            if (defender.Pokemon.StatusCondition is null)
            {
                // test for probability
                if (!Utility.Math.Statistics.BernoulliTrial(Asset.StatusEffect.Prob) ||
                    defender.Pokemon.IsImmuneToStatus(Asset.StatusEffect.StatusCondition))
                {
                    yield break;
                }

                defender.Pokemon.StatusCondition = StatusCondition.GetConditionByID(Asset.StatusEffect.StatusCondition);
                if (defender.Pokemon.StatusCondition is not null)
                {
                    yield return battleDialogueBox.TypeDialogue($"{defender} {defender.Pokemon.StatusCondition.InflictMessage}");
                }
            }
            else if (Asset.Category is Category.Status)
            {
                yield return battleDialogueBox.TypeDialogue("But it failed!");
            }
        }

        public override string ToString()
        {
            return Asset.name;
        }
    }
}
