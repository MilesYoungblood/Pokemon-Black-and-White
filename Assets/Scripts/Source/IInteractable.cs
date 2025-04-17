using System.Collections;

namespace Scripts.Source
{
    public interface IInteractable
    {
        IEnumerator Interact(PlayerController initiator);
    }
}
