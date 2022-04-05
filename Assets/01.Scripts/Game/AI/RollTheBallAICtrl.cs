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
        ROTATING,
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
                if (rotatingAI == null)
                    rotatingAI = transform.GetChild(0).GetComponent<RollTheBallRotatingAI>();
                jumpingAI.enabled = false;
                movingAI.enabled = false;
                rotatingAI.enabled = false;
                switch (value)
                {
                    case AIType.IDLE:
                        break;
                    case AIType.JUMPING:
                        jumpingAI.enabled = true;
                        break;
                    case AIType.MOVING:
                        movingAI.enabled = true;
                        break;
                    case AIType.ROTATING:
                        rotatingAI.enabled = true;
                        break;
                }
            }
        }
        private RollTheBallJumpingAI jumpingAI;
        private RollTheBallMovingAI movingAI;
        private RollTheBallRotatingAI rotatingAI;

        public void ChangeBallColor(Material mat)
        {
            if (transform.GetChild(2).TryGetComponent(out MeshRenderer enemyBallMesh))
            {
                Material[] mats = enemyBallMesh.materials;
                mats[0] = mat;
                enemyBallMesh.materials = mats;
            }
        }

        public void RespawnEnemy(Vector3 position, Quaternion rotation)
        {
            if (transform.GetChild(0).TryGetComponent(out PlayerRollTheBallMove ballMove))
            {
                /*
                Transform ballTransform = transform.GetChild(2);
                if (ballTransform.gameObject.activeSelf)
                {
                    EffectQueueManager.Instance.ShowExplosionEffect(transform.GetChild(2).position);
                    transform.GetChild(2).gameObject.SetActive(false);
                }
                */
                StartCoroutine(ballMove.PortalSpawn(position, rotation));
            }
        }
    }
}

