using Capsule.Util;
using Cinemachine;
using System.Collections;
using UnityEngine;

namespace Capsule.Game
{
    [System.Serializable]
    public class DeadCameraAction
    {
        [Header("Cam Move Setting")]
        public float distance = 8.0f;
        public float height = 15.0f;
        public float moveDamping = 3.0f;
        public float rotateDamping = 5.0f;
        public bool isActive = false;
        public IEnumerator SetCameraQuater(Transform camTransform, Transform targetTransform)
        {
            while (isActive)
            {
                camTransform.SetPositionAndRotation(
                    Vector3.Slerp(
                        camTransform.position,
                        -Vector3.forward * distance +
                        Vector3.up * height + targetTransform.position,
                        Time.deltaTime * moveDamping),
                    Quaternion.Slerp(
                        camTransform.rotation,
                        Quaternion.LookRotation(
                            (targetTransform.position - camTransform.position).normalized),
                        Time.deltaTime * rotateDamping));
                yield return new WaitForSeconds(0.02f);
                InfiniteLoopDetector.Run();
            }
        }
    }

    public class GameCameraManager : MonoBehaviour
    {
        private static GameCameraManager camMgr;
        public static GameCameraManager Instance
        {
            get
            {
                if (camMgr == null)
                    camMgr = FindObjectOfType<GameCameraManager>();
                return camMgr;
            }
        }

        public DeadCameraAction deadCameraAction;
        private Transform mainCameraTransform;
        private CinemachineVirtualCamera moveFollowCam;
        private Coroutine mainCamCoroutine = null;

        private Tuple<Transform, bool> target;
        public Tuple<Transform, bool> Target
        {
            get
            {
                if (target == null)
                {
                    target = new Tuple<Transform, bool>
                    {
                        First = moveFollowCam.Follow,
                        Second = false
                    };
                }
                return target;
            }
            set
            {
                target.First = value.First;
                target.Second = value.Second;
                if (target.Second)
                {
                    moveFollowCam.enabled = false;
                    deadCameraAction.isActive = true;
                    if (mainCamCoroutine != null)
                        StopCoroutine(mainCamCoroutine);
                    mainCamCoroutine = StartCoroutine(deadCameraAction.SetCameraQuater(mainCameraTransform, target.First));
                }
                else
                {
                    deadCameraAction.isActive = false;
                    if (mainCamCoroutine != null)
                    {
                        StopCoroutine(mainCamCoroutine);
                        mainCamCoroutine = null;
                    }
                    moveFollowCam.Follow = target.First;
                    moveFollowCam.LookAt = target.First;
                    moveFollowCam.enabled = true;
                }
            }
        }

        private void Awake()
        {
            target = new Tuple<Transform, bool>();
            deadCameraAction = new DeadCameraAction();
            mainCameraTransform = Camera.main.transform;
            moveFollowCam = GameObject.Find("MoveFollowCam").GetComponent<CinemachineVirtualCamera>();
        }

        public void CameraShake()
        {
            if (Target.Second) return;
            // 추후 구현
        }
    }
}
