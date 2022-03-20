using Capsule.Game.RollTheBall;
using UnityEngine;

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
                if (ballMove.IsDead) return;
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
