using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    public class AspearBerry : Berry, IHoldable
    {
        public IEnumerator Listen(Pokemon pokemon)
        {
            yield return new WaitUntil(() => pokemon.StatusCondition is Freeze);
        }

        public void Use(Pokemon pokemon)
        {
            pokemon.StatusCondition = null;
        }
    }
}