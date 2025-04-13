using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class TrainerFOV : MonoBehaviour, ITriggerable
    {
        [SerializeField] private TrainerController trainerController;

        public void OnTrigger(PlayerController playerController)
        {
            if (GameController.Instance.CurrentState != GameController.State.Overworld)
            {
                return;
            }

            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySound("Spotted");

            playerController.DisableInput();

            StartCoroutine(trainerController.TriggerTrainerBattle(playerController));
        }
    }
}
