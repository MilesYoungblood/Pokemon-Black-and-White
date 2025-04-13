using System.Collections.Generic;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [CreateAssetMenu(menuName = "Move/Create new Move")]
    public class MoveAsset : ScriptableObject
    {
        [SerializeField] [TextArea] private string effect;

        [SerializeField] private Type.ID type;

        [SerializeField] private Move.Category category;

        [SerializeField] private int power;

        [SerializeField] [Min(0)] [DisableIf("sureHit")]
        private int accuracy;

        [SerializeField] private bool sureHit;

        [SerializeField, Min(0)] private int pp;

        [SerializeField, Range(-1, 1)] private sbyte priority;

        [SerializeField] private bool makesContact;

        [SerializeField] private StatEffect[] statEffects;

        [SerializeField] private StatusEffect statusEffect;

        [SerializeField] private VolatileStatusEffect volatileStatusEffect;

        public string Effect => effect;

        public Type.ID Type => type;

        public Move.Category Category => category;

        public int Power => power;

        public int Accuracy => accuracy;

        public bool SureHit => sureHit;

        public int PP => pp;

        public int MaxPP => (int)(PP * 1.6f);

        public sbyte Priority => priority;

        public bool MakesContact => makesContact;

        public StatEffect[] StatEffects => statEffects;

        public StatusEffect StatusEffect => statusEffect;

        public VolatileStatusEffect VolatileStatusEffect => volatileStatusEffect;

        private static readonly Dictionary<string, MoveAsset> Bases = new();

        public static void Init()
        {
            foreach (var moveBase in Resources.LoadAll<MoveAsset>("Moves"))
            {
                var name = moveBase.name;
                if (!Bases.TryAdd(name, moveBase))
                {
                    MonoBehaviour.print($"Unable to add Move Base {name}");
                }
            }
        }

        public static MoveAsset GetBaseByName(string name)
        {
            return Bases[name];
        }
    }
}
