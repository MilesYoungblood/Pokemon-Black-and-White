using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Trainer))]
    public sealed class TrainerController : NPCController, IInteractable, ISavable
    {
        [SerializeField] private Trainer trainer;

        [SerializeField] private Transform fov;

        protected override Vector2Int Direction
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

            if (trainer.CanFight)
            {
                playerController.ActionMap = "Dialogue";
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
                transform.DestroyChildren();
            }
        }
    }
}
