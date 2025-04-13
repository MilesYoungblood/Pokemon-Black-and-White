using Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source.UI
{
    public class MovesPage : MonoBehaviour
    {
        private Selector _moveSet;

        private Image[] _typeIcons;
        private TextMeshProUGUI[] _moveNames;
        private TextMeshProUGUI[] _ppAmounts;

        private void Awake()
        {
            _moveSet = transform.Find("Moveset").GetComponent<Selector>();

            var childCount = _moveSet.transform.childCount;
            _typeIcons = new Image[childCount];
            _moveNames = new TextMeshProUGUI[childCount];
            _ppAmounts = new TextMeshProUGUI[childCount];

            for (var i = 0; i < childCount; ++i)
            {
                var move = _moveSet.transform.GetChild(i);
                _typeIcons[i] = move.Find("Type").GetComponent<Image>();
                _moveNames[i] = move.Find("Name").GetComponent<TextMeshProUGUI>();
                _ppAmounts[i] = move.Find("PP Amount").GetComponent<TextMeshProUGUI>();
            }
        }

        public void Init(Pokemon pokemon)
        {
            for (var i = 0; i < _moveSet.transform.childCount; ++i)
            {
                {
                    var move = _moveSet.transform.GetChild(i).gameObject;
                    move.SetActive(i < pokemon.MoveSet.Count);
                    if (!move.activeSelf)
                    {
                        continue;
                    }
                }
                {
                    var move = pokemon[i];
                    _typeIcons[i].sprite = Pokedex.Instance.GetTypeIcon(move.Asset.Type);
                    _moveNames[i].text = move.ToString();
                    _ppAmounts[i].text = $"{move.PP}/{move.MaxPP}";
                }
            }
        }
    }
}
