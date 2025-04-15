using System;
using System.Collections.Generic;
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

        [SerializeField] private PartyMemberUI[] members;

        public int Selection => partyList.Selection;

        public BattleSystem.State? CalledFrom { get; set; }

        public Action Callbacks
        {
            set
            {
                gameObject.SetActive(true);
                message.text = "Choose a Pokemon";

                // TODO if this breaks, try using player party size instead of Trainer.MaxPartySize
                partyList.Callbacks = new Action[Trainer.MaxPartySize];
                for (var i = 0; i < Trainer.MaxPartySize; ++i)
                {
                    partyList.Callbacks[i] = value;
                }
            }
        }

        public List<Pokemon> Party
        {
            set => throw new NotImplementedException();
        }

        public string Message
        {
            set => message.text = value;
        }

        private void OnEnable()
        {
            GameController.Instance.PlayerController += partyList;
            // for whatever reason, this is necessary
            partyList.gameObject.SetActive(true);
            for (var i = 0; i < partyList.transform.childCount; ++i)
            {
                members[i].gameObject.SetActive(i < GameController.Instance.PlayerController.Player.Party.Count);
                if (members[i].gameObject.activeSelf)
                {
                    members[i].Pokemon = GameController.Instance.PlayerController.Player.Party[i];
                }
            }
        }

        private void OnDisable()
        {
            GameController.Instance.PlayerController -= partyList;
        }

        public void Init(Action onCancel)
        {
            partyList.OnCancel += onCancel;
        }

        public void Destroy(Action onCancel)
        {
            partyList.OnCancel -= onCancel;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
