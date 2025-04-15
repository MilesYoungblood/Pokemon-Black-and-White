using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class Menu : MonoBehaviour
    {
        [SerializeField] private Selector selector;

        [SerializeField] private DialogueBox dialogueBox;

        [SerializeField] private PlayerController playerController;

        [SerializeField] private PartyScreen partyScreen;

        [SerializeField] private SummaryScreen summaryScreen;

        [SerializeField] private InventoryScreen inventoryScreen;

        private void Awake()
        {
            selector.OnCancel += Close;
        }

        private void OnDestroy()
        {
            selector.OnCancel -= Close;
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

        private void OnEnable()
        {
            playerController += selector;
        }

        private void OnDisable()
        {
            playerController -= selector;
        }

        public void Open(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            Open();
            playerController.ActionMap = "UI Selection";
        }

        private void Open()
        {
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
            playerController.ActionMap = "Overworld";
        }

        #region Party Screen

        private void OnPokemonSubmit()
        {
            partyScreen.Callbacks = OnPartyScreenSubmit;
            partyScreen.Init(OnPartyScreenCancel);
            gameObject.SetActive(false);
        }

        private void OnPartyScreenSubmit()
        {
            summaryScreen.gameObject.SetActive(true);
            summaryScreen.Init(
                partyScreen.Selection,
                playerController.Player,
                playerController.Player.Party.ToArray(),
                new HashSet<string>
                {
                    "Pokemon Info Page",
                    "Moves Page"
                },
                OnSummaryScreenCancel
            );
            partyScreen.Close();
            partyScreen.Destroy(OnPartyScreenCancel);
        }

        private void OnPartyScreenCancel()
        {
            partyScreen.Close();
            partyScreen.Destroy(OnPartyScreenCancel);
            Open();
        }

        #endregion

        #region Summary Screen

        private void OnSummaryScreenCancel()
        {
            summaryScreen.gameObject.SetActive(false);
            OnPartyScreenSubmit();
        }

        #endregion

        #region Pokedex

        private void OnPokedexSubmit()
        {
            print(new NotImplementedException());
        }

        #endregion

        #region Bag

        private void OnBagSubmit()
        {
            inventoryScreen.Init(
                playerController.Player.Inventory,
                OnBagCancel,
                (typeof(Medicine), OnMedicineSubmit), (typeof(PokeBall), OnPokeBallSubmit), (typeof(BattleItem), OnBattleItemSubmit)
            );
            gameObject.SetActive(false);
        }

        private void OnBagCancel()
        {
            inventoryScreen.Close();
            Open();
        }

        private void OnMedicineSubmit()
        {
            var item = inventoryScreen.CurrentAsset;
            inventoryScreen.Close();
            partyScreen.Callbacks = HandleSubmit;
            partyScreen.Init(HandleCancel);

            return;

            void HandleSubmit()
            {
                playerController.Player.Inventory.UseItem<Medicine>(item);
                partyScreen.Close();
                partyScreen.Destroy(HandleCancel);
                StartCoroutine(
                    dialogueBox.ShowDialogue(
                        new Dialogue(item.Use(playerController.Player[partyScreen.Selection]))
                    )
                );
            }

            void HandleCancel()
            {
                partyScreen.Close();
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

        private void OnSaveSubmit()
        {
            StartCoroutine(Save());
        }

        private IEnumerator Save()
        {
            // TODO change dialogue to prompt user to decide on saving
            yield return dialogueBox.ShowDialogue(new Dialogue("Save complete!"));

            SavingSystem.Save("saveSlot1");
        }
    }
}
