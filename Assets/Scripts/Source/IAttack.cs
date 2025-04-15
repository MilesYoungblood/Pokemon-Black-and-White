using System;

namespace Scripts.Source
{
    public interface IAttack
    {
        public static float CalculateDamage(BattleUnit attacker, BattleUnit defender, Move.Category category, int power)
        {
            var damage = (2 * attacker.Pokemon.Level / 5.0f + 2) * power;
            switch (category)
            {
                case Move.Category.Physical:
                    damage *= attacker.Pokemon.Attack * attacker[Stat.Attack] /
                        defender.Pokemon.Defense * defender[Stat.Defense];
                    break;
                case Move.Category.Special:
                    damage *= attacker.Pokemon.SpAttack * attacker[Stat.SpAttack] /
                        defender.Pokemon.SpDefense * defender[Stat.SpDefense];
                    break;
                case Move.Category.Status:
                    return 0.0f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }

            damage /= 50.0f;
            damage += 2;
            return damage;
        }
    }
}
