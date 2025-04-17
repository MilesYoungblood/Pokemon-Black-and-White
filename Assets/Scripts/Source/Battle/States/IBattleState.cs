using System.Collections;

namespace Scripts.Source
{
    public interface IBattleState
    {
        public IEnumerator Enter(BattleSystem battleSystem);
    }
}
