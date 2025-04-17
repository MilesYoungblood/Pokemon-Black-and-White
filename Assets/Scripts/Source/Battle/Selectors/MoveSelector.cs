using System;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class MoveSelector : BattleSelector
    {
        [SerializeField] private GameObject moveDetails;

        [SerializeField] private TextMeshProUGUI[] moves;

        private void Awake()
        {
            Selector.OnCancel += OnMoveCancel;
        }

        private void OnDestroy()
        {
            Selector.OnCancel -= OnMoveCancel;
        }

        private void OnEnable()
        {
            moveDetails.SetActive(true);
            BattleDialogueBox.DialogueEnabled = false;

            Selector.Callbacks = new Action[PlayerUnit.Pokemon.Moveset.Count];
            for (var i = 0; i < Selector.Callbacks.Length; ++i)
            {
                Selector.Callbacks[i] = OnMoveSelected;
                moves[i].text = PlayerUnit.Pokemon[i].ToString();
            }
        }

        private void OnDisable()
        {
            moveDetails.SetActive(false);
            BattleDialogueBox.DialogueEnabled = true;
        }

        private void OnMoveSelected()
        {
            if (Selector.Selection >= PlayerUnit.Battler.Party.Count)
            {
                return;
            }

            if (PlayerUnit.Pokemon[Selector.Selection].CanUse())
            {
                return;
            }

            gameObject.SetActive(false);

            BattleSystem.ExecuteTurn(
                PlayerUnit.Pokemon[Selector.Selection],
                OpponentUnit.Pokemon.ChooseRandomMove()
            );
        }

        private void OnMoveCancel()
        {
            gameObject.SetActive(false);
            BattleSystem.TriggerActionSelection();
        }
    }
}
