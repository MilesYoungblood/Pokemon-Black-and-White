using System;
using System.Collections.Generic;
using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source.UI
{
    [DisallowMultipleComponent]
    public class PartyScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI message;

        private Selector _partyList;

        private PartyMemberUI[] _members;

        public int Selection => _partyList.Selection;

        public BattleSystem.State? CalledFrom { get; set; }

        public Action Callbacks
        {
            set
            {
                gameObject.SetActive(true);
                message.text = "Choose a Pokemon";

                // TODO if this breaks, try using player party size instead of Trainer.MaxPartySize
                var callbacks = new Action[Trainer.MaxPartySize];
                for (var i = 0; i < callbacks.Length; ++i)
                {
                    callbacks[i] = value;
                }

                _partyList.Callbacks = callbacks;
            }
        }

        private void OnEnable()
        {
            // for whatever reason, this is necessary
            _partyList.gameObject.SetActive(true);
            for (var i = 0; i < _partyList.transform.childCount; ++i)
            {
                _members[i].gameObject.SetActive(i < GameController.Instance.PlayerController.Player.Party.Count);
                if (_members[i].gameObject.activeSelf)
                {
                    _members[i].Init(GameController.Instance.PlayerController.Player.Party[i]);
                }
            }
        }

        private void Awake()
        {
            _partyList = GetComponentInChildren<Selector>();
            _members = _partyList.GetComponentsInChildren<PartyMemberUI>();
        }

        public void Init(Action onCancel)
        {
            _partyList.OnCancel += onCancel;
        }

        public void Destroy(Action onCancel)
        {
            _partyList.OnCancel -= onCancel;
        }

        public void SetMessageText(string newMessage)
        {
            message.text = newMessage;
        }

        public void Open(List<Pokemon> party)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
