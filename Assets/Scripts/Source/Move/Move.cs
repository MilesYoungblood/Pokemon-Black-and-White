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

        private int _maxPp;

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
            get => _maxPp;
            set => _maxPp = Mathf.Clamp(value, Asset.PP, Asset.MaxPP);
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

        private static bool HandlePreTurnStatus(
            BattleDialogueBox battleDialogueBox,
            BattleUnit attacker,
            out List<IEnumerator> enumerators)
        {
            enumerators = new List<IEnumerator>();
            var canMove = false;
            if (attacker.Pokemon.StatusCondition != null)
            {
                attacker.Pokemon.StatusCondition.HandlePreTurn(attacker, out var message, out canMove);
                if (message != string.Empty)
                {
                    enumerators.Add(battleDialogueBox.TypeDialogue($"{attacker} {message}"));
                }
            }

            if (canMove && attacker.HasVolatileStatusCondition(VolatileStatusCondition.Flinch))
            {
                enumerators.Add(battleDialogueBox.TypeDialogue($"{attacker.PokemonPrefixName} flinched!"));
                canMove = false;
            }

            if (canMove && attacker.HasVolatileStatusCondition(VolatileStatusCondition.Confusion))
            {
                if (attacker.GetVolatileStatusCount(VolatileStatusCondition.Confusion) == 0)
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
                        var damage = (2 * attacker.Pokemon.Level / 5.0f + 2) * 40;
                        damage *= attacker.Pokemon.Attack * attacker.GetStatModCalc(Stat.Attack) /
                            attacker.Pokemon.Defense * attacker.GetStatModCalc(Stat.Defense);
                        damage /= 50.0f;
                        damage += 2;
                        attacker.Pokemon.HP -= Mathf.FloorToInt(damage);

                        attacker.PlayHitAnimation();
                        enumerators.Add(attacker.PokemonHud.UpdateHp());
                        enumerators.Add(battleDialogueBox.TypeDialogue("It hit itself in confusion"));

                        if (!attacker.Pokemon.CanFight)
                        {
                            //_currentState = State.EndOfTurn;
                            //yield return HandleFaint(attacker);
                        }

                        canMove = false;
                    }
                }
            }

            return canMove;
        }

        private bool AccuracyCheck(BattleUnit attacker, BattleUnit defender)
        {
            if (Asset.SureHit)
            {
                return true;
            }

            return UnityEngine.Random.value <= Asset.Accuracy * Mathf.Round(attacker.GetStatModCalc(Stat.Accuracy) * defender.GetStatModCalc(Stat.Evasiveness));
        }

        public void HandleAttack(BattleUnit attacker, BattleUnit defender, out float typeEff, out float critical)
        {
            var burn = 1.0f;

            // calculate core damage
            var damage = (2 * attacker.Pokemon.Level / 5.0f + 2) * Asset.Power;
            if (Asset.Category == Category.Physical)
            {
                damage *= attacker.Pokemon.Attack * attacker.GetStatModCalc(Stat.Attack) /
                    defender.Pokemon.Defense * defender.GetStatModCalc(Stat.Defense);
                if (attacker.Pokemon.StatusCondition is Burn)
                {
                    burn = 0.5f;
                }
            }
            else
            {
                damage *= attacker.Pokemon.SpAttack * attacker.GetStatModCalc(Stat.SpAttack) /
                    defender.Pokemon.SpDefense * defender.GetStatModCalc(Stat.SpDefense);
            }

            damage /= 50.0f;
            damage += 2;

            // calculate critical hit
            critical = new RangeInt(1, 17).RandomInt() == 1 ? 2.0f : 1.0f;

            // calculate stab (same type attack bonus)
            var stab = Asset.Type == attacker.Pokemon.Asset.Type1 || Asset.Type == attacker.Pokemon.Asset.Type2 ? 1.5f : 1.0f;

            // calculate type effectiveness
            var type1Eff = Type.GetEffectiveness(Asset.Type, defender.Pokemon.Asset.Type1);
            var type2Eff = Type.GetEffectiveness(Asset.Type, defender.Pokemon.Asset.Type2);
            typeEff = type1Eff * type2Eff;

            defender.Pokemon.HP -= Mathf.FloorToInt(damage * critical * stab * typeEff * burn);
        }

        private static IEnumerator TypeDamageDetails(
            BattleDialogueBox battleDialogueBox,
            BattleUnit defender,
            float typeEff,
            float critical)
        {
            switch (typeEff)
            {
                case Type.NoEffect:
                    yield return battleDialogueBox.TypeDialogue($"It doesn't affect {defender.PokemonPrefixName}...");
                    yield break;
                case > Type.RegularEffect:
                    yield return battleDialogueBox.TypeDialogue("It's super effective!");
                    break;
                case < Type.RegularEffect:
                    yield return battleDialogueBox.TypeDialogue("It's not very effective...");
                    break;
            }

            if (Mathf.Approximately(critical, 2.0f))
            {
                yield return battleDialogueBox.TypeDialogue("A critical hit!");
            }
        }

        private static IEnumerator TypeStatEffectDetails(
            BattleDialogueBox battleDialogueBox,
            BattleUnit attacker,
            BattleUnit defender,
            StatEffect effect,
            bool limitReached)
        {
            var target = (effect.Self ? attacker : defender).PokemonPrefixName;
            if (limitReached)
            {
                var limit = effect.Amount > 0 ? "higher" : "lower";
                yield return battleDialogueBox.TypeDialogue($"{target}'s {effect.Stat} cannot go any {limit}!");
            }
            else
            {
                var action = effect.Amount > 0 ? "rose" : "fell";

                var amplitude = Mathf.Abs(effect.Amount) switch
                {
                    2 => " sharply",
                    3 => " drastically",
                    4 => " immensely",
                    _ => string.Empty
                };

                yield return battleDialogueBox.TypeDialogue($"{target}'s {effect.Stat} {action}{amplitude}!");
            }
        }

        private bool?[] ApplyStatEffects(BattleUnit attacker, BattleUnit defender)
        {
            var effects = new bool?[Asset.StatEffects.Length];
            for (var i = 0; i < effects.Length; ++i)
            {
                var statEffect = Asset.StatEffects[i];
                if (UnityEngine.Random.value > statEffect.Prob)
                {
                    effects[i] = null;
                }
                else
                {
                    var target = statEffect.Self ? attacker : defender;
                    var previousValue = target.GetStatModCalc(statEffect.Stat);

                    attacker[statEffect.Stat] = Mathf.Clamp(attacker[statEffect.Stat] + statEffect.Amount, -6, 6);
                    effects[i] = Mathf.Approximately(previousValue, target.GetStatModCalc(statEffect.Stat));
                }
            }

            return effects;
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
            var canMove = HandlePreTurnStatus(
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

            if (!AccuracyCheck(attacker, defender))
            {
                yield return battleDialogueBox.TypeDialogue($"{defender.PokemonPrefixName} avoided the attack!");
                yield break;
            }

            if (Asset.Category is Category.Physical or Category.Special)
            {
                attacker.PlayAttackAnimation();
                yield return new WaitForSeconds(1.0f);

                defender.PlayHitAnimation();

                HandleAttack(attacker, defender, out var typeEff, out var critical);
                yield return defender.PokemonHud.UpdateHp();
                yield return TypeDamageDetails(battleDialogueBox, defender, typeEff, critical);
                if (!defender.Pokemon.CanFight)
                {
                    //_currentState = State.EndOfTurn;
                    //yield return HandleFaint(defender);
                    yield break;
                }
            }

            // TODO Fix logic error where this cannot apply on self if accuracy check fails
            var effects = ApplyStatEffects(attacker, defender);
            for (var i = 0; i < effects.Length; ++i)
            {
                if (effects[i].HasValue)
                {
                    yield return TypeStatEffectDetails(
                        battleDialogueBox,
                        attacker,
                        defender,
                        Asset.StatEffects[i],
                        effects[i].Value
                    );
                }
            }

            if (Asset.StatusEffect.StatusCondition is StatusCondition.ID.None)
            {
                yield break;
            }

            // A status condition can only be applied if the Pokémon doesn't currently have one
            if (defender.Pokemon.StatusCondition is null)
            {
                // test for probability
                if (Utility.Math.Statistics.BernoulliTrial(Asset.StatusEffect.Prob))
                {
                    // check for type immunities
                    switch (Asset.StatusEffect.StatusCondition)
                    {
                        case StatusCondition.ID.Burn:
                            if (defender.Pokemon.HasType(Type.ID.Fire))
                            {
                                yield break;
                            }

                            break;
                        case StatusCondition.ID.Paralysis:
                            if (defender.Pokemon.HasType(Type.ID.Electric))
                            {
                                yield break;
                            }

                            break;
                        case StatusCondition.ID.Freeze:
                            if (defender.Pokemon.HasType(Type.ID.Ice))
                            {
                                yield break;
                            }

                            break;
                        case StatusCondition.ID.Poison or StatusCondition.ID.BadPoison:
                            if (defender.Pokemon.HasType(Type.ID.Poison))
                            {
                                yield break;
                            }

                            break;
                        case StatusCondition.ID.None or StatusCondition.ID.Sleep:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    defender.Pokemon.StatusCondition = StatusCondition.GetConditionByID(Asset.StatusEffect.StatusCondition);
                }

                yield return battleDialogueBox.TypeDialogue($"{defender} {defender.Pokemon.StatusCondition?.InflictMessage}");
            }
            else if (Asset.Category == Category.Status)
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
