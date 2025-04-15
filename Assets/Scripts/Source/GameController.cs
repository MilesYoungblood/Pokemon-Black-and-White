using System;
using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class GameController : MonoBehaviour
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

        [SerializeField] private PartyScreen partyScreen;

        [SerializeField] private SummaryScreen summaryScreen;

        [SerializeField] private InventoryScreen inventoryScreen;

        [SerializeField] private Fader fader;

        [SerializeField] private DialogueBox dialogueBox;

        private State _currentState;

        private State _previousState;

        private SceneInfo _currentScene;

        public State CurrentState
        {
            get => _currentState;
            private set
            {
                _previousState = CurrentState;
                _currentState = value;
                playerController.ActionMap = _currentState is State.Dialogue ? "Dialogue" : "Overworld";
            }
        }

        public SceneInfo CurrentScene
        {
            set
            {
                PreviousScene = _currentScene;
                _currentScene = value;
            }
        }

        public SceneInfo PreviousScene { get; private set; }

        public static GameController Instance { get; private set; }

        public PlayerController PlayerController
        {
            get => playerController;
            set => playerController = value;
        }

        public PartyScreen PartyScreen => partyScreen;

        public InventoryScreen InventoryScreen => inventoryScreen;

        public Fader Fader => fader;

        public DialogueBox DialogueBox => dialogueBox;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;

                Resources.LoadAll<PokemonAsset>("Pokemon");
                Resources.LoadAll<MoveAsset>("Moves");
                Resources.LoadAll<ItemAsset>("Items");

                EncounterLayer.OnWildEncounter += StartBattle;
                TrainerFOV.OnDialogueFinished += StartBattle;

                battleSystem.OnBattleOver += EndBattle;

                StartCoroutine(FreeMemory());
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            fader.Alpha = 1.0f;
            CurrentState = State.Paused;

            {
                //var previousState = dialogueBox.gameObject.activeSelf;
                //dialogueBox.gameObject.SetActive(true);
                //dialogueBox.gameObject.SetActive(previousState);
            }

            try
            {
                SavingSystem.Load("saveSlot1");
            }
            catch (Exception e)
            {
                print(e);
            }

            StartGame();
        }

        private void OnDestroy()
        {
            EncounterLayer.OnWildEncounter -= StartBattle;
            TrainerFOV.OnDialogueFinished -= StartBattle;

            battleSystem.OnBattleOver -= EndBattle;
        }

        private void StartGame()
        {
            fader.Alpha = 0.0f;
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
                playerController.ActionMap = "UI Selection";
                CurrentState = State.Paused;

                AudioManager.Instance.StopMusic();
                yield return AudioManager.Instance.PlayMusicAsync(battler.BattleThemeKey, true);

                yield return new WaitForSeconds(1.0f);

                fader.RGB = Color.white;
                for (var i = 0; i < 2; ++i)
                {
                    yield return fader.FadeIn(0.15f);
                    yield return fader.FadeOut(0.15f);
                }

                fader.RGB = Color.black;

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
            CurrentState = CurrentState is not State.Paused ? State.Paused : _previousState;
        }

        private static IEnumerator FreeMemory()
        {
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}
