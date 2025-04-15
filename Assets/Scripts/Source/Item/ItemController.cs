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
            if (!gameObject.activeSelf)
            {
                yield break;
            }

            yield return AudioManager.Instance.PlayFanfare(item.Asset.SongID);
            //GameController.Instance.TogglePause();
            yield return GameController.Instance.DialogueBox.ShowDialogue(new Dialogue($"You obtained {item.GetIndefiniteArticle()} {item}!"));

            playerController.Player.Inventory.AddItem(item);
            item = null;
            gameObject.SetActive(false);
            //GameController.Instance.TogglePause();
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
