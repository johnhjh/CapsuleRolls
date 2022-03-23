using Capsule.Entity;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class GoalInCtrl : MonoBehaviour
    {
        public bool isTeamA = false;
        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.Instance != null)
            {
                if (other.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
                {
                    if (GameManager.Instance.CurrentGameData.Mode == GameMode.STAGE)
                    {
                        GameManager.Instance.StageClear();
                        return;
                    }
                    if (other.transform.TryGetComponent<RollingBallRotate>(out RollingBallRotate rollingBall))
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
}
