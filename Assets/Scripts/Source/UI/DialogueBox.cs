using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source.UI
{
    [DisallowMultipleComponent]
    public class DialogueBox : MonoBehaviour
    {
        private DialogueBoxInput _dialogueBoxInput;

        private TextMeshProUGUI _dialogueText;

        public event Action OnShowDialogue;

        public event Action OnCloseDialogue;

        public static DialogueBox Instance { get; private set; }

        public void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                _dialogueText = GetComponentInChildren<TextMeshProUGUI>();
                _dialogueBoxInput = new DialogueBoxInput();
                _dialogueBoxInput.Default.Accept.performed += HandleAccept;
            }
        }

        private void OnEnable()
        {
            _dialogueBoxInput.Default.Enable();
        }

        private void OnDisable()
        {
            _dialogueBoxInput.Default.Disable();
        }

        private void OnDestroy()
        {
            _dialogueBoxInput.Default.Accept.performed -= HandleAccept;
        }

        public void HandleAccept(InputAction.CallbackContext context)
        {
            /*
            if (!context.performed || _isTyping)
            {
                return;
            }

            ++_currentPage;
            if (_currentPage < _dialogue.Pages.Length)
            {
                AudioManager.Instance.PlaySound("Accept");
                StartCoroutine(TypeDialogue(_dialogue.Pages[_currentPage]));
            }
            else
            {
                _currentPage = 0;
                gameObject.SetActive(false);
                _onDialogueFinished?.Invoke();
                OnCloseDialogue?.Invoke();
            }
            */
        }

        public IEnumerator ShowDialogue(Dialogue dialogue)
        {
            yield return new WaitForEndOfFrame();
            OnShowDialogue?.Invoke();

            gameObject.SetActive(true);
            for (var i = 0; i < dialogue.Pages.Length; ++i)
            {
                yield return TypeDialogue(dialogue.Pages[i]);
                yield return new WaitUntil(() => _dialogueBoxInput.Default.Accept.phase == InputActionPhase.Performed);

                if (i < dialogue.Pages.Length - 1)
                {
                    //AudioManager.Instance.PlaySound("Accept");
                }
            }

            gameObject.SetActive(false);
            OnCloseDialogue?.Invoke();
        }

        private IEnumerator TypeDialogue(string line)
        {
            _dialogueText.text = string.Empty;
            foreach (var letter in line)
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(1.0f / 50.0f);
            }
        }
    }
}
