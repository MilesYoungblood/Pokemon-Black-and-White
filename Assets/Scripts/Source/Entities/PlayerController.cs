using Scripts.Utility;
using Scripts.Utility.Math;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player), typeof(PlayerInput))]
    public class PlayerController : EntityController
    {
        [SerializeField] private Player player;

        [SerializeField] private PlayerInput input;

        private Speed _currentSpeed;

        public PlayerActions Actions { get; private set; }

        public Player Player => player;

        public string ActionMap
        {
            get => input.currentActionMap.name;
            set => input.SwitchCurrentActionMap(value);
        }

        protected void Awake()
        {
            Actions = new PlayerActions();
            Actions.Enable();
        }

        private void Start()
        {
            EnableInput();
        }

        private void FixedUpdate()
        {
            if (!CanAct())
            {
                return;
            }

            var vector = Actions.Overworld.Move.ReadValue<Vector2>().ClampToAxis();

            // the zero vector denotes no movement
            if (vector == Vector2.zero)
            {
                return;
            }

            Direction = Vector2Int.RoundToInt(vector);
            if (IsPathClear())
            {
                StartCoroutine(Move(_currentSpeed, vector, OnMoveOver));
            }
        }

        private void OnMoveOver()
        {
            var collision = Physics2D.OverlapPoint(
                transform.position,
                LayerMask.GetMask("Encounter")
            );

            if (collision && collision.TryGetComponent<ITriggerable>(out var trigger))
            {
                trigger.OnTrigger();
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            if (!CanAct())
            {
                return;
            }

            var vector = Vector2Int.RoundToInt(context.ReadValue<Vector2>());

            // the zero vector denotes no movement
            if (vector == Vector2Int.zero)
            {
                return;
            }

            Direction = vector;
            if (IsPathClear())
            {
                StartCoroutine(Move(_currentSpeed, (Vector2)vector, OnMoveOver));
            }
        }

        public void Face(InputAction.CallbackContext context)
        {
            if (context.started && CanAct())
            {
                Direction = Vector2Int.RoundToInt(context.ReadValue<Vector2>());
            }
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (!context.performed || !CanAct())
            {
                return;
            }

            var collision = GetCollider2DInFront(LayerMask.GetMask("Interact"));
            if (collision && collision.TryGetComponent<IInteractable>(out var interactable))
            {
                StartCoroutine(interactable.Interact(this));
            }
        }

        public void Run(InputAction.CallbackContext context)
        {
            _currentSpeed = context.canceled ? Speed.Walk : Speed.Run;
        }

        public void EnableInput()
        {
            Actions.Overworld.Enable();
        }

        private bool CanAct()
        {
            return IsCenteredOnTile() && input.currentActionMap.name is "Overworld" && GameController.Instance.CurrentState is not GameController.State.Paused;
        }

        public static PlayerController operator +(PlayerController playerController, Selector selector)
        {
            playerController.Actions.UISelection.Move.performed += selector.Move;
            playerController.Actions.UISelection.Submit.performed += selector.Submit;
            playerController.Actions.UISelection.Cancel.performed += selector.Cancel;
            return playerController;
        }

        public static PlayerController operator -(PlayerController playerController, Selector selector)
        {
            playerController.Actions.UISelection.Move.performed -= selector.Move;
            playerController.Actions.UISelection.Submit.performed -= selector.Submit;
            playerController.Actions.UISelection.Cancel.performed -= selector.Cancel;
            return playerController;
        }
    }
}
