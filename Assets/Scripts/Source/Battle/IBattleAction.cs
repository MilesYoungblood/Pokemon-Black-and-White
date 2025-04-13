using System.Collections;

namespace Scripts.Source
{
    public interface IBattleAction
    {
        public byte Priority { get; }

        public static bool HandleSamePriorityBase(BattleUnit user, BattleUnit opponent)
        {
            return user.Pokemon.RivalsInSpeed(opponent.Pokemon)
                ? Utility.Math.Statistics.BernoulliTrial()
                : user.Pokemon.IsFasterThan(opponent.Pokemon);
        }

        public bool HandleSamePriority(BattleUnit user, BattleUnit opponent);

        public IEnumerator Use(
            BattleSystem battleSystem,
            BattleUnit user,
            BattleUnit opponent,
            BattleDialogueBox battleDialogueBox);
    }
}
