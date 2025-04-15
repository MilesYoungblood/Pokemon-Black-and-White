using System;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public sealed class Item : ISavable
    {
        public const int MinQuantity = 0;

        public const int MaxQuantity = 99;

        [SerializeField] private ItemAsset asset;

        [SerializeField] [Range(MinQuantity, MaxQuantity)]
        private int quantity;

        public ItemAsset Asset
        {
            get => asset;
            private set => asset = value;
        }

        public int Quantity
        {
            get => quantity;
            set => quantity = Mathf.Clamp(value, MinQuantity, MaxQuantity);
        }

        public Item(string name, int quantity)
        {
            Init(name, quantity);
        }

        private void Init(string name, int n)
        {
            Asset = ItemAsset.GetBaseByName(name);
            Quantity = n;
        }

        public override string ToString()
        {
            return Asset.name;
        }

        public object CaptureState()
        {
            return new object[] { Asset.name, quantity };
        }

        public void RestoreState(object state)
        {
            var objects = (object[])state;
            Init((string)objects[0], (int)objects[1]);
        }
    }
}
