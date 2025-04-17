using System;
using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class PartyScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI message;

        [SerializeField] private Selector partyList;

        [SerializeField] private GameObject pokemonHud;

        public int Selection => partyList.Selection;

        public BattleSystem.State? CalledFrom { get; set; }

        public Action Callbacks
        {
            set
            {
                this.Activate();

                // TODO if this breaks, try using player party size instead of Trainer.MaxPartySize
                partyList.Callbacks = new Action[Mathf.Min(GameController.Instance.PlayerController.Player.Party.Count, Trainer.MaxPartySize)];
                Array.Fill(partyList.Callbacks, value);
            }
        }

        public string Message
        {
            set => message.text = value;
        }

        private void OnEnable()
        {
            GameController.Instance.PlayerController += partyList;
            Message = "Choose a Pokemon";

            for (var i = 0; i < GameController.Instance.PlayerController.Player.Party.Count; ++i)
            {
                if (Instantiate(pokemonHud, partyList.transform).TryGetComponent<PokemonHUD>(out var hud))
                {
                    hud.Pokemon = GameController.Instance.PlayerController.Player[i];
                }
                else
                {
                    Debug.LogWarning($"{nameof(hud)} is missing component {nameof(PokemonHUD)}");
                }
            }
        }

        private void OnDisable()
        {
            GameController.Instance.PlayerController -= partyList;
            Message = "";
            partyList.transform.DestroyChildren();
        }

        public void Init(Action onCancel)
        {
            partyList.OnCancel += onCancel;
        }

        public void Destroy(Action onCancel)
        {
            partyList.OnCancel -= onCancel;
            this.Deactivate();
        }
    }
}
