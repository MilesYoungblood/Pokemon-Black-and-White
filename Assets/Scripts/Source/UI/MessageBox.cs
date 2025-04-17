using System.Collections;
using Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class MessageBox : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        [SerializeField] private TextMeshProUGUI message;

        [SerializeField] [Min(0)] private int frameRate;

        public void Submit(InputAction.CallbackContext context)
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

        public IEnumerator Print(Message newMessage)
        {
            yield return new WaitForEndOfFrame();
            playerController.ActionMap = "Dialogue";

            gameObject.SetActive(true);
            for (var i = 0; i < newMessage.Pages.Length; ++i)
            {
                this.message.text = string.Empty;
                foreach (var letter in newMessage.Pages[i])
                {
                    this.message.text += letter.ToString();
                    yield return new WaitForSeconds(1.0f / frameRate);
                }

                yield return new WaitUntil(IsPerformed);

                if (i < newMessage.Pages.Length - 1)
                {
                    AudioManager.Instance.PlaySound("Accept");
                }
            }

            if (playerController.ActionMap is "Dialogue")
            {
                playerController.ActionMap = "Overworld";
            }

            this.Deactivate();
        }

        private static bool IsPerformed()
        {
            return GameController.Instance.PlayerController.Actions.Dialogue.Submit.phase is InputActionPhase.Performed;
        }
    }
}
