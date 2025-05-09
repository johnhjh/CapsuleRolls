﻿using Capsule.Game.RollTheBall;
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
            {
                ballMove = move;
                StartCoroutine(AIJump());
            }
            else
                ballMove = null;
        }

        private void OnEnable()
        {
            timeBetTime = Time.time + 2.5f;
        }

        private IEnumerator AIJump()
        {
            WaitForSeconds ws02 = new WaitForSeconds(0.2f);
            while (true)
            {
                InfiniteLoopDetector.Run();
                if ((ballMove.IsDead))
                {
                    if (GameManager.Instance != null &&
                        GameManager.Instance.CurrentGameData.Mode == Entity.GameMode.STAGE &&
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
                    if (!GameManager.Instance.IsGameReady)
                    {
                        yield return ws02;
                        continue;
                    }
                    if (GameManager.Instance.IsGameOver)
                        yield break;
                    if (ballMove.IsLanded)
                    {
                        if (Time.time > timeBetTime)
                        {
                            ballMove.AIJump();
                            timeBetTime = Time.time + timeDelayed;
                        }
                    }
                }
            }
        }
    }
}
