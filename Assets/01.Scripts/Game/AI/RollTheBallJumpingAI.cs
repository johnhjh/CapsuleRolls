using Capsule.Game.RollTheBall;
using System.Collections;
using UnityEngine;

namespace Capsule.Game.AI
{
    public class RollTheBallJumpingAI : MonoBehaviour
    {
        private PlayerRollTheBallMove ballMove = null;
        public float timeDelayed = 2f;
        private float timeBetTime = 0f;

        private void Start()
        {
            if (TryGetComponent(out PlayerRollTheBallMove move))
                ballMove = move;
            else
                ballMove = null;
            if (ballMove != null)
                StartCoroutine(AIJump());
        }

        private IEnumerator AIJump()
        {
            WaitForSeconds ws02 = new WaitForSeconds(0.2f);
            while (!ballMove.IsDead)
            {
                yield return ws02;
                if (GameManager.Instance != null)
                {
                    if (ballMove.IsLanded)
                    {
                        if (Time.time > timeBetTime)
                        {
                            ballMove.AIJump();
                            timeBetTime = Time.time + timeDelayed;
                        }
                    }
                }
                InfiniteLoopDetector.Run();
            }
        }
    }
}
