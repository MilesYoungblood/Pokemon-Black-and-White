using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    public sealed class MessageTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private Message message;

        public IEnumerator Interact(PlayerController initiator)
        {
            Action action = null;
            if (TryGetComponent<MovementAI>(out var movement))
            {
                if (movement.CurrentState is not (MovementAI.State.Idle or MovementAI.State.Blocked))
                {
                    yield break;
                }

                movement.EntityController.LookAt = initiator.transform.position;
                movement.CurrentState = MovementAI.State.Interacting;
                action = movement.SetIdle;
            }

            yield return OpenDialogue(action);
        }

        public IEnumerator OpenDialogue(Action onFinished = null)
        {
            yield return GameController.Instance.MessageBox.Print(message);
            onFinished?.Invoke();
        }
    }
}
