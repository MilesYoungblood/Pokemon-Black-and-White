using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class ItemController : MonoBehaviour, IInteractable, ISavable
    {
        [SerializeField] private Item item;

        public IEnumerator Interact(Transform initiator)
        {
            if (!gameObject.activeSelf)
            {
                yield break;
            }

            yield return AudioManager.Instance.PlayFanfare(item.Asset.SongID);
            //GameController.Instance.TogglePause();
            yield return UI.DialogueBox.Instance.ShowDialogue(new UI.Dialogue($"You obtained {item.GetIndefiniteArticle()} {item}!"));

            if (!initiator.TryGetComponent<PlayerController>(out var playerController))
            {
                yield break;
            }

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
