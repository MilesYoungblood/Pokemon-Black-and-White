using System;
using System.Collections;
using System.Linq;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    public class BattleSystem : MonoBehaviour
    {
        public enum State
        {
            ActionSelection,
            RunTurn,
            EndOfTurn,
            NextPokemon,
            Waiting,
            End
        }

        [SerializeField] private BattleUnit playerUnit;

        [SerializeField] private BattleUnit opponentUnit;

        [SerializeField] private BattleDialogueBox battleDialogueBox;

        public event Action OnBattleOver;

        private State _currentState;

        public ulong Turn { get; private set; }

        private void Awake()
        {
            Player.OnNextPokemon += PlayerNext;
            Trainer.OnNextPokemon += TrainerNext;
            Trainer.OnDefeat += TrainerDefeat;
        }

        private void OnDestroy()
        {
            Player.OnNextPokemon -= PlayerNext;
            Trainer.OnNextPokemon -= TrainerNext;
            Trainer.OnDefeat -= TrainerDefeat;
        }

        private void PlayerNext()
        {
            OpenPartyScreen(State.EndOfTurn);
        }

        private void TrainerNext()
        {
            StartCoroutine(HandleTrainerNextPokemon());
        }

        private void TrainerDefeat()
        {
            _currentState = State.EndOfTurn;
            StartCoroutine(battleDialogueBox.TypeDialogue($"You defeated {opponentUnit.Battler}!"));
        }

        public IEnumerator Init(Player player, IBattler opponent)
        {
            yield return opponentUnit.Init(opponent);
            foreach (var message in opponentUnit.Battler.StartingDialogue)
            {
                yield return battleDialogueBox.TypeDialogue(message);
            }

            yield return playerUnit.Init(player);
            yield return battleDialogueBox.TypeDialogue($"Go, {player.ActivePokemon}!");

            battleDialogueBox.InitActionSelector(new Action[]
            {
                HandleFightSelected,
                HandleBagSelected,
                HandlePokemonSelected,
                HandleRunSelected
            }, null);

            battleDialogueBox.InitMoveSelector(playerUnit.Pokemon.MoveSet, OnMoveSelected, OnMoveCancel);

            battleDialogueBox.InitItemTypeSelector(new Action[]
            {
                null,
                null,
                HandlePokeBallsSelected,
                null
            }, HandleItemTypeCancel);

            battleDialogueBox.InitSelectionBox(new Action[]
            {
                HandleSelectionBoxYes,
                HandleSelectionBoxNo
            }, HandleSelectionBoxNo);

            TriggerActionSelection();

            yield break;

            void HandleFightSelected()
            {
                if (playerUnit.Pokemon.MoveSet.All(move => !move.CanUse()))
                {
                    // TODO replace with Struggle
                    StartCoroutine(HandleTurn(new Move(), opponentUnit.Pokemon.ChooseRandomMove()));
                }
                else
                {
                    battleDialogueBox.EnableMoveSelector(true);
                }
            }

            void HandleBagSelected()
            {
                battleDialogueBox.EnableDialogueText(false);
                battleDialogueBox.OpenItemTypeSelector();
            }

            void HandlePokemonSelected()
            {
                OpenPartyScreen(State.ActionSelection);
            }

            void HandleRunSelected()
            {
                if (opponentUnit.Battler is Pokemon)
                {
                    StartCoroutine(HandleTurn(new RunAway(), opponentUnit.Pokemon.ChooseRandomMove()));
                }
                else
                {
                    _currentState = State.Waiting;
                    StartCoroutine(HandleErrorRunning());
                }

                return;

                IEnumerator HandleErrorRunning()
                {
                    yield return battleDialogueBox.TypeDialogue("You can't run from a trainer battle!");
                    TriggerActionSelection();
                }
            }

            void HandleResourceItemsSelected()
            {
                print(new NotImplementedException());
            }

            void HandleStatusItemsSelected()
            {
                print(new NotImplementedException());
            }

            void HandlePokeBallsSelected()
            {
                GameController.Instance.InventoryScreen.Init(
                    player.Inventory,
                    HandleItemCancel,
                    (typeof(PokeBall), HandleItemSelected)
                );

                return;

                void HandleItemSelected()
                {
                    GameController.Instance.InventoryScreen.Close();
                    battleDialogueBox.EnableDialogueText(true);

                    StartCoroutine(HandleTurn(
                        GameController.Instance.InventoryScreen.CurrentAsset,
                        opponentUnit.Pokemon.ChooseRandomMove()
                    ));
                }

                void HandleItemCancel()
                {
                    GameController.Instance.InventoryScreen.Close();
                    battleDialogueBox.OpenItemTypeSelector();
                }
            }

            void HandleBattleItemsSelected()
            {
                print(new NotImplementedException());
            }

            void HandleItemTypeCancel()
            {
                battleDialogueBox.EnableDialogueText(true);
                TriggerActionSelection();
            }

            void HandleSelectionBoxYes()
            {
                OpenPartyScreen(State.NextPokemon);
            }

            void HandleSelectionBoxNo()
            {
                StartCoroutine(HandleNoSelection());

                return;

                IEnumerator HandleNoSelection()
                {
                    yield return opponentUnit.Init();
                    yield return battleDialogueBox.TypeDialogue($"{opponentUnit.Battler} sent out {opponentUnit.Pokemon}.");
                    TriggerActionSelection();
                }
            }
        }

        public void Terminate()
        {
            _currentState = State.End;
            StopAllCoroutines();

            Turn = 1;
            OnBattleOver?.Invoke();
        }

        public void TriggerActionSelection()
        {
            _currentState = State.ActionSelection;
            battleDialogueBox.SetDialogue($"What will {playerUnit.Pokemon} do?");
            battleDialogueBox.EnableActionSelector(true);
        }

        public void OnMoveSelected()
        {
            if (battleDialogueBox.MoveSelection >= playerUnit.Battler.Party.Count)
            {
                return;
            }

            if (playerUnit.Pokemon[battleDialogueBox.MoveSelection].CanUse())
            {
                return;
            }

            battleDialogueBox.EnableMoveSelector(false);

            StartCoroutine(HandleTurn(
                playerUnit.Pokemon[battleDialogueBox.MoveSelection],
                opponentUnit.Pokemon.ChooseRandomMove()
            ));
        }

        public void OnMoveCancel()
        {
            battleDialogueBox.EnableMoveSelector(false);
            TriggerActionSelection();
        }

        private void OpenPartyScreen(State calledFrom)
        {
            GameController.Instance.PartyScreen.CalledFrom = calledFrom;
            GameController.Instance.PartyScreen.Init(PartyScreenSelected, PartyScreenCancel);

            return;

            void PartyScreenSelected()
            {
                var selectedMember = playerUnit.Battler.Party[GameController.Instance.PartyScreen.Selection];
                if (selectedMember is { CanFight: false })
                {
                    GameController.Instance.PartyScreen.SetMessageText($"{playerUnit.Battler.Party[GameController.Instance.PartyScreen.Selection]} is fainted!");
                    return;
                }

                if (selectedMember == playerUnit.Pokemon)
                {
                    GameController.Instance.PartyScreen.SetMessageText($"{playerUnit.Pokemon} is already in battle!");
                    return;
                }

                GameController.Instance.PartyScreen.Close();

                // only start the turn if the player chose to switch from the action selection screen
                if (GameController.Instance.PartyScreen.CalledFrom is State.ActionSelection)
                {
                    StartCoroutine(HandleTurn(
                        new SwitchOut(GameController.Instance.PartyScreen.Selection),
                        opponentUnit.Pokemon.ChooseRandomMove())
                    );
                }
                else
                {
                    StartCoroutine(new SwitchOut(GameController.Instance.PartyScreen.Selection).Use(this, playerUnit, opponentUnit, battleDialogueBox));
                }
            }

            void PartyScreenCancel()
            {
                if (!playerUnit.Pokemon.CanFight)
                {
                    return;
                }

                GameController.Instance.PartyScreen.Close();
                GameController.Instance.PartyScreen.CalledFrom = null;

                TriggerActionSelection();
            }
        }

        private IEnumerator HandleFaint(BattleUnit faintedBattleUnit)
        {
            yield return battleDialogueBox.TypeDialogue($"{faintedBattleUnit.PokemonPrefixName} fainted!");
            faintedBattleUnit.PlayFaintAnimation();

            faintedBattleUnit.Battler.HandleFaint();

            yield return new WaitForSeconds(1.0f);

            if (faintedBattleUnit.Battler.CanFight)
            {
                faintedBattleUnit.Battler.HandleNextPokemon();
            }
            else
            {
                if (!faintedBattleUnit.IsPlayerUnit)
                {
                    AudioManager.Instance.PlayMusicAsync(opponentUnit.Battler.VictoryThemeKey, true);
                }

                faintedBattleUnit.Battler.HandleDefeat();
                yield return Functional.WaitUntilThenCall(() => !battleDialogueBox.IsTyping, Terminate);
            }
        }

        private IEnumerator HandleTrainerNextPokemon()
        {
            yield return battleDialogueBox.TypeDialogue($"{opponentUnit.Battler} is about to send out {opponentUnit.Pokemon}.");
            yield return battleDialogueBox.TypeDialogue("Would you like to swap Pokemon?");

            _currentState = State.NextPokemon;
            battleDialogueBox.OpenSelectionBox();
        }

        private IEnumerator HandlePostTurnStatus(BattleUnit target)
        {
            // TODO implement badly poisoned
            if (_currentState is State.End || target.Pokemon.StatusCondition is not (Burn or Poison or BadPoison))
            {
                yield break;
            }

            var message = target.Pokemon.StatusCondition is Burn
                ? "was hurt by it's burn"
                : "took damage from poisoning";

            target.Pokemon.HP -= Mathf.RoundToInt(target.Pokemon.MaxHP / 8.0f);
            target.PlayHitAnimation();
            yield return target.PokemonHud.UpdateHp();
            yield return battleDialogueBox.TypeDialogue($"{target.PokemonPrefixName} {message}!");
            if (!target.Pokemon.CanFight)
            {
                yield return HandleFaint(target);
            }
        }

        private IEnumerator HandleTurn(IBattleAction playerAction, IBattleAction opponentAction)
        {
            _currentState = State.RunTurn;

            bool playerFirst;
            if (playerAction.Priority == opponentAction.Priority)
            {
                playerFirst = playerAction.HandleSamePriority(playerUnit, opponentUnit);
            }
            else
            {
                playerFirst = playerAction.Priority > opponentAction.Priority;
            }

            var firstAction = playerFirst ? playerAction : opponentAction;
            var secondAction = playerFirst ? opponentAction : playerAction;

            var firstUnit = playerFirst ? playerUnit : opponentUnit;
            var secondUnit = playerFirst ? opponentUnit : playerUnit;

            var previousSize = -1;
            {
                if (secondUnit.Battler is Trainer trainer)
                {
                    previousSize = trainer.Party.Count;
                }
            }

            yield return firstAction.Use(this, firstUnit, secondUnit, battleDialogueBox);
            firstUnit.Pokemon.StatusCondition?.Update();
            firstUnit.UpdateVolatileStatusCounters();

            if (secondUnit.Battler.CanFight)
            {
                if (_currentState is State.RunTurn)
                {
                    yield return secondAction.Use(this, secondUnit, firstUnit, battleDialogueBox);
                    secondUnit.Pokemon.StatusCondition?.Update();
                    secondUnit.UpdateVolatileStatusCounters();
                }
            }
            else
            {
                yield break;
            }

            // Post turn
            _currentState = State.EndOfTurn;

            yield return HandlePostTurnStatus(firstUnit);
            yield return HandlePostTurnStatus(secondUnit);

            ++Turn;

            {
                if (secondUnit.Battler is Trainer trainer && previousSize > trainer.Party.Count)
                {
                    yield return new WaitUntil(IsSelected);
                    yield break;
                }
            }

            TriggerActionSelection();

            yield break;

            bool IsSelected()
            {
                return _currentState is State.ActionSelection;
            }
        }
    }
}
