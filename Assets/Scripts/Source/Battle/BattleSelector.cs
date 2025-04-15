using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public abstract class BattleSelector : MonoBehaviour
    {
        [SerializeField] private Selector selector;

        [SerializeField] private BattleUnit playerUnit;

        [SerializeField] private BattleUnit opponentUnit;

        [SerializeField] private BattleSystem battleSystem;

        [SerializeField] private BattleDialogueBox battleDialogueBox;

        protected Selector Selector => selector;

        protected BattleUnit PlayerUnit => playerUnit;

        protected BattleUnit OpponentUnit => opponentUnit;

        protected BattleSystem BattleSystem => battleSystem;

        protected BattleDialogueBox BattleDialogueBox => battleDialogueBox;
    }
}
