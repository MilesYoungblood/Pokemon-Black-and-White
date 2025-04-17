using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player), typeof(PlayerInput))]
    public class PlayerController : EntityController
    {
        public const string OverworldMapping = "Overworld";

        public const string DialogueMapping = "Dialogue";

        public const string UISelectionMapping = "UI Selection";

        [SerializeField] private Player player;

        [SerializeField] private PlayerInput input;

        private Speed _currentSpeed;

        public Player Player => player;

        public PlayerActions Actions { get; private set; }

        public string ActionMap
        {
            get => input.currentActionMap.name;
            set => input.SwitchCurrentActionMap(value);
        }

        protected override void Awake()
        {
            base.Awake();
            Actions = new PlayerActions();
            Actions.Enable();
        }

        private void FixedUpdate()
        {
            if (!CanAct())
            {
                return;
            }

            var vector = Actions.Overworld.Move.ReadValue<Vector2>();

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
            if (!CanAct())
            {
                return;
            }

            if (context.started)
            {
                Direction = Vector2Int.RoundToInt(context.ReadValue<Vector2>());
            }
            else if (context.performed && IsPathClear())
            {
                StartCoroutine(Move(_currentSpeed, (Vector2)Direction, OnMoveOver));
            }
        }

        public void Face(InputAction.CallbackContext context)
        {
#if DEBUG
            Debug.Log($"Context: {context.phase}, Value: {context.ReadValue<Vector2>().ToString()}");
#endif
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
            Actions.Enable();
        }

        private bool CanAct()
        {
            return IsCenteredOnTile() && ActionMap is OverworldMapping && GameController.Instance.CurrentState is not GameController.State.Paused;
        }

        /// <summary>
        /// Subscribes the selector's callback's to the PlayerController's UI Selection input actions.
        /// </summary>
        /// <param name="playerController">The <see cref="PlayerController"/>.</param>
        /// <param name="selector">the <see cref="Selector"/>.</param>
        /// <returns>The <see cref="PlayerController"/>.</returns>
        public static PlayerController operator +(PlayerController playerController, Selector selector)
        {
            playerController.Actions.UISelection.Move.performed += selector.Move;
            playerController.Actions.UISelection.Submit.performed += selector.Submit;
            playerController.Actions.UISelection.Cancel.performed += selector.Cancel;
            return playerController;
        }

        /// <summary>
        /// Unsubscribes the selector's callback's to the PlayerController's UI Selection input actions.
        /// </summary>
        /// <param name="playerController">The <see cref="PlayerController"/>.</param>
        /// <param name="selector">the <see cref="Selector"/>.</param>
        /// <returns>The <see cref="PlayerController"/>.</returns>
        public static PlayerController operator -(PlayerController playerController, Selector selector)
        {
            playerController.Actions.UISelection.Move.performed -= selector.Move;
            playerController.Actions.UISelection.Submit.performed -= selector.Submit;
            playerController.Actions.UISelection.Cancel.performed -= selector.Cancel;
            return playerController;
        }
    }
}
