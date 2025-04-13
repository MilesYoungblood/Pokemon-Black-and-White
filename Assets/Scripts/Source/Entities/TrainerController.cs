using System;
using System.Collections;
using Scripts.Utility;
using Scripts.Utility.Math;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Trainer), typeof(BoxCollider2D))]
    public class TrainerController : NPCController, IInteractable, ISavable
    {
        [SerializeField] private GameObject exclamation;

        [SerializeField] private Trainer trainer;

        [SerializeField] private BoxCollider2D fov;

        protected override Vector2Int Direction
        {
            get => base.Direction;
            set
            {
                base.Direction = value;
                CalibrateFoV();
            }
        }

        public static event Action<Trainer> OnDialogueFinished;

        private void Start()
        {
            CalibrateFoV();
        }

        public IEnumerator TriggerTrainerBattle(PlayerController player)
        {
            exclamation.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            exclamation.SetActive(false);

            var offset = player.transform.position - transform.position;
            yield return Move(Speed.Walk, (offset - offset.normalized).Round());

            player.LookTowardsPosition(transform.position);
            yield return OpenDialogue(() =>
            {
                OnDialogueFinished?.Invoke(trainer);
            });
        }

        private void CalibrateFoV()
        {
        }

        public void DestroyObjects()
        {
            Destroy(fov.gameObject);
            Destroy(exclamation);
        }

        IEnumerator IInteractable.Interact(Transform initiator)
        {
            LookTowardsPosition(initiator.position);

            if (trainer.CanFight && initiator.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.DisableInput();
            }
            else
            {
                yield return OpenDialogue();
            }
        }

        void ISavable.RestoreState(object state)
        {
            base.RestoreState(state);
            if (!trainer.CanFight)
            {
                DestroyObjects();
            }
        }
    }
}
