using System;
using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class TrainerFOV : MonoBehaviour
    {
        [SerializeField] private TrainerController trainerController;

        [SerializeField] private GameObject exclamation;

        public static event Action<Trainer> OnDialogueFinished;

        public IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (GameController.Instance.CurrentState is not GameController.State.Overworld ||
                !other.TryGetComponent<PlayerController>(out var playerController))
            {
                yield break;
            }

            yield return new WaitUntil(playerController.IsCenteredOnTile);

            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySound("Spotted");

            playerController.ActionMap = "Dialogue";

            var prefab = Instantiate(exclamation, transform);
            yield return new WaitForSeconds(1.0f);
            Destroy(prefab);

            var offset = playerController.transform.position - transform.position;
            yield return trainerController.Move(EntityController.Speed.Walk, Vector3Int.RoundToInt(offset - offset.normalized));

            playerController.LookAt = transform.position;
            if (trainerController.TryGetComponent<MessageTrigger>(out var messageTrigger))
            {
                yield return messageTrigger.OpenDialogue(HandleFinished);
            }
            else
            {
                HandleFinished();
            }

            yield break;

            void HandleFinished()
            {
                OnDialogueFinished?.Invoke(trainerController.Trainer);
            }
        }
    }
}
