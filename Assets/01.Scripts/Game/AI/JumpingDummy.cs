using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Game.RollTheBall;

namespace Capsule.Game.AI
{
    public class JumpingDummy : MonoBehaviour
    {
        private PlayerRollTheBallMove ballMove = null;
        public float timeDelayed = 2f;
        private float timeBetTime = 0f;

        private void Start()
        {
            if (TryGetComponent<PlayerRollTheBallMove>(out PlayerRollTheBallMove move))
                ballMove = move;
            else
                ballMove = null;
        }

        private void Update()
        {
            if (ballMove != null)
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
        }
    }
}
