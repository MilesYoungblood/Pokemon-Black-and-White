using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Source
{
    public class CheriBerry : Berry, IHoldable
    {
        public IEnumerator Listen(Pokemon pokemon)
        {
            yield return new WaitUntil(() => pokemon.StatusCondition is Paralysis);
        }

        public override List<string> Use(Pokemon pokemon)
        {
            pokemon.StatusCondition = null;
            return new List<string>();
        }
    }
}