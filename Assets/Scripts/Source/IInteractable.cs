using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    public interface IInteractable
    {
        IEnumerator Interact(Transform initiator);
    }
}
