using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    public class TrainerFOV : MonoBehaviour, ITriggerable
    {
        private TrainerController _trainerController;

        private void Awake()
        {
            _trainerController = GetComponentInParent<TrainerController>();
        }

        public void OnTrigger(PlayerController playerController)
        {
            if (GameController.Instance.CurrentState != GameController.State.Overworld)
            {
                return;
            }

            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySound("Spotted");

            playerController.DisableInput();

            StartCoroutine(_trainerController.TriggerTrainerBattle(playerController));
        }
    }
}
