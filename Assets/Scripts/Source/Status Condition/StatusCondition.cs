using System;

namespace Scripts.Source
{
    public abstract class StatusCondition
    {
        public enum ID
        {
            None,
            Burn,
            Paralysis,
            Freeze,
            Poison,
            BadPoison,
            Sleep
        }

        protected int Counter { get; private set; }

        public abstract string InflictMessage { get; }

        public abstract string CureMessage { get; }

        protected StatusCondition() : this(int.MaxValue)
        {
        }

        protected StatusCondition(int counter)
        {
            Counter = counter;
        }

        public abstract ID GetID();

        public abstract void HandlePreTurn(BattleUnit unit, out string message, out bool canMove);

        public void Update()
        {
            if (Counter != int.MaxValue)
            {
                --Counter;
            }
        }

        public static StatusCondition GetConditionByID(ID id)
        {
            return id switch
            {
                ID.None => null,
                ID.Burn => new Burn(),
                ID.Paralysis => new Paralysis(),
                ID.Freeze => new Freeze(),
                ID.Poison => new Poison(),
                ID.BadPoison => new BadPoison(),
                ID.Sleep => new Sleep(),
                _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
            };
        }
    }
}
