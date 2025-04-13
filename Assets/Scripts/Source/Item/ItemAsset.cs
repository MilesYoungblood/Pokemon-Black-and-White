using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public abstract class ItemAsset : ScriptableObject, IBattleAction
    {
        [SerializeField, TextArea] private string effect;

        [SerializeField] private Sprite sprite;

        public string Effect => effect;

        public Sprite Sprite => sprite;

        public virtual string SongID => "Disc 1/31 - Obtained an Item!";

        public byte Priority => 2;

        public virtual List<string> Use(Pokemon pokemon)
        {
            return new List<string>();
        }

        private static readonly Dictionary<string, ItemAsset> Bases = new();

        public static void Init()
        {
            foreach (var itemBase in Resources.LoadAll<ItemAsset>("Items"))
            {
                if (!Bases.TryAdd(itemBase.name, itemBase))
                {
                    MonoBehaviour.print($"Unable to add Item Base {itemBase.name}");
                }
            }
        }

        public static ItemAsset GetBaseByName(string name)
        {
            return Bases[name];
        }

        public bool HandleSamePriority(BattleUnit user, BattleUnit opponent)
        {
            return IBattleAction.HandleSamePriorityBase(user, opponent);
        }

        public IEnumerator Use(
            BattleSystem battleSystem,
            BattleUnit user,
            BattleUnit opponent,
            BattleDialogueBox battleDialogueBox)
        {
            yield break;
        }
    }
}
