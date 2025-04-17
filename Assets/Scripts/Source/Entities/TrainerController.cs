using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Trainer))]
    public sealed class TrainerController : EntityController, IInteractable, ISavable
    {
        [SerializeField] private Trainer trainer;

        [SerializeField] private Transform fov;

        public override Vector2Int Direction
        {
            get => base.Direction;
            set
            {
                base.Direction = value;
                CalibrateFOV();
            }
        }

        public Trainer Trainer => trainer;

        protected override void Awake()
        {
            base.Awake();
            CalibrateFOV();
        }

        private void CalibrateFOV()
        {
            fov.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg);
        }

        IEnumerator IInteractable.Interact(PlayerController playerController)
        {
            LookAt = playerController.transform.position;

            if (Trainer.CanFight)
            {
                playerController.ActionMap = "Dialogue";
            }
            else if (TryGetComponent<MessageTrigger>(out var message))
            {
                yield return message.OpenDialogue();
            }
        }

        void ISavable.RestoreState(object state)
        {
            base.RestoreState(state);
            if (!Trainer.CanFight)
            {
                transform.DestroyChildren();
            }
        }
    }
}
