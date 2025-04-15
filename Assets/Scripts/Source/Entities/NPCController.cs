using System;
using System.Collections;
using Scripts.Utility;
using Scripts.Utility.Math;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class NPCController : EntityController, IInteractable
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
            [SerializeField] [_4DirectionalVector] private Vector2Int direction;

            [SerializeField] [Min(0)] private int displacement;

            [SerializeField] [Min(0.0f)] private float waitTime;

            public Vector2Int Direction => direction;

            public int Displacement => displacement;

            public float WaitTime => waitTime;
        }

        [SerializeField] private Dialogue dialogue;

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
            _idleTimer -= movementPattern[_currentMovement].WaitTime;
        }

        private IEnumerator Walk()
        {
            _currentState = State.Walking;

            Direction = movementPattern[_currentMovement].Direction;

            // continue walking until we have traveled the current movement pattern's displacement amount
            for (var remaining = movementPattern[_currentMovement].Displacement; remaining > 0; --remaining)
            {
                if (!IsPathClear())
                {
                    _currentState = State.Blocked;
                    yield return new WaitUntil(IsPathClear);
                    _currentState = State.Walking;
                }

                yield return Move(Speed.Walk, (Vector2)Direction);
            }

            _currentMovement.ModuloIncrement(movementPattern.Length);
            _currentState = State.Idle;
        }

        public IEnumerator OpenDialogue(Action onFinished = null)
        {
            _currentState = State.Interacting;
            onFinished ??= SetIdle;
            yield return GameController.Instance.DialogueBox.ShowDialogue(dialogue);
            onFinished.Invoke();
        }

        private void SetIdle()
        {
            _currentState = State.Idle;
        }

        public IEnumerator Interact(PlayerController playerController)
        {
            if (_currentState is not (State.Idle or State.Blocked))
            {
                yield break;
            }

            LookAt = playerController.transform.position;
            yield return OpenDialogue();
        }
    }
}
