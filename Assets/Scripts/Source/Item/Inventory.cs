using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using Scripts.Utility.Algorithm;
using UnityEngine;

namespace Scripts.Source
{
    public class Inventory : MonoBehaviour, ISavable
    {
        [SerializeField] private Item[] inventory;

        private readonly Dictionary<System.Type, ItemPocket> _pockets = new();

        public void Init(params System.Type[] pockets)
        {
            // this means we have already set the pockets in RestoreState
            if (_pockets.Any(pocket => pocket.Value.Count > 0))
            {
                return;
            }

            // add each pocket from the initializer
            foreach (var pocket in pockets)
            {
                _pockets.Add(pocket, new ItemPocket());
            }

            // add each item into its respective pocket
            Array.ForEach(inventory, AddItem);
            inventory = null;
        }

        public void AddItem(Item item)
        {
            if (item.Asset is Medicine)
            {
                if (!_pockets[typeof(Medicine)].TryAdd(item.Asset, item))
                {
                    _pockets[typeof(Medicine)][item.Asset].Quantity += item.Quantity;
                }
            }
            else if (!_pockets[item.Asset.GetType()].TryAdd(item.Asset, item))
            {
                _pockets[item.Asset.GetType()][item.Asset].Quantity += item.Quantity;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void RemoveItem<T>(ItemAsset id)
        {
            if (!_pockets[typeof(T)].Remove(id))
            {
                throw new Exception("Item was not successfully removed.");
            }
        }

        public void UseItem<T>(ItemAsset id)
        {
            if (--_pockets[typeof(T)][id].Quantity == Item.MinQuantity)
            {
                RemoveItem<T>(id);
            }
        }

        public ItemPocket GetPocket(System.Type type)
        {
            return _pockets[type];
        }

        // ReSharper disable once UnusedMember.Local
        private void SortPocket<T>()
        {
            _pockets[typeof(T)].ToArray().MergeSort((x, y) => string.Compare(x.Key.ToString(), y.Key.ToString(), StringComparison.Ordinal));
        }

        public object CaptureState()
        {
            var pockets = new string[_pockets.Keys.Count];
            {
                var pocketList = _pockets.Keys.ToArray();
                for (var i = 0; i < pockets.Length; ++i)
                {
                    pockets[i] = pocketList[i].AssemblyQualifiedName;
                }
            }

            var items = new List<List<object>>();
            foreach (var pocket in _pockets)
            {
                items.AddRange(pocket.Value.Select(item => new List<object> { item.Key.name, item.Value.Quantity }));
            }

            return new object[] { pockets, items };
        }

        public void RestoreState(object state)
        {
            var data = (object[])state;
            var pockets = (string[])data[0];

            // add each pocket from the initializer
            foreach (var pocket in pockets)
            {
                _pockets.Add(System.Type.GetType(pocket) ?? throw new InvalidOperationException(), new ItemPocket());
            }

            // now we can add the stored items
            var items = (List<List<object>>)data[1];
            foreach (var item in items)
            {
                AddItem(new Item((string)item[0], (int)item[1]));
            }
        }
    }
}
