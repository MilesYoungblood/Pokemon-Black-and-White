using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    public class ChestoBerry : Berry, IHoldable
    {
        public IEnumerator Listen(Pokemon pokemon)
        {
            yield return new WaitUntil(() => pokemon.StatusCondition is Sleep);
        }

        public void Use(Pokemon pokemon)
        {
            pokemon.StatusCondition = null;
        }
    }
}