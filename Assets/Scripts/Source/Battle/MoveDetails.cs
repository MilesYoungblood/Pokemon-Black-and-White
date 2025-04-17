using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    public sealed class MoveDetails : MonoBehaviour
    {
        [SerializeField] private BattleUnit playerUnit;

        [SerializeField] private Selector moveSelector;

        [SerializeField] private TextMeshProUGUI pp;

        [SerializeField] private TextMeshProUGUI type;

        private void Awake()
        {
            moveSelector.OnUpdate += Update;
            enabled = false;
        }

        private void OnDestroy()
        {
            moveSelector.OnUpdate -= Update;
        }

        private void Update()
        {
            var move = playerUnit.Pokemon[moveSelector.Selection];
            pp.text = $"PP: {move.PP.ToString()}/{move.MaxPP.ToString()}";
            type.text = move.Asset.Type.ToString();
        }
    }
}
