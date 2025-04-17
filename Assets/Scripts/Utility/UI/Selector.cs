using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scripts.Utility
{
    [DisallowMultipleComponent]
    [UsedImplicitly]
    public class Selector : MonoBehaviour
    {
        private ISelectable[] _selectables;

        private List<Action>[] _onSubmitEvents;

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
                    _selectables[i].Highlight(Selection == i, IsSelectable?.Invoke(i) ?? true);
                }

                OnUpdate?.Invoke();
            }
        }

        public int NCallbacks
        {
            set => _onSubmitEvents = new List<Action>[value];
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

        [CanBeNull]
        public Func<int, bool> IsSelectable
        {
            get;
            [UsedImplicitly] set;
        }

        public event Action OnCancel;

        public event Action OnUpdate;

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

        private void OnDestroy()
        {
            OnCancel = null;
            OnUpdate = null;
        }

        private void OnTransformChildrenChanged()
        {
            _selectables = GetComponentsInChildren<ISelectable>();
        }

        public void ResetSelection()
        {
            Selection = 0;
        }

        public void Move(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            var input = context.ReadValue<Vector2>();

            if (input == Vector2.right)
            {
                if (Selection % _columnCount < _columnCount - 1 && Selection + 1 < transform.childCount &&
                    transform.GetChild(Selection + 1).gameObject.activeSelf)
                {
                    ++Selection;
                }
            }
            else if (input == Vector2.left)
            {
                if (Selection % _columnCount > 0 && transform.GetChild(Selection - 1).gameObject.activeSelf)
                {
                    --Selection;
                }
            }
            else if (input == Vector2.down)
            {
                if (Selection + _columnCount < transform.childCount &&
                    transform.GetChild(Selection + _columnCount).gameObject.activeSelf)
                {
                    Selection += _columnCount;
                }
            }
            else if (input == Vector2.up)
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

        public void AddCallback(int i, Action callback)
        {
            _onSubmitEvents[i].Add(callback);
        }

        public bool RemoveCallback(int i, Action callback)
        {
            return _onSubmitEvents[i].Remove(callback);
        }
    }
}
