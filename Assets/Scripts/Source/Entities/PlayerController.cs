using Scripts.Utility.Math;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source
{
    [RequireComponent(typeof(Player))]
    public class PlayerController : CharacterController
    {
        private Speed _currentSpeed;

        private PlayerInput _input;

        public Player Player { get; private set; }

        protected void Awake()
        {
            _input = new PlayerInput();
            Player = GetComponent<Player>();
        }

        private void Start()
        {
            EnableInput();
        }

        private void FixedUpdate()
        {
            if (!IsCenteredOnTile() || GameController.Instance.CurrentState is GameController.State.Paused)
            {
                return;
            }

            var input = _input.Overworld.Move.ReadValue<Vector2>().ClampToAxis();

            // the zero vector denotes no movement
            if (input == Vector2.zero)
            {
                return;
            }

            Direction = Vector2Int.RoundToInt(input);
            if (IsPathClear())
            {
                StartCoroutine(Move(_currentSpeed, input, OnMoveOver));
            }
        }

        public void HandleRun(InputAction.CallbackContext context)
        {
            if (context.started || context.performed || context.canceled)
            {
                _currentSpeed = context.canceled ? Speed.Walk : Speed.Run;
            }
        }

        private void OnMoveOver()
        {
            var collision = Physics2D.OverlapPoint(
                transform.position,
                LayerMask.GetMask("Encounter", "Portal", "FOV")
            );

            if (collision && collision.TryGetComponent<ITriggerable>(out var trigger))
            {
                trigger.OnTrigger(this);
            }
        }

        public void HandleFace(InputAction.CallbackContext context)
        {
            if (!Animator.GetBool(IsMoving) && context.started)
            {
                Direction = Vector2Int.RoundToInt(context.ReadValue<Vector2>().ClampToAxis());
            }
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (!context.performed || Animator.GetBool(IsMoving))
            {
                return;
            }

            var collision = GetCollider2DInFront(LayerMask.GetMask("Interact"));
            if (collision && collision.TryGetComponent<IInteractable>(out var interactable))
            {
                StartCoroutine(interactable.Interact(transform));
            }
        }

        public void HandlePause(InputAction.CallbackContext context)
        {
            if (context.performed && !Animator.GetBool(IsMoving))
            {
                GameController.Instance.OpenMenu();
            }
        }

        public void DisableInput()
        {
            _input.Disable();
        }

        public void EnableInput()
        {
            _input.Enable();
        }
    }
}
