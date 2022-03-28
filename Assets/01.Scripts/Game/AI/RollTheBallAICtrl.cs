using Capsule.Game.Effect;
using Capsule.Game.RollTheBall;
using UnityEngine;

namespace Capsule.Game.AI
{
    public enum AIType
    {
        IDLE = 0,
        JUMPING,
        MOVING,
    }

    public class RollTheBallAICtrl : MonoBehaviour
    {
        private AIType aiType = AIType.IDLE;
        public AIType Type
        {
            get { return aiType; }
            set
            {
                aiType = value;
                if (jumpingAI == null)
                {
                    jumpingAI = transform.GetChild(0).GetComponent<RollTheBallJumpingAI>();
                    jumpingAI.timeDelayed = Random.Range(1.5f, 4f);
                }
                if (movingAI == null)
                    movingAI = transform.GetChild(0).GetComponent<RollTheBallMovingAI>();
                switch (value)
                {
                    case AIType.IDLE:
                        jumpingAI.enabled = false;
                        movingAI.enabled = false;
                        break;
                    case AIType.JUMPING:
                        jumpingAI.enabled = true;
                        movingAI.enabled = false;
                        break;
                    case AIType.MOVING:
                        jumpingAI.enabled = false;
                        movingAI.enabled = true;
                        break;
                }
            }
        }
        private RollTheBallJumpingAI jumpingAI;
        private RollTheBallMovingAI movingAI;

        public void RespawnEnemy(Vector3 position, Quaternion rotation)
        {
            if (transform.GetChild(0).TryGetComponent(out PlayerRollTheBallMove ballMove))
            {
                Transform ballTransform = transform.GetChild(2);
                if (ballTransform.gameObject.activeSelf)
                {
                    EffectQueueManager.Instance.ShowExplosionEffect(transform.GetChild(2).position);
                    transform.GetChild(2).gameObject.SetActive(false);
                }
                StartCoroutine(ballMove.PortalSpawn(position, rotation));
            }
        }
    }
}

