using System;
using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    public class NPCController : CharacterController, IInteractable
    {
        private enum State
        {
            Idle,
            Interacting,
            Walking,
            Blocked
        }

        [Serializable]
        private struct Movement
        {
            [SerializeField, _4DirectionalVector] private Vector2Int direction;

            [SerializeField, Min(0)] private int displacement;

            [SerializeField, Min(0.0f)] private float waitTime;

            public Vector2Int Direction => direction;

            public int Displacement => displacement;

            public float WaitTime => waitTime;
        }

        [SerializeField] private UI.Dialogue dialogue;

        [SerializeField] private Movement[] movementPattern;

        private float _idleTimer;
        private int _currentMovement;
        private State _currentState;

        protected virtual void Awake()
        {
            enabled = movementPattern.Length > 0;
        }

        private void FixedUpdate()
        {
            if (GameController.Instance.CurrentState is not (GameController.State.Overworld or GameController.State.Dialogue))
            {
                return;
            }

            if (_currentState is State.Interacting or not State.Idle)
            {
                return;
            }

            _idleTimer += Time.fixedDeltaTime;
            if (_idleTimer < movementPattern[_currentMovement].WaitTime)
            {
                return;
            }

            StartCoroutine(Walk());
            _idleTimer = 0.0f;
        }

        private IEnumerator Walk()
        {
            _currentState = State.Walking;

            // store the remaining tiles left
            var remaining = Mathf.Abs(movementPattern[_currentMovement].Displacement);

            // store the direction
            var direction = movementPattern[_currentMovement].Direction;

            Direction = direction;

            // continue walking until we have traveled the current movement pattern's displacement amount
            while (remaining > 0)
            {
                if (!IsPathClear())
                {
                    _currentState = State.Blocked;
                    yield return new WaitUntil(IsPathClear);
                    _currentState = State.Walking;
                }

                yield return Move(Speed.Walk, (Vector2)direction);

                --remaining;
            }

            _currentMovement = (_currentMovement + 1) % movementPattern.Length;
            _currentState = State.Idle;
        }

        protected IEnumerator OpenDialogue(Action onFinished = null)
        {
            _currentState = State.Interacting;
            onFinished ??= SetIdle;
            yield return UI.DialogueBox.Instance.ShowDialogue(dialogue);
            onFinished.Invoke();
        }

        private void SetIdle()
        {
            _currentState = State.Idle;
        }

        public IEnumerator Interact(Transform initiator)
        {
            if (_currentState is not (State.Idle or State.Blocked))
            {
                yield break;
            }

            LookTowardsPosition(initiator.position);
            yield return OpenDialogue();
        }
    }
}
