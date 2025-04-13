using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scripts.Source.UI
{
    [DisallowMultipleComponent]
    public class InventoryScreen : MonoBehaviour
    {
        [SerializeField] private ItemSlotUI itemSlotUI;

        [SerializeField] private TextMeshProUGUI pocketText;

        [SerializeField] private TextMeshProUGUI effectText;

        [SerializeField] private Image itemIcon;

        private GameObject _upArrow;

        private GameObject _downArrow;

        private GameObject _leftArrow;

        private GameObject _rightArrow;

        private Transform _itemList;

        private RectTransform _itemListRect;

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

        private static int ItemsInViewport => 6;

        public ItemAsset CurrentAsset => _sets[_currentSet][_selection].Asset;

        private void Awake()
        {
            _upArrow = transform.Find("Up Arrow").gameObject;
            _downArrow = transform.Find("Down Arrow").gameObject;
            _leftArrow = transform.Find("Left Arrow").gameObject;
            _rightArrow = transform.Find("Right Arrow").gameObject;
            _itemList = transform.Find("Item View/Viewport/Item List");
            _itemListRect = _itemList.GetComponent<RectTransform>();

            _inventoryScreenInput = new InventoryScreenInput();
            _inventoryScreenInput.Default.Select.performed += HandleSelect;
            _inventoryScreenInput.Default.Accept.performed += HandleAccept;
            _inventoryScreenInput.Default.Cancel.performed += HandleCancel;
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

            _leftArrow.SetActive(true);
            _rightArrow.SetActive(true);

            _inventoryScreenInput.Default.Disable();
        }

        private void OnDestroy()
        {
            _inventoryScreenInput.Default.Select.performed -= HandleSelect;
            _inventoryScreenInput.Default.Accept.performed -= HandleAccept;
            _inventoryScreenInput.Default.Cancel.performed -= HandleCancel;
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
                _itemSlotUIs[i] = Instantiate(itemSlotUI, _itemList).Init(_sets[set][i]);
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
                _sets[i] = inventory.GetPocket(sets[i].Item1).Values.ToList();

                // add each pocket text
                _pocketTexts[i] = sets[i].Item1.ToString();

                // Add each set function
                // Each set will carry one common function
                _setCallbacks[i] = sets[i].Item2;
            }

            InitItemSlotUIs(0);

            pocketText.text = _pocketTexts.First();
            if (_pocketTexts.Length == 1)
            {
                _leftArrow.SetActive(false);
                _rightArrow.SetActive(false);
            }

            _onCancel = onCancel;

            // required for the initial rendering
            UpdateSelection();
        }

        private void Clean()
        {
            // clear the itemList GameObject of its children
            foreach (Transform child in _itemList)
            {
                Destroy(child.gameObject);
            }

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
                _currentSet = (_currentSet + 1) % _sets.Length;
            }
            else
            {
                var setCount = _sets.Length;
                _currentSet = (_currentSet - 1 + setCount) % setCount;
            }

            pocketText.text = _pocketTexts[_currentSet];

            if (_sets.Length > 1)
            {
                Init();
            }
        }

        private void HandleScrolling()
        {
            var size = ItemsInViewport / 2;
            if (_itemSlotUIs.Length > ItemsInViewport)
            {
                var scrollPosition = Mathf.Clamp(_selection - size, 0, _selection) * _itemSlotUIs[_selection].Height;
                _itemListRect.localPosition = new Vector3(_itemListRect.localPosition.x, scrollPosition);
            }

            _upArrow.SetActive(_selection > size);
            _downArrow.SetActive(_selection + size < _itemSlotUIs.Length);
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
                effectText.text = item.Effect;

                HandleScrolling();
            }
            else
            {
                itemIcon.sprite = null;
                effectText.text = string.Empty;
                _upArrow.SetActive(false);
                _downArrow.SetActive(false);
            }
        }

        public void HandleSelect(InputAction.CallbackContext context)
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
            else if (_sets[_currentSet].Count > 0)
            {
                if (input == Vector2.up)
                {
                    var currentSetCount = _sets[_currentSet].Count;
                    _selection = (_selection - 1 + currentSetCount) % currentSetCount;
                    UpdateSelection();
                }
                else if (input == Vector2.down)
                {
                    _selection = (_selection + 1) % _sets[_currentSet].Count;
                    UpdateSelection();
                }
            }
        }

        public void HandleAccept(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _setCallbacks[_currentSet]?.Invoke();
            }
        }

        public void HandleCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _onCancel?.Invoke();
            }
        }
    }
}
