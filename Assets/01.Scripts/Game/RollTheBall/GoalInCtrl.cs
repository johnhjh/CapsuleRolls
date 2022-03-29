using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.Effect;
using System.Collections;
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
                        StartCoroutine(FireWorks(transform.parent));
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

        private IEnumerator FireWorks(Transform goalPostTransform)
        {
            while (GameManager.Instance.IsGameOver)
            {
                Vector3 fireWorkPosition = Random.Range(8.0f, 11.0f) * Vector3.up
                    + Random.Range(-10.0f, 10.0f) * Vector3.right
                    + goalPostTransform.position;
                SFXManager.Instance.PlayOneShot(GameSFX.FIREWORK, fireWorkPosition, 10f);
                if (EffectQueueManager.Instance != null)
                    EffectQueueManager.Instance.ShowFireworkEffect(fireWorkPosition);
                yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
                InfiniteLoopDetector.Run();
            }
        }
    }
}
