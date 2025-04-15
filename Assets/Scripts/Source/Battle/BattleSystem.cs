using System;
using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class BattleSystem : MonoBehaviour
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

        [SerializeField] private GameObject actionSelector;

        public event Action OnBattleOver;

        private State _currentState;

        public ulong Turn { get; private set; }

        public GameObject ActionSelector => actionSelector;

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

            return;

            IEnumerator HandleTrainerNextPokemon()
            {
                yield return battleDialogueBox.TypeDialogue($"{opponentUnit.Battler} is about to send out {opponentUnit.Pokemon}.");
                yield return battleDialogueBox.TypeDialogue("Would you like to swap Pokemon?");

                _currentState = State.NextPokemon;
                battleDialogueBox.OpenSelectionBox();
            }
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

            battleDialogueBox.InitSelectionBox(new Action[]
            {
                HandleSelectionBoxYes,
                HandleSelectionBoxNo
            }, HandleSelectionBoxNo);

            TriggerActionSelection();

            yield break;

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
            battleDialogueBox.Dialogue = $"What will {playerUnit.Pokemon} do?";
            ActionSelector.SetActive(true);
        }

        public void OpenPartyScreen(State calledFrom)
        {
            GameController.Instance.PartyScreen.CalledFrom = calledFrom;
            GameController.Instance.PartyScreen.Callbacks = HandlePartyScreenSelected;
            GameController.Instance.PartyScreen.Init(HandlePartyScreenCancel);

            return;

            void HandlePartyScreenSelected()
            {
                var selectedMember = playerUnit.Battler.Party[GameController.Instance.PartyScreen.Selection];
                if (selectedMember is { CanFight: false })
                {
                    GameController.Instance.PartyScreen.Message = $"{playerUnit.Battler.Party[GameController.Instance.PartyScreen.Selection]} is fainted!";
                    return;
                }

                if (selectedMember == playerUnit.Pokemon)
                {
                    GameController.Instance.PartyScreen.Message = $"{playerUnit.Pokemon} is already in battle!";
                    return;
                }

                GameController.Instance.PartyScreen.Close();
                GameController.Instance.PartyScreen.Destroy(HandlePartyScreenCancel);

                // only start the turn if the player chose to switch from the action selection screen
                if (GameController.Instance.PartyScreen.CalledFrom is State.ActionSelection)
                {
                    StartCoroutine(RunTurn(
                        new SwitchOut(GameController.Instance.PartyScreen.Selection),
                        opponentUnit.Pokemon.ChooseRandomMove())
                    );
                }
                else
                {
                    StartCoroutine(new SwitchOut(GameController.Instance.PartyScreen.Selection).Use(
                        this,
                        playerUnit,
                        opponentUnit,
                        battleDialogueBox
                    ));
                }
            }

            void HandlePartyScreenCancel()
            {
                if (!playerUnit.Pokemon.CanFight)
                {
                    return;
                }

                GameController.Instance.PartyScreen.Close();
                GameController.Instance.PartyScreen.Destroy(HandlePartyScreenCancel);
                GameController.Instance.PartyScreen.CalledFrom = null;

                TriggerActionSelection();
            }
        }

        public IEnumerator Faint(BattleUnit faintedBattleUnit)
        {
            _currentState = State.EndOfTurn;

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
                    yield return AudioManager.Instance.PlayMusicAsync(opponentUnit.Battler.VictoryThemeKey, true);
                }

                faintedBattleUnit.Battler.HandleDefeat();
                yield return Functional.WaitUntilThenCall(battleDialogueBox.IsNotTyping, Terminate);
            }
        }

        private IEnumerator PostTurnStatus(BattleUnit target)
        {
            if (_currentState is not State.End)
            {
                yield return target.Pokemon.StatusCondition?.PostTurn(
                    this,
                    battleDialogueBox,
                    target
                );
            }
        }

        public void ExecuteTurn(IBattleAction playerAction, IBattleAction opponentAction)
        {
            StartCoroutine(RunTurn(playerAction, opponentAction));
        }

        private IEnumerator RunTurn(IBattleAction playerAction, IBattleAction opponentAction)
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

            var previousSize = (secondUnit.Battler as Trainer)?.Party.Count ?? -1;

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

            yield return PostTurnStatus(firstUnit);
            yield return PostTurnStatus(secondUnit);

            ++Turn;

            if (secondUnit.Battler is Trainer trainer && previousSize > trainer.Party.Count)
            {
                yield return new WaitUntil(HandleIsSelected);
                yield break;
            }

            TriggerActionSelection();

            yield break;

            bool HandleIsSelected()
            {
                return _currentState is State.ActionSelection;
            }
        }
    }
}
