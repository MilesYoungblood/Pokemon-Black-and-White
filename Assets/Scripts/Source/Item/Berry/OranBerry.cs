using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    public class OranBerry : Berry, IHoldable
    {
        public IEnumerator Listen(Pokemon pokemon)
        {
            yield return new WaitUntil(() => pokemon.HP <= pokemon.MaxHP - 10);
        }

        public void Use(Pokemon pokemon)
        {
            pokemon.HP += 10;
        }
    }
}