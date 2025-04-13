using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Source
{
    public class Character : MonoBehaviour
    {
        protected enum Class
        {
            [UsedImplicitly] AceTrainer,
            [UsedImplicitly] Artist,
            [UsedImplicitly] Backer,
            [UsedImplicitly] Backpacker,
            [UsedImplicitly] Baker,
            [UsedImplicitly] BattleGirl,
            [UsedImplicitly] Beauty,
            [UsedImplicitly] Biker,
            [UsedImplicitly] BlackBelt,
            [UsedImplicitly] Clerk,
            [UsedImplicitly] Cyclist,
            [UsedImplicitly] Dancer,
            [UsedImplicitly] Doctor,
            [UsedImplicitly] Fisherman,
            [UsedImplicitly] Gentleman,
            [UsedImplicitly] Harlequin,
            [UsedImplicitly] Hiker,
            [UsedImplicitly] Hoopster,
            [UsedImplicitly] Infielder,
            [UsedImplicitly] Janitor,
            [UsedImplicitly] Lady,
            [UsedImplicitly] Lass,
            [UsedImplicitly] Linebacker,
            [UsedImplicitly] Maid,
            [UsedImplicitly] Musician,
            [UsedImplicitly] Nurse,
            [UsedImplicitly] NurseryAide,
            [UsedImplicitly] ParasolLady,
            [UsedImplicitly] Pilot,
            [UsedImplicitly] Pokefan,
            [UsedImplicitly] PokemonBreeder,
            [UsedImplicitly] PokemonRanger,
            [UsedImplicitly] PokemonTrainer,
            [UsedImplicitly] Policeman,
            [UsedImplicitly] Preschooler,
            [UsedImplicitly] Psychic,
            [UsedImplicitly] RichBoy,
            [UsedImplicitly] Roughneck,
            [UsedImplicitly] SchoolKid,
            [UsedImplicitly] Scientist,
            [UsedImplicitly] Smasher,
            [UsedImplicitly] Socialite,
            [UsedImplicitly] Striker,
            [UsedImplicitly] Swimmer,
            [UsedImplicitly] Twin,
            [UsedImplicitly] Veteran,
            [UsedImplicitly] Waiter,
            [UsedImplicitly] Waitress,
            [UsedImplicitly] Worker,
            [UsedImplicitly] WorkerIce,
            [UsedImplicitly] Youngster
        }

        [SerializeField] private Class @class;

        [SerializeField] private new string name;

        protected Class CharacterClass => @class;

        protected string Name => name;
    }
}
