using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class ActionSelector : BattleSelector
    {
        [SerializeField] private GameObject moveSelector;

        private void Awake()
        {
            Selector.Callbacks = new Action[]
            {
                OnFightSelected,
                OnBagSelected,
                OnPokemonSelected,
                OnRunSelected
            };
        }

        private void OnFightSelected()
        {
            if (PlayerUnit.Pokemon.MoveSet.All(move => !move.CanUse()))
            {
                BattleSystem.ExecuteTurn(new Move(), OpponentUnit.Pokemon.ChooseRandomMove());
            }
            else
            {
                moveSelector.SetActive(true);
            }
        }

        private void OnBagSelected()
        {
            BattleDialogueBox.DialogueEnabled = false;
            GameController.Instance.InventoryScreen.Init(
                (PlayerUnit.Battler as Player)?.Inventory,
                HandleItemCancel,
                (typeof(Medicine), HandleItemSelected), (typeof(PokeBall), HandleItemSelected), (typeof(BattleItem), HandleItemSelected)
            );

            return;

            void HandleItemSelected()
            {
                GameController.Instance.InventoryScreen.Close();
                BattleDialogueBox.DialogueEnabled = true;
                BattleSystem.ExecuteTurn(
                    GameController.Instance.InventoryScreen.CurrentAsset,
                    OpponentUnit.Pokemon.ChooseRandomMove()
                );
            }

            void HandleItemCancel()
            {
                GameController.Instance.InventoryScreen.Close();
                BattleDialogueBox.DialogueEnabled = true;
                BattleSystem.TriggerActionSelection();
            }
        }

        private void OnPokemonSelected()
        {
            BattleSystem.OpenPartyScreen(BattleSystem.State.ActionSelection);
        }

        private void OnRunSelected()
        {
            if (OpponentUnit.Battler is Pokemon)
            {
                BattleSystem.ExecuteTurn(new RunAway(), OpponentUnit.Pokemon.ChooseRandomMove());
            }
            else
            {
                //_currentState = State.Waiting;
                StartCoroutine(HandleErrorRunning());
            }

            return;

            IEnumerator HandleErrorRunning()
            {
                yield return BattleDialogueBox.TypeDialogue("You can't run from a trainer battle!");
                BattleSystem.TriggerActionSelection();
            }
        }
    }
}
