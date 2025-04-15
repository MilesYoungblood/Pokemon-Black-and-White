using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using Scripts.Utility.Math;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class InventoryScreen : MonoBehaviour
    {
        [SerializeField] [Min(0)] private int itemsInViewport;

        [SerializeField] private ItemSlotUI itemSlotUI;

        [SerializeField] private TextMeshProUGUI pocket;

        [SerializeField] private TextMeshProUGUI effect;

        [SerializeField] private Image itemIcon;

        [SerializeField] private GameObject upArrow;

        [SerializeField] private GameObject downArrow;

        [SerializeField] private GameObject leftArrow;

        [SerializeField] private GameObject rightArrow;

        [SerializeField] private RectTransform itemList;

        // list representation of itemList GameObject
        private ItemSlotUI[] _itemSlotUIs;

        // contains all item pockets
        // this is initialized once per opening of inventory screen
        private List<Item>[] _sets;

        private string[] _pocketTexts;

        private Action[] _setCallbacks;

        private Action _onCancel;

        private int _currentSet;

        private int _selection;

        private InventoryScreenInput _inventoryScreenInput;

        public ItemAsset CurrentAsset => _sets[_currentSet][_selection].Asset;

        private void Awake()
        {
            _inventoryScreenInput = new InventoryScreenInput();
            _inventoryScreenInput.Default.Move.performed += Move;
            _inventoryScreenInput.Default.Submit.performed += Submit;
            _inventoryScreenInput.Default.Cancel.performed += Cancel;
        }

        private void OnDestroy()
        {
            _inventoryScreenInput.Default.Move.performed -= Move;
            _inventoryScreenInput.Default.Submit.performed -= Submit;
            _inventoryScreenInput.Default.Cancel.performed -= Cancel;
        }

        private void OnEnable()
        {
            _inventoryScreenInput.Default.Enable();
        }

        private void OnDisable()
        {
            _sets = null;
            _currentSet = 0;
            Clean();

            _inventoryScreenInput.Default.Disable();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void InitItemSlotUIs(int set)
        {
            _itemSlotUIs = new ItemSlotUI[_sets[set].Count];
            for (var i = 0; i < _itemSlotUIs.Length; ++i)
            {
                _itemSlotUIs[i] = Instantiate(itemSlotUI, itemList).Init(_sets[set][i]);
            }
        }

        public void Init(Inventory inventory, Action onCancel, params (System.Type, Action)[] sets)
        {
            gameObject.SetActive(true);
            _sets = new List<Item>[sets.Length];
            _setCallbacks = new Action[sets.Length];
            _pocketTexts = new string[sets.Length];

            // Add each set passed
            for (var i = 0; i < sets.Length; ++i)
            {
                _sets[i] = inventory.GetItems(sets[i].Item1);

                // add each pocket text
                _pocketTexts[i] = sets[i].Item1.Name;

                // Add each set function
                // Each set will carry one common function
                _setCallbacks[i] = sets[i].Item2;
            }

            InitItemSlotUIs(0);

            pocket.text = _pocketTexts.First();
            var multiple = _pocketTexts.Length > 1;
            leftArrow.SetActive(multiple);
            rightArrow.SetActive(multiple);

            _onCancel = onCancel;

            // required for the initial rendering
            UpdateSelection();
        }

        private void Clean()
        {
            itemList.DestroyChildren();

            // TODO this might not be necessary
            foreach (var itemSlotUi in _itemSlotUIs)
            {
                Destroy(itemSlotUi.gameObject);
            }

            _itemSlotUIs.Initialize();
            _itemSlotUIs = null;

            _selection = 0;
        }

        private void Init()
        {
            // safe clean the previous list
            Clean();

            // re-add the item uis
            InitItemSlotUIs(_currentSet);

            // render the initial selection
            UpdateSelection();
        }

        private void ChangeSet(bool next)
        {
            if (next)
            {
                _currentSet.ModuloIncrement(_sets.Length);
            }
            else
            {
                _currentSet.ModuloDecrement(_sets.Length);
            }

            pocket.text = _pocketTexts[_currentSet];

            if (_sets.Length > 1)
            {
                Init();
            }
        }

        private void Scroll()
        {
            var size = itemsInViewport / 2;
            if (_itemSlotUIs.Length > itemsInViewport)
            {
                itemList.localPosition = new Vector3(
                    itemList.localPosition.x,
                    Mathf.Clamp(_selection - size, 0, _selection) * _itemSlotUIs[_selection].Height
                );
            }

            upArrow.SetActive(_selection > size);
            downArrow.SetActive(_selection + size < _itemSlotUIs.Length);
        }

        private void UpdateSelection()
        {
            if (_itemSlotUIs.Length > 0)
            {
                for (var i = 0; i < _itemSlotUIs.Length; ++i)
                {
                    _itemSlotUIs[i].SetSelected(i == _selection, true);
                }

                var item = _sets[_currentSet][_selection].Asset;
                itemIcon.sprite = item.Sprite;
                effect.text = item.Effect;

                Scroll();
            }
            else
            {
                itemIcon.sprite = null;
                effect.text = string.Empty;
                upArrow.SetActive(false);
                downArrow.SetActive(false);
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            var input = context.ReadValue<Vector2>();
            if (input == Vector2.left)
            {
                ChangeSet(true);
            }
            else if (input == Vector2.right)
            {
                ChangeSet(false);
            }
            else if (_sets[_currentSet].Any())
            {
                if (input == Vector2.up)
                {
                    _selection.ModuloDecrement(_sets[_currentSet].Count);
                    UpdateSelection();
                }
                else if (input == Vector2.down)
                {
                    _selection.ModuloIncrement(_sets[_currentSet].Count);
                    UpdateSelection();
                }
            }
        }

        public void Submit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _setCallbacks[_currentSet]?.Invoke();
            }
        }

        public void Cancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _onCancel?.Invoke();
            }
        }
    }
}
