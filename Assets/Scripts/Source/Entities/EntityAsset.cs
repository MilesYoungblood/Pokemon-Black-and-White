using UnityEngine;

namespace Scripts.Source
{
    [CreateAssetMenu(menuName = "Entity/Create new Entity")]
    public class EntityAsset : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorController;

        [SerializeField] private Sprite battleSprite;

        [SerializeField] private AudioClip eyesMeetClip;

        public AnimatorOverrideController AnimatorController => animatorController;

        public Sprite BattleSprite => battleSprite;

        public AudioClip EyesMeetClip => eyesMeetClip;
    }
}
