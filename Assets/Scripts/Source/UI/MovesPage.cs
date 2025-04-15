using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class MovesPage : MonoBehaviour
    {
        [SerializeField] private RectTransform moveset;

        [SerializeField] private Image[] typeIcons;

        [SerializeField] private TextMeshProUGUI[] moveNames;

        [SerializeField] private TextMeshProUGUI[] ppAmounts;

        public Pokemon Pokemon
        {
            set
            {
                for (var i = 0; i < moveset.childCount; ++i)
                {
                    {
                        var move = moveset.GetChild(i).gameObject;
                        move.SetActive(i < value.MoveSet.Count);
                        if (!move.activeSelf)
                        {
                            continue;
                        }
                    }
                    {
                        var move = value[i];
                        typeIcons[i].sprite = Pokedex.Instance.GetTypeIcon(move.Asset.Type);
                        moveNames[i].text = move.ToString();
                        ppAmounts[i].text = $"{move.PP}/{move.MaxPP}";
                    }
                }
            }
        }

        private void Awake()
        {
            enabled = false;
        }
    }
}
