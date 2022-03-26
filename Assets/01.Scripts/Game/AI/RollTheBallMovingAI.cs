using Capsule.Game.Base;
using Capsule.Game.RollTheBall;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.AI
{
    public class RollTheBallMovingAI : MonoBehaviour
    {
        private PlayerRollTheBallMove ballMove = null;
        private MoveInput moveInput = null;
        public List<Transform> wayPoints;
        public int currentWayPoint = 0;

        private void Start()
        {
            InitAIMove();
        }

        private void InitAIMove()
        {
            if (TryGetComponent(out MoveInput input))
                moveInput = input;
            else
                moveInput = null;
            if (TryGetComponent(out PlayerRollTheBallMove move))
                ballMove = move;
            else
                ballMove = null;
            if (moveInput != null &&
                ballMove != null &&
                wayPoints != null &&
                wayPoints.Count > 0)
            {
                foreach(Transform wayPoint in wayPoints)
                    wayPoint.position = new Vector3(wayPoint.position.x, transform.position.y, wayPoint.position.z);
                StartCoroutine(AIMove());
            }
        }

        private IEnumerator AIMove()
        {
            WaitForSeconds ws01 = new WaitForSeconds(0.1f);
            while (!ballMove.IsDead)
            {
                if (GameManager.Instance != null)
                {
                    if (ballMove.IsLanded)
                    {
                        if (CheckArrival())
                        {
                            moveInput.vertical = 0f;
                            ChangeWayPoint();
                        }
                        else
                        {
                            if (CheckRotation())
                            {
                                moveInput.rotate = 0f;
                                moveInput.vertical = 1f;
                            }
                            else
                            {
                                moveInput.rotate = 0.6f;
                                moveInput.vertical = 0f;
                            }
                        }
                    }
                }
                yield return ws01;
                InfiniteLoopDetector.Run();
            }
        }

        private bool CheckArrival()
        {
            //Debug.Log("AI remained dist : " + Vector3.Distance(wayPoints[currentWayPoint].position, transform.position));
            return Vector3.Distance(wayPoints[currentWayPoint].position, transform.position) <= 5f;
        }

        private bool CheckRotation()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            int layerMask = 1 << LayerMask.NameToLayer(GameManager.Instance.tagData.TAG_WAY_POINT);

            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag(GameManager.Instance.tagData.TAG_WAY_POINT))
                    if (hit.collider.transform == wayPoints[currentWayPoint])
                        return true;
            }

            return false;
        }

        private void ChangeWayPoint()
        {
            currentWayPoint = (++currentWayPoint) % wayPoints.Count;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 0f, 0f, 1f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100f);
        }
    }
}