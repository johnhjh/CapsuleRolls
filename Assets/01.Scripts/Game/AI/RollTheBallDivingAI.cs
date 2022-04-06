using Capsule.Game.RollTheBall;
using System.Collections;
using UnityEngine;

namespace Capsule.Game.AI
{
    public class RollTheBallDivingAI : MonoBehaviour
    {
        private PlayerRollTheBallMove ballMove = null;
        public Transform targetTransform = null;
        public Transform Target
        {
            get { return targetTransform; }
            set { targetTransform = value; }
        }

        private float minRange = 1.5f;
        private float maxRange = 12f;

        private void OnEnable()
        {
            if (Target == null)
                Target = GameObject.Find("RollTheBallPlayer").transform.GetChild(0);
        }

        private void Start()
        {
            if (TryGetComponent(out PlayerRollTheBallMove move))
            {
                ballMove = move;
                StartCoroutine(AIDive());
            }
            else
                ballMove = null;
        }

        private IEnumerator AIDive()
        {
            WaitForSeconds ws01 = new WaitForSeconds(0.1f);
            while (true)
            {
                InfiniteLoopDetector.Run();
                if (ballMove.IsDead)
                {
                    if (GameManager.Instance != null &&
                        GameManager.Instance.CurrentGameData.Mode == Entity.GameMode.STAGE &&
                        GameManager.Instance.IsGameOver)
                        yield break;
                    else
                    {
                        yield return ws01;
                        continue;
                    }
                }
                if (GameManager.Instance != null)
                {
                    if (!GameManager.Instance.IsGameReady)
                    {
                        yield return ws01;
                        continue;
                    }
                    if (ballMove.IsLanded && CheckFoundEnemy())
                        ballMove.AIDive();
                }
                yield return ws01;
            }
        }

        private bool CheckFoundEnemy()
        {
            Ray ray = new Ray(transform.position + minRange * transform.forward + Vector3.up, transform.forward);
            int layerMask = 1 << LayerMask.NameToLayer(GameManager.Instance.tagData.TAG_PLAYER);

            RaycastHit[] hits = Physics.RaycastAll(ray, maxRange, layerMask);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag(GameManager.Instance.tagData.TAG_PLAYER))
                {
                    if (hit.collider.transform == targetTransform)
                        return true;
                }
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 255f, 0f, 1f);
            Gizmos.DrawLine(transform.position + minRange * transform.forward,
                transform.position + maxRange * transform.forward);
        }
    }
}

