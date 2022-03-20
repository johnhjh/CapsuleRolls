using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class GoalInCtrl : MonoBehaviour
    {
        public bool isTeamA = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
            {
                if (other.transform.TryGetComponent<RollingBallCtrl>(out RollingBallCtrl rollingBall))
                {
                    if (rollingBall.isTeamA != this.isTeamA)
                    {
                        GameManager.Instance.AddScore(rollingBall.isTeamA, 1);
                    }
                }
            }
        }
    }
}
