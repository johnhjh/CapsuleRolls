using Capsule.Game.Base;
using Capsule.Game.RollTheBall;
using System.Collections;
using UnityEngine;

namespace Capsule.Game.AI
{
    public class RollTheBallRotatingAI : MonoBehaviour
    {
        private PlayerRollTheBallMove ballMove = null;
        private MoveInput moveInput = null;

        private void Start()
        {
            if (TryGetComponent(out MoveInput input))
                moveInput = input;
            if (TryGetComponent(out PlayerRollTheBallMove move))
                ballMove = move;
            if (moveInput && ballMove != null)
                StartCoroutine(AIRotate());
        }

        private IEnumerator AIRotate()
        {
            WaitForSeconds ws02 = new WaitForSeconds(0.2f);
            while (true)
            {
                if ((ballMove.IsDead))
                {
                    if (GameManager.Instance != null &&
                        GameManager.Instance.IsGameOver)
                        yield break;
                    else
                    {
                        yield return ws02;
                        continue;
                    }
                }
                yield return ws02;
                if (GameManager.Instance != null)
                {
                    if (ballMove.IsLanded)
                        moveInput.rotate = 0.6f;
                    else
                        moveInput.rotate = 0f;
                }
                InfiniteLoopDetector.Run();
            }
        }
    }
}