using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    public class PechaBerry : Berry, IHoldable
    {
        public IEnumerator Listen(Pokemon pokemon)
        {
            yield return new WaitUntil(() => pokemon.StatusCondition is Poison);
        }

        public void Use(Pokemon pokemon)
        {
            pokemon.StatusCondition = null;
        }
    }
}