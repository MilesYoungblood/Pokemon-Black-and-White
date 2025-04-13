using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class GameController : MonoBehaviour
    {
        public enum State
        {
            Overworld,
            Battle,
            Dialogue,
            Paused
        }

        [SerializeField] private PlayerController playerController;

        [SerializeField] private BattleSystem battleSystem;

        [SerializeField] private GameObject battleScreen;

        [SerializeField] private UI.PartyScreen partyScreen;

        [SerializeField] private UI.SummaryScreen summaryScreen;

        [SerializeField] private UI.InventoryScreen inventoryScreen;

        [SerializeField] private UI.Fader fader;

        [SerializeField] private UI.DialogueBox dialogueBox;

        [SerializeField] private Selector menu;

        public State CurrentState { get; private set; }

        private State _previousState;

        private SceneInfo _currentScene;

        public SceneInfo PreviousScene { get; private set; }

        public static GameController Instance { get; private set; }

        public PlayerController PlayerController => playerController;

        public UI.PartyScreen PartyScreen => partyScreen;

        public UI.InventoryScreen InventoryScreen => inventoryScreen;

        public UI.Fader Fader => fader;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;

                MoveAsset.Init();
                ItemAsset.Init();

                EncounterLayer.OnWildEncounter += StartBattle;

                TrainerController.OnDialogueFinished += StartBattle;

                menu.OnCancel += CancelMenu;
                battleSystem.OnBattleOver += EndBattle;

                StartCoroutine(FreeMemory());
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            fader.SetAlpha(1.0f);
            CurrentState = State.Paused;

            {
                var previousState = dialogueBox.gameObject.activeSelf;
                dialogueBox.gameObject.SetActive(true);
                dialogueBox.OnShowDialogue += () =>
                {
                    playerController.DisableInput();
                    TransitionState(State.Dialogue);
                };
                dialogueBox.OnCloseDialogue += () =>
                {
                    if (CurrentState == State.Dialogue)
                    {
                        playerController.EnableInput();
                    }
                };
                dialogueBox.gameObject.SetActive(previousState);
            }

            menu.Callbacks = new Action[]
            {
                HandlePokemonSelected,
                HandlePokedexSelected,
                HandleBagSelected,
                HandleSaveSelected
            };

            try
            {
                SavingSystem.Load("saveSlot1");
            }
            catch (Exception e)
            {
                print(e);
            }

            StartGame();

            return;

            void HandlePokemonSelected()
            {
                partyScreen.Callbacks = HandlePartyScreenSelect;
                partyScreen.Init(HandlePartyScreenCancel);

                return;

                void HandlePartyScreenSelect()
                {
                    summaryScreen.gameObject.SetActive(true);
                    summaryScreen.Init(partyScreen.Selection, playerController.Player,
                        playerController.Player.Party.ToArray(),
                        new HashSet<string>
                        {
                            "Pokemon Info Page",
                            "Moves Page"
                        }, HandleSummaryScreenCancel
                    );
                    partyScreen.Close();
                    partyScreen.Destroy(HandleSummaryScreenCancel);

                    return;

                    void HandleSummaryScreenCancel()
                    {
                        summaryScreen.gameObject.SetActive(false);
                        HandlePokemonSelected();
                    }
                }

                void HandlePartyScreenCancel()
                {
                    partyScreen.Close();
                    partyScreen.Destroy(HandlePartyScreenCancel);
                    OpenMenu();
                }
            }

            void HandleBagSelected()
            {
                HandleOpenInventoryScreen((typeof(Medicine), HandleMedicineSelect), (typeof(PokeBall), null), (typeof(BattleItem), null));

                return;

                void HandleOpenInventoryScreen(params (System.Type, Action)[] sets)
                {
                    inventoryScreen.Init(playerController.Player.Inventory, HandleInventoryScreenCancel, sets);
                }

                void HandleMedicineSelect()
                {
                    var item = inventoryScreen.CurrentAsset;
                    inventoryScreen.Close();
                    partyScreen.Callbacks = HandleSelect;
                    partyScreen.Init(HandleCancel);

                    return;

                    void HandleSelect()
                    {
                        playerController.Player.Inventory.UseItem<Medicine>(item);
                        partyScreen.Close();
                        partyScreen.Destroy(HandleCancel);
                        StartCoroutine(
                            dialogueBox.ShowDialogue(
                                new UI.Dialogue(item.Use(playerController.Player.Party[partyScreen.Selection]))
                            )
                        );
                    }

                    void HandleCancel()
                    {
                        partyScreen.Close();
                        partyScreen.Destroy(HandleCancel);
                        HandleOpenInventoryScreen((typeof(Medicine), HandleMedicineSelect), (typeof(PokeBall), null), (typeof(BattleItem), null));
                    }
                }

                void HandleInventoryScreenCancel()
                {
                    inventoryScreen.Close();
                    OpenMenu();
                }
            }

            void HandlePokedexSelected()
            {
                print(new NotImplementedException());
                CurrentState = State.Overworld;
            }

            void HandleSaveSelected()
            {
                StartCoroutine(HandleSave());

                return;

                IEnumerator HandleSave()
                {
                    // TODO change dialogue to prompt user to decide on saving
                    yield return dialogueBox.ShowDialogue(new UI.Dialogue("Save complete!"));

                    SavingSystem.Save("saveSlot1");
                }
            }
        }

        private void OnDestroy()
        {
            EncounterLayer.OnWildEncounter -= StartBattle;

            TrainerController.OnDialogueFinished -= StartBattle;

            battleSystem.OnBattleOver -= EndBattle;

            menu.OnCancel -= CancelMenu;
        }

        private void StartGame()
        {
            fader.SetAlpha(0.0f);
            UnlockPlayer();
        }

        private void UnlockPlayer()
        {
            CurrentState = State.Overworld;
            playerController.EnableInput();
        }

        private void StartBattle(IBattler opponent)
        {
            StartCoroutine(HandleStart(opponent));

            return;

            IEnumerator HandleStart(IBattler battler)
            {
                playerController.DisableInput();
                CurrentState = State.Paused;

                AudioManager.Instance.StopMusic();
                yield return AudioManager.Instance.PlayMusicAsync(battler.BattleThemeKey, true);

                yield return new WaitForSeconds(1.0f);

                fader.SetRGB(Color.white);
                for (var i = 0; i < 2; ++i)
                {
                    yield return fader.FadeIn(0.15f);
                    yield return fader.FadeOut(0.15f);
                }

                fader.SetRGB(Color.black);

                CurrentState = State.Battle;

                yield return new WaitForSeconds(0.5f);

                yield return fader.FadeIn(1.25f);

                battleScreen.SetActive(true);

                yield return fader.FadeOut(0.25f);

                yield return battleSystem.Init(playerController.Player, battler);
            }
        }

        private void EndBattle()
        {
            StartCoroutine(HandleEnding());
            return;

            IEnumerator HandleEnding()
            {
                yield return fader.FadeIn(1.0f);
                yield return fader.FadeOut(0.0f);

                battleScreen.SetActive(false);
                UnlockPlayer();
                yield return AudioManager.Instance.RequestSong(_currentScene.Music);
            }
        }

        public void TogglePause()
        {
            TransitionState(CurrentState != State.Paused ? State.Paused : _previousState);
        }

        private void TransitionState(State nextState)
        {
            _previousState = CurrentState;
            CurrentState = nextState;
        }

        public void TransitionScene(SceneInfo nextScene)
        {
            PreviousScene = _currentScene;
            _currentScene = nextScene;
        }

        public void OpenMenu()
        {
            menu.gameObject.SetActive(true);
            playerController.DisableInput();
            CurrentState = State.Paused;
        }

        private void CancelMenu()
        {
            menu.gameObject.SetActive(false);
            UnlockPlayer();
        }

        private static IEnumerator FreeMemory()
        {
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}
