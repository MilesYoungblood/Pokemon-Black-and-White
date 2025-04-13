using System;
using System.Linq;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Inventory))]
    public class Player : Trainer, IBattler
    {
        [SerializeField] private Inventory inventory;

        public Inventory Inventory => inventory;

        private int _money;

        public int Money
        {
            get => _money;
            set => _money = Mathf.Clamp(value, 0, 9999999);
        }

        public new static event Action OnNextPokemon;

        protected override void Start()
        {
            base.Start();
            Inventory.Init(typeof(BattleItem), typeof(Berry), typeof(HoldItem), typeof(Machine), typeof(Medicine), typeof(PokeBall));
        }

        public new void HandleFaint()
        {
        }

        public new void HandleNextPokemon()
        {
            OnNextPokemon?.Invoke();
        }

        public new bool CanFight => Party.Any(pokemon => pokemon.CanFight);

        public new string Prefix => string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }
}
