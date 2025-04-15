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
            Inventory.Pockets = new[]
            {
                typeof(BattleItem),
                typeof(Berry),
                typeof(HoldItem),
                typeof(Machine),
                typeof(Medicine),
                typeof(PokeBall)
            };
        }

        void IBattler.HandleFaint()
        {
        }

        void IBattler.HandleNextPokemon()
        {
            OnNextPokemon?.Invoke();
        }

        bool IBattler.CanFight => Party.Any(pokemon => pokemon.CanFight);

        string IBattler.Prefix => string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }
}
