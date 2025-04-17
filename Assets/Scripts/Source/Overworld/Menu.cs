using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class Menu : MonoBehaviour
    {
        #region Fields

        #region Serialized Fields

        [SerializeField] private Selector selector;

        [SerializeField] private MessageBox messageBox;

        [SerializeField] private PartyScreen partyScreen;

        [SerializeField] private SummaryScreen summaryScreen;

        [SerializeField] private InventoryScreen inventoryScreen;

        [SerializeField] private PlayerController playerController;

        #endregion

        private readonly StateMachine _stateMachine = new();

        #endregion

        #region Methods

        #region Event Functions

        private void Awake()
        {
            selector.OnCancel += _stateMachine.Pop;
        }

        private void OnDestroy()
        {
            selector.OnCancel -= _stateMachine.Pop;
        }

        private void Start()
        {
            selector.Callbacks = new Action[]
            {
                OnPokemonSubmit,
                OnPokedexSubmit,
                OnBagSubmit,
                OnSaveSubmit
            };
        }

        /// <summary>
        /// Assigns this selector to the player's UI input actions.
        /// </summary>
        private void OnEnable()
        {
            playerController += selector;
        }

        /// <summary>
        /// Unassigns this selector from the player's UI input actions.
        /// </summary>
        private void OnDisable()
        {
            playerController -= selector;
        }

        #endregion

        #region Input Actions

        [UsedImplicitly]
        public void Open(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _stateMachine.Push(new State(
                    Open,
                    this.Deactivate,
                    this.Activate,
                    Close
                ));
            }
        }

        #endregion

        /// <summary>
        /// Opens the <see cref="Menu"/> and transitions the Player's control to UI Selection.
        /// </summary>
        /// <remarks>
        /// <see cref="State.StrongEnter"/> of the <see cref="Menu"/>.
        /// </remarks>
        private void Open()
        {
            this.Activate();

            // The reason we do not need to set the UI action map of the player to upon activation is
            // that the menu is enabled upon leaving a state (Pokémon, Pokédex, Bag, etc.),
            // but it is already in UI selection, so resetting it is technically redundant.
            playerController.ActionMap = PlayerController.UISelectionMapping;
        }

        /// <summary>
        /// Closes the <see cref="Menu"/> and returns Overworld control to the Player.
        /// </summary>
        /// <remarks>
        /// <see cref="State.StrongExit"/> of the <see cref="Menu"/>.
        /// </remarks>
        private void Close()
        {
            this.Deactivate();

            // the reason we do not set the UI action map of the player upon deactivation is
            // that the menu is disabled upon choosing an option (Pokémon, Pokédex, Bag, etc.),
            // but it does not yet leave UI selection entirely.
            playerController.ActionMap = PlayerController.OverworldMapping;
        }

        #region Pokemon

        /// <summary>
        /// Opens the <see cref="PartyScreen"/>.
        /// </summary>
        private void OnPokemonSubmit()
        {
            _stateMachine.Push(new State(
                PartyScreenEnter,
                PartyScreenWeakExit,
                PartyScreenEnter,
                PartyScreenStrongExit
            ));
        }

        #region Party Screen

        private void PartyScreenSubmit()
        {
            _stateMachine.Push(new State(
                SummaryScreenStrongEnter,
                summaryScreen.Deactivate,
                summaryScreen.Activate,
                SummaryScreenStrongExit
            ));
        }

        /// <summary>
        /// Opens the <see cref="PartyScreen"/>.
        /// </summary>
        /// <remarks>
        /// Both <see cref="State.StrongEnter"/> and <see cref="State.WeakEnter"/> of the <see cref="Menu"/>.
        /// </remarks>
        private void PartyScreenEnter()
        {
            partyScreen.Callbacks = PartyScreenSubmit;
            partyScreen.Init(_stateMachine.Pop);
        }

        /// <summary>
        /// Opens the <see cref="SummaryScreen"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="State.WeakExit"/> of the <see cref="PartyScreen"/>.
        /// </remarks>
        private void PartyScreenWeakExit()
        {
            partyScreen.Destroy(_stateMachine.Pop);
        }

        /// <summary>
        /// Closes the <see cref="PartyScreen"/> and opens the <see cref="Menu"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="State.StrongExit"/> of the <see cref="PartyScreen"/>.
        /// </remarks>
        private void PartyScreenStrongExit()
        {
            partyScreen.Destroy(_stateMachine.Pop);
        }

        #endregion

        #region Summary Screen

        /// <summary>
        /// <see cref="State.StrongEnter"/> of the <see cref="SummaryScreen"/>.
        /// </summary>
        private void SummaryScreenStrongEnter()
        {
            summaryScreen.Activate();
            summaryScreen.Init(
                partyScreen.Selection,
                playerController.Player,
                playerController.Player.Party.ToArray(),
                new HashSet<string>
                {
                    "Pokemon Info Page",
                    "Moves Page"
                },
                _stateMachine.Pop
            );
        }

        /// <summary>
        /// <see cref="State.StrongExit"/> of the <see cref="SummaryScreen"/>.
        /// </summary>
        private void SummaryScreenStrongExit()
        {
            summaryScreen.Deactivate();
        }

        #endregion

        # endregion

        #region Pokedex

        private void OnPokedexSubmit()
        {
            Debug.LogWarning(new NotImplementedException());
        }

        #endregion

        #region Bag

        private void OnBagSubmit()
        {
            _stateMachine.Push(new State(
                InventoryScreenStrongEnter,
                null,
                inventoryScreen.Activate,
                inventoryScreen.Deactivate
            ));
        }

        private void InventoryScreenStrongEnter()
        {
            inventoryScreen.Init(
                playerController.Player.Inventory,
                _stateMachine.Pop,
                (typeof(Medicine), OnMedicineSubmit), (typeof(PokeBall), OnPokeBallSubmit), (typeof(BattleItem), OnBattleItemSubmit)
            );
        }

        private void OnMedicineSubmit()
        {
            var item = inventoryScreen.CurrentAsset;
            inventoryScreen.Deactivate();
            partyScreen.Callbacks = HandleSubmit;
            partyScreen.Init(HandleCancel);

            return;

            void HandleSubmit()
            {
                playerController.Player.Inventory.UseItem<Medicine>(item);
                partyScreen.Destroy(HandleCancel);
                StartCoroutine(
                    messageBox.Print(
                        new Message(item.Use(playerController.Player[partyScreen.Selection]))
                    )
                );
            }

            void HandleCancel()
            {
                partyScreen.Destroy(HandleCancel);
                OnBagSubmit();
            }
        }

        private void OnPokeBallSubmit()
        {
        }

        private void OnBattleItemSubmit()
        {
        }

        #endregion

        #region Save

        private void OnSaveSubmit()
        {
            StartCoroutine(Save());
        }

        private IEnumerator Save()
        {
            // TODO change dialogue to prompt user to decide on saving
            yield return messageBox.Print(new Message("Save complete!"));

            SavingSystem.Save("saveSlot1");
        }

        #endregion

        #endregion
    }
}
