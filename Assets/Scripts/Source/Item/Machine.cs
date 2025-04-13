using UnityEngine;

namespace Scripts.Source
{
    [CreateAssetMenu(menuName = "Item/Machine/Create new Machine")]
    public class Machine : ItemAsset
    {
        [SerializeField] private int number;

        [SerializeField] private MoveAsset move;

        public int Number => number;

        public MoveAsset Move => move;

        public override string SongID => "Disc 1/50 - Obtained a TM!";
    }
}
