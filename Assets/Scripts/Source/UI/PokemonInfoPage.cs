using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class PokemonInfoPage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dex;

        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private Image type1;

        [SerializeField] private Image type2;

        [SerializeField] private TextMeshProUGUI trainer;

        [SerializeField] private TextMeshProUGUI id;

        private void Awake()
        {
            enabled = false;
        }

        public void Init(Player player, Pokemon pokemon)
        {
            dex.text = pokemon.Asset.GetDexAsString(true);

            name.text = pokemon.Asset.name;
            type1.sprite = Pokedex.Instance.GetTypeIcon(pokemon.Asset.Type1);
            type2.gameObject.SetActive(pokemon.Asset.Type2 is not Type.ID.None);
            type2.sprite = type2.gameObject.activeSelf ? Pokedex.Instance.GetTypeIcon(pokemon.Asset.Type2) : null;

            trainer.text = player.ToString();
            id.text = "00000";
        }
    }
}
