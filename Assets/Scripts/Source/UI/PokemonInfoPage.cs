using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source.UI
{
    public class PokemonInfoPage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dex;

        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private Image type1;

        [SerializeField] private Image type2;

        [SerializeField] private TextMeshProUGUI trainer;

        [SerializeField] private TextMeshProUGUI id;

        public void Init(Player player, Pokemon pokemon)
        {
            dex.text = pokemon.Asset.GetDexAsString(true);

            name.text = pokemon.Asset.name;
            type1.sprite = Pokedex.Instance.GetTypeIcon(pokemon.Asset.Type1);
            {
                var comparison = pokemon.Asset.Type2 != Type.ID.None;

                type2.sprite = comparison ? Pokedex.Instance.GetTypeIcon(pokemon.Asset.Type2) : null;
                type2.gameObject.SetActive(comparison);
            }

            trainer.text = player.ToString();
            id.text = "12345";
        }
    }
}
