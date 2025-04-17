using System.Collections;
using Scripts.Utility.Math;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EntityController))]
    public class MovementAI : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Interacting,
            Walking,
            Blocked
        }

        [SerializeField] private EntityController entityController;

        [SerializeField] private MovementPattern[] movementPattern;

        private float _idleTimer;

        private int _currentMovement;

        public EntityController EntityController => entityController;

        public State CurrentState { get; set; }

        private void Awake()
        {
            if (movementPattern.Length is 0)
            {
                Destroy(this);
            }
        }

        private void FixedUpdate()
        {
            if (GameController.Instance.CurrentState is not (GameController.State.Overworld or GameController.State.Dialogue))
            {
                return;
            }

            if (CurrentState is State.Interacting or not State.Idle)
            {
                return;
            }

            _idleTimer += Time.fixedDeltaTime;
            if (_idleTimer < movementPattern[_currentMovement].WaitTime)
            {
                return;
            }

            StartCoroutine(Walk());
            _idleTimer -= movementPattern[_currentMovement].WaitTime;
        }

        private IEnumerator Walk()
        {
            CurrentState = State.Walking;

            entityController.Direction = movementPattern[_currentMovement].Direction;

            // continue walking until we have traveled the current movement pattern's displacement amount
            for (var remaining = movementPattern[_currentMovement].Displacement; remaining > 0; --remaining)
            {
                if (!entityController.IsPathClear())
                {
                    CurrentState = State.Blocked;
                    yield return new WaitUntil(entityController.IsPathClear);
                    CurrentState = State.Walking;
                }

                yield return entityController.Move(EntityController.Speed.Walk, (Vector2)entityController.Direction);
            }

            _currentMovement.ModuloIncrement(movementPattern.Length);
            SetIdle();
        }

        public void SetIdle()
        {
            CurrentState = State.Idle;
        }
    }
}
