using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    public class RawstBerry : Berry, IHoldable
    {
        public IEnumerator Listen(Pokemon pokemon)
        {
            yield return new WaitUntil(() => pokemon.StatusCondition is Burn);
        }

        public void Use(Pokemon pokemon)
        {
            pokemon.StatusCondition = null;
        }
    }
}