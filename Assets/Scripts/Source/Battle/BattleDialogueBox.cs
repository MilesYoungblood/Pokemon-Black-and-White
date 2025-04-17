using System;
using System.Collections;
using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class BattleDialogueBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogue;

        [SerializeField] private Selector selectionBox;

        [SerializeField] [Min(1)] private int lettersPerSecond;

        private bool isTyping;

        public string Dialogue
        {
            set => dialogue.text = value;
        }

        public bool DialogueEnabled
        {
            set
            {
                dialogue.enabled = value;
                dialogue.text = string.Empty;
            }
        }

        private void OnDisable()
        {
            dialogue.text = string.Empty;
        }

        public IEnumerator TypeDialogue(string value)
        {
            isTyping = true;

            var waitTime = 1.0f / lettersPerSecond;

            dialogue.text = string.Empty;
            foreach (var letter in value)
            {
                dialogue.text += letter.ToString();
                yield return new WaitForSeconds(waitTime);
            }

            yield return new WaitForSeconds(1.0f - waitTime);

            isTyping = false;
        }

        public bool IsNotTyping()
        {
            return !isTyping;
        }

        public void InitSelectionBox(Action[] callbacks, Action onCancel)
        {
            selectionBox.Callbacks = callbacks;
        }

        public void OpenSelectionBox()
        {
            selectionBox.gameObject.SetActive(true);
        }
    }
}
