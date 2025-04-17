using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class ItemController : MonoBehaviour, IInteractable, ISavable
    {
        [SerializeField] private Item item;

        public IEnumerator Interact(PlayerController playerController)
        {
            yield return AudioManager.Instance.PlayFanfare(item.Asset.SongID);
            yield return GameController.Instance.MessageBox.Print(new Message($"You obtained {item.GetIndefiniteArticle()} {item}!"));

            playerController.Player.Inventory.AddItem(item);
            item = null;
            gameObject.SetActive(false);
        }

        public object CaptureState()
        {
            return gameObject.activeSelf;
        }

        public void RestoreState(object state)
        {
            gameObject.SetActive((bool)state);
        }
    }
}
