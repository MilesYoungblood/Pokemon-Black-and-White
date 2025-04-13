using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    public class BattleDialogueBox : MonoBehaviour
    {
        private const int LettersPerSecond = 30;

        private const float WaitTime = 1.0f / LettersPerSecond;

        [SerializeField] private TextMeshProUGUI dialogue;

        [SerializeField] private Selector actionSelector;

        [SerializeField] private Selector moveSelector;

        [SerializeField] private Selector itemTypeSelector;

        [SerializeField] private TextMeshProUGUI ppLabel;

        [SerializeField] private TextMeshProUGUI typeLabel;

        [SerializeField] private Selector selectionBox;

        private GameObject _moveDetails;

        private TextMeshProUGUI[] _moves;

        public bool IsTyping { get; private set; }

        public int MoveSelection => moveSelector.Selection;

        private void Awake()
        {
            _moveDetails = transform.Find("Move Details").gameObject;
            _moves = moveSelector.GetComponentsInChildren<TextMeshProUGUI>();
        }

        private void OnDisable()
        {
            dialogue.text = string.Empty;
        }

        public void SetDialogue(string value)
        {
            dialogue.text = value;
        }

        public IEnumerator TypeDialogue(string value)
        {
            IsTyping = true;

            dialogue.text = string.Empty;
            foreach (var letter in value)
            {
                dialogue.text += letter;
                yield return new WaitForSeconds(WaitTime);
            }

            yield return new WaitForSeconds(1.0f - WaitTime);

            IsTyping = false;
        }

        public void EnableDialogueText(bool value)
        {
            dialogue.enabled = value;
            dialogue.text = string.Empty;
        }

        public void EnableActionSelector(bool value)
        {
            actionSelector.gameObject.SetActive(value);
        }

        public void EnableMoveSelector(bool value)
        {
            moveSelector.gameObject.SetActive(value);
            _moveDetails.SetActive(value);
            EnableDialogueText(!value);
        }

        public void OpenItemTypeSelector()
        {
            itemTypeSelector.gameObject.SetActive(true);
        }

        public void InitActionSelector(Action[] callbacks, Action onCancel)
        {
            //actionSelector.Init(callbacks, onCancel);
        }

        public void InitMoveSelector(List<Move> moves, Action onSelected, Action onCancel)
        {
            var callbacks = new Action[moves.Count];
            for (var i = 0; i < callbacks.Length; ++i)
            {
                callbacks[i] = onSelected;
                _moves[i].text = moves[i].ToString();
            }

            //moveSelector.Init(callbacks, onCancel, HandleUpdate);

            return;

            void HandleUpdate()
            {
                var move = moves[moveSelector.Selection];
                ppLabel.text = $"PP: {move.PP.ToString()}/{move.MaxPP.ToString()}";
                typeLabel.text = move.Asset.Type.ToString();
            }
        }

        public void InitItemTypeSelector(Action[] callbacks, Action onCancel)
        {
            //itemTypeSelector.Init(callbacks, onCancel);
        }

        public void InitSelectionBox(Action[] callbacks, Action onCancel)
        {
            //selectionBox.Init(callbacks, onCancel);
        }

        public void OpenSelectionBox()
        {
            selectionBox.gameObject.SetActive(true);
        }
    }
}
