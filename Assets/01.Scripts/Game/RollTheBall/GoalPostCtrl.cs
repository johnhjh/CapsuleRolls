using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class GoalPostCtrl : MonoBehaviour
    {
        public bool isTeamA = false;
        private void OnTriggerEnter(Collider other)
        {
            /*
            if (other.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
            {

            }
            */
        }
    }
}
