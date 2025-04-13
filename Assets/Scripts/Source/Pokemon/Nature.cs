using System;

namespace Scripts.Source
{
    public static class Nature
    {
        public enum ID
        {
            Hardy,
            Lonely,
            Brave,
            Adamant,
            Naughty,
            Bold,
            Docile,
            Relaxed,
            Impish,
            Lax,
            Timid,
            Hasty,
            Serious,
            Jolly,
            Naive,
            Modest,
            Mild,
            Quiet,
            Bashful,
            Rash,
            Calm,
            Gentle,
            Sassy,
            Careful,
            Quirky
        }

        private static (Stat, Stat) GetStat(ID nature)
        {
            return nature switch
            {
                ID.Hardy => (Stat.Attack, Stat.Attack),
                ID.Lonely => (Stat.Attack, Stat.Defense),
                ID.Brave => (Stat.Attack, Stat.Speed),
                ID.Adamant => (Stat.Attack, Stat.SpAttack),
                ID.Naughty => (Stat.Attack, Stat.SpDefense),
                ID.Bold => (Stat.Defense, Stat.Attack),
                ID.Docile => (Stat.Defense, Stat.Defense),
                ID.Relaxed => (Stat.Defense, Stat.Speed),
                ID.Impish => (Stat.Defense, Stat.SpAttack),
                ID.Lax => (Stat.Defense, Stat.SpDefense),
                ID.Timid => (Stat.Speed, Stat.Attack),
                ID.Hasty => (Stat.Speed, Stat.Defense),
                ID.Serious => (Stat.Speed, Stat.Speed),
                ID.Jolly => (Stat.Speed, Stat.SpAttack),
                ID.Naive => (Stat.Speed, Stat.SpDefense),
                ID.Modest => (Stat.SpAttack, Stat.Attack),
                ID.Mild => (Stat.SpAttack, Stat.Defense),
                ID.Quiet => (Stat.SpAttack, Stat.Speed),
                ID.Bashful => (Stat.SpAttack, Stat.SpAttack),
                ID.Rash => (Stat.SpAttack, Stat.SpDefense),
                ID.Calm => (Stat.SpDefense, Stat.Attack),
                ID.Gentle => (Stat.SpDefense, Stat.Defense),
                ID.Sassy => (Stat.SpDefense, Stat.Speed),
                ID.Careful => (Stat.SpDefense, Stat.SpAttack),
                ID.Quirky => (Stat.SpDefense, Stat.SpDefense),
                _ => throw new ArgumentOutOfRangeException(nameof(nature), nature, null)
            };
        }

        public static Stat BoostedStat(ID nature)
        {
            return GetStat(nature).Item1;
        }

        public static Stat LoweredStat(ID nature)
        {
            return GetStat(nature).Item2;
        }
    }
}
