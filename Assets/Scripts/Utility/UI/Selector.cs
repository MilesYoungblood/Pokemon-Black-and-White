using System;
using JetBrains.Annotations;
using Scripts.Utility.Math;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scripts.Utility
{
    public class Selector : MonoBehaviour
    {
        private ISelectable[] _selectables;

        private Action[] _callbacks;

        private int _columnCount;

        private int _selection;

        public int Selection
        {
            get => _selection;
            private set
            {
                _selection = value;
                var iterations = Mathf.Min(transform.childCount, _selectables.Length);
                for (var i = 0; i < iterations; ++i)
                {
                    _selectables[i].SetSelected(Selection == i, IsSelectable?.Invoke(i) ?? true);
                }

                OnChange?.Invoke();
            }
        }

        public Action[] Callbacks
        {
            get => _callbacks;
            set
            {
                var previousState = gameObject.activeSelf;
                gameObject.SetActive(true);

                _callbacks = value;
                for (var i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(i < _callbacks.Length);
                }

                Selection = 0;
                gameObject.SetActive(previousState);
            }
        }

        [CanBeNull] public Func<int, bool> IsSelectable { get; set; }

        public event Action OnCancel;

        public event Action OnChange;

        private void Awake()
        {
            OnTransformChildrenChanged();

            if (TryGetComponent<GridLayoutGroup>(out var gridLayoutGroup))
            {
                _columnCount = gridLayoutGroup.constraint switch
                {
                    GridLayoutGroup.Constraint.Flexible => throw new NotImplementedException(),
                    GridLayoutGroup.Constraint.FixedColumnCount => gridLayoutGroup.constraintCount,
                    GridLayoutGroup.Constraint.FixedRowCount => Mathf.CeilToInt(
                        transform.childCount / (float)gridLayoutGroup.constraintCount
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else if (TryGetComponent<VerticalLayoutGroup>(out _))
            {
                _columnCount = 1;
            }
            else if (TryGetComponent<HorizontalLayoutGroup>(out var horizontalLayoutGroup))
            {
                _columnCount = horizontalLayoutGroup.transform.childCount;
            }
            else
            {
                throw new Exception("No layout group component found.");
            }
        }

        private void OnDisable()
        {
            Selection = 0;
        }

        private void OnDestroy()
        {
            OnCancel = null;
            OnChange = null;
        }

        private void OnTransformChildrenChanged()
        {
            _selectables = GetComponentsInChildren<ISelectable>();
        }

        public void Move(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            var input = Vector2Int.RoundToInt(context.ReadValue<Vector2>().ClampToAxis());

            if (input == Vector2Int.right)
            {
                if (Selection % _columnCount < _columnCount - 1 && Selection + 1 < transform.childCount &&
                    transform.GetChild(Selection + 1).gameObject.activeSelf)
                {
                    ++Selection;
                }
            }
            else if (input == Vector2Int.left)
            {
                if (Selection % _columnCount > 0 && transform.GetChild(Selection - 1).gameObject.activeSelf)
                {
                    --Selection;
                }
            }
            else if (input == Vector2Int.down)
            {
                if (Selection + _columnCount < transform.childCount &&
                    transform.GetChild(Selection + _columnCount).gameObject.activeSelf)
                {
                    Selection += _columnCount;
                }
            }
            else if (input == Vector2Int.up)
            {
                if (Selection >= _columnCount && transform.GetChild(Selection - _columnCount).gameObject.activeSelf)
                {
                    Selection -= _columnCount;
                }
            }
            else
            {
                throw new Exception($"This branch should be inaccessible: input is {input}");
            }
        }

        public void Submit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Callbacks?[Selection]?.Invoke();
            }
        }

        public void Cancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnCancel?.Invoke();
            }
        }
    }
}
