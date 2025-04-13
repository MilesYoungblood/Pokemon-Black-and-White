using System;
using System.Collections;
using Cinemachine.Utility;
using Scripts.Utility;
using Scripts.Utility.Math;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public abstract class CharacterController : MonoBehaviour, ISavable
    {
        protected enum Speed
        {
            Walk,
            Run
        }

        [SerializeField] private Animator animator;

        [SerializeField] [_4DirectionalVector] private Vector2Int direction = Vector2Int.down;

        private static readonly int LookX = Animator.StringToHash("LookX");

        private static readonly int LookY = Animator.StringToHash("LookY");

        protected static readonly int IsMoving = Animator.StringToHash("IsMoving");

        protected Animator Animator => animator;

        protected virtual Vector2Int Direction
        {
            get => direction;
            set
            {
                if (value == Vector2Int.zero)
                {
                    return;
                }

                direction = value;
                Animator.SetFloat(LookX, value.x);
                Animator.SetFloat(LookY, value.y);
            }
        }

        protected static float GetSpeed(Speed speed)
        {
            return speed switch
            {
                Speed.Walk => 5.0f,
                Speed.Run => 7.5f,
                _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
            };
        }

        protected IEnumerator Move(Speed speed, Vector3 moveVector, Action onMoveOver = null)
        {
            Animator.SetBool(IsMoving, true);

            var start = transform.position;
            var end = start + moveVector;

            var duration = Vector3.Distance(start, end.normalized) / GetSpeed(speed);
            for (var elapsed = 0.0f; elapsed < duration; elapsed += Time.fixedDeltaTime)
            {
                transform.position = Vector3.Lerp(start, end, elapsed / duration);
                yield return new WaitForFixedUpdate();
            }

            transform.position = end;

            onMoveOver?.Invoke();

            Animator.SetBool(IsMoving, false);
        }

        protected Collider2D GetCollider2DInFront(int layerMask)
        {
            return Physics2D.OverlapPoint((Vector2)transform.position + Direction, layerMask);
        }

        protected bool IsCenteredOnTile()
        {
            return transform.position.Abs().Mod(1.0f) == (Vector3)(Vector2.up / 2.0f);
        }

        protected bool IsPathClear()
        {
            return !GetCollider2DInFront(LayerMask.GetMask("Collision", "Interact"));
        }

        public void LookTowardsPosition(Vector3 targetPosition)
        {
            var diffVector = (Vector2)(targetPosition.Floor() - transform.position.Floor());
            if (diffVector.x is 0 || diffVector.y is 0)
            {
                Direction = Vector2Int.RoundToInt(diffVector);
            }
        }

        public object CaptureState()
        {
            return new[] { transform.position.x, transform.position.y, transform.position.z };
        }

        public void RestoreState(object state)
        {
            var position = (float[])state;
            transform.position = new Vector3(position[0], position[1], position[2]);
        }
    }
}
