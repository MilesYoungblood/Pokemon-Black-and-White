using System;
using System.Collections;
using Cinemachine.Utility;
using DG.Tweening;
using JetBrains.Annotations;
using Scripts.Utility;
using Scripts.Utility.Math;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public abstract class EntityController : MonoBehaviour, ISavable
    {
        public enum Speed
        {
            Walk,
            Run
        }

        [SerializeField] private Animator animator;

        [SerializeField] [_4DirectionalVector] private Vector2Int direction = Vector2Int.down;

        private static readonly int LookX = Animator.StringToHash("LookX");

        private static readonly int LookY = Animator.StringToHash("LookY");

        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        protected virtual Vector2Int Direction
        {
            get => direction;
            set
            {
                value.Clamp(-Vector2Int.one, Vector2Int.one);
                if (value.sqrMagnitude is not 1)
                {
                    return;
                }

                direction = value;
                animator.SetFloat(LookX, value.x);
                animator.SetFloat(LookY, value.y);
            }
        }

        public Vector3 LookAt
        {
            set => Direction = Vector2Int.RoundToInt(value.Floor() - transform.position.Floor());
        }

        private static float GetSpeed(Speed speed)
        {
            return speed switch
            {
                Speed.Walk => 5.0f,
                Speed.Run => 7.5f,
                _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
            };
        }

        public IEnumerator Move(Speed speed, Vector3 moveVector, Action onMoveOver = null)
        {
            animator.SetBool(IsMoving, true);

            var start = transform.position;
            var end = start + moveVector;

            yield return transform.DOMove(
                end,
                Vector3.Distance(start, end) / GetSpeed(speed)
            ).SetEase(Ease.Linear).WaitForCompletion();

            onMoveOver?.Invoke();

            animator.SetBool(IsMoving, false);
        }

        [CanBeNull]
        protected Collider2D GetCollider2DInFront(int layerMask)
        {
            return Physics2D.OverlapPoint((Vector2)transform.position + Direction, layerMask);
        }

        public bool IsCenteredOnTile()
        {
            return transform.position.Abs().Mod(1.0f) == Vector3.up / 2.0f;
        }

        protected bool IsPathClear()
        {
            return !GetCollider2DInFront(LayerMask.GetMask("Collision", "Interact"));
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
