using System;
using System.Collections;
using System.Globalization;
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
    public class EntityController : MonoBehaviour, ISavable
    {
        public enum Speed
        {
            Walk,
            Run
        }

        [SerializeField] private EntityAsset asset;

        [SerializeField] private Animator animator;

        [SerializeField] [_4DirectionalVector] private Vector2Int direction = Vector2Int.down;

        private static readonly int LookX = Animator.StringToHash("LookX");

        private static readonly int LookY = Animator.StringToHash("LookY");

        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        private static readonly int StepParity = Animator.StringToHash("Step Parity");

        public virtual Vector2Int Direction
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
            set => Direction = (Vector2Int)(Vector3Int.FloorToInt(value) - Vector3Int.FloorToInt(transform.position));
        }

        protected virtual void Awake()
        {
            if (asset)
            {
                animator.runtimeAnimatorController = asset.AnimatorController;
            }
        }

        private void OnValidate()
        {
            Awake();
        }

        public void SetStepParity(int value)
        {
            animator.SetInteger(StepParity, value.ModuloIncrement(2));

#if DEBUG
            Debug.Log($"Step parity set to {(value is 1).ToString()} at time {(animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f).ToString(CultureInfo.CurrentCulture)}");
#endif
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

        public bool IsPathClear()
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
