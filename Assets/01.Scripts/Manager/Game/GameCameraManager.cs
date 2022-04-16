using Capsule.Util;
using Cinemachine;
using System.Collections;
using UnityEngine;

namespace Capsule.Game
{
    [System.Serializable]
    public class ScriptedCameraAction
    {
        [Header("Cam Move Setting")]
        public float distance = 8.0f;
        public float height = 15.0f;
        public float moveDamping = 3.0f;
        public float rotateDamping = 5.0f;
        public bool isActive = false;
        public IEnumerator SetCameraQuater(Transform camTransform, Transform targetTransform)
        {
            if (camTransform == null || targetTransform == null)
                yield break;
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

        public IEnumerator MoveCameraInverseTarget(Transform camTransform, Transform targetTransform)
        {
            if (camTransform == null || targetTransform == null)
                yield break;
            Vector3 camPos = camTransform.position;
            Vector3 desiredPos = targetTransform.position;
            desiredPos = 2.2f * desiredPos - camPos;
            desiredPos.y = camPos.y;
            while (isActive)
            {
                camTransform.SetPositionAndRotation(
                    Vector3.Slerp(
                        camTransform.position,
                        desiredPos,
                        Time.deltaTime * moveDamping),
                    Quaternion.Slerp(
                        camTransform.rotation,
                        Quaternion.LookRotation(
                            (targetTransform.position - camTransform.position + 2f * Vector3.up).normalized),
                        Time.deltaTime * rotateDamping));
                yield return new WaitForSeconds(0.02f);
                InfiniteLoopDetector.Run();
            }
        }

        public IEnumerator MoveCameraByPos(Transform camTransform, Transform targetTransform, Vector3 position)
        {
            if (camTransform == null || targetTransform == null)
                yield break;
            Vector3 desiredPos = camTransform.position + position;
            while (isActive)
            {
                camTransform.SetPositionAndRotation(
                    Vector3.Slerp(
                        camTransform.position,
                        desiredPos,
                        Time.deltaTime * moveDamping),
                    Quaternion.Slerp(
                        camTransform.rotation,
                        Quaternion.LookRotation(
                            (targetTransform.position - camTransform.position + position).normalized),
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

        public ScriptedCameraAction scriptedCameraAction;
        private Transform mainCameraTransform;
        //private CinemachineVirtualCamera moveFollowCam;
        private CinemachineFreeLook moveFollowCam;
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
                if (value.Second)
                    SetCameraQuater();
                else
                    ActivateFollowCam();
            }
        }

        private void Awake()
        {
            target = new Tuple<Transform, bool>();
            scriptedCameraAction = new ScriptedCameraAction();
            //moveFollowCam = GameObject.Find("MoveFollowCam").GetComponent<CinemachineVirtualCamera>();
            moveFollowCam = GameObject.Find("MoveFollowCam").GetComponent<CinemachineFreeLook>();
            target.First = moveFollowCam.Follow;
        }

        private void Start()
        {
            if (Camera.main != null)
                mainCameraTransform = Camera.main.transform;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                SetCameraYSpeed(0f);
            else
                SetCameraYSpeed(PlayerPrefs.GetFloat("ViewY", 1f));
            if (GameManager.Instance != null)
                GameManager.Instance.OnStageClear += () => { MoveCameraInverseTarget(); };
        }

        public void SetCameraYSpeed(float speed)
        {
            moveFollowCam.m_YAxis.m_MaxSpeed = speed;
        }

        public void ActivateFollowCam()
        {
            scriptedCameraAction.isActive = false;
            if (mainCamCoroutine != null)
            {
                StopCoroutine(mainCamCoroutine);
                mainCamCoroutine = null;
            }
            moveFollowCam.Follow = Target.First;
            moveFollowCam.LookAt = Target.First;
            moveFollowCam.enabled = true;
        }

        private bool UsingScriptedCamera()
        {
            moveFollowCam.enabled = false;
            scriptedCameraAction.isActive = true;
            if (mainCameraTransform == null && Camera.main != null)
                mainCameraTransform = Camera.main.transform;
            if (mainCamCoroutine != null)
                StopCoroutine(mainCamCoroutine);
            return mainCameraTransform != null;
        }

        public void SetCameraQuater()
        {
            if (UsingScriptedCamera())
            {
                mainCameraTransform.GetComponent<Camera>().nearClipPlane = 0.3f;
                mainCamCoroutine = StartCoroutine(
                    scriptedCameraAction.SetCameraQuater(mainCameraTransform, Target.First));
            }
        }

        public void MoveCameraInverseTarget()
        {
            if (UsingScriptedCamera())
            {
                mainCameraTransform.GetComponent<Camera>().nearClipPlane = 0.3f;
                mainCamCoroutine = StartCoroutine(
                    scriptedCameraAction.MoveCameraInverseTarget(mainCameraTransform, Target.First));
            }
        }

        public void MoveCameraByPos(Vector3 pos)
        {
            if (UsingScriptedCamera())
            {
                mainCameraTransform.GetComponent<Camera>().nearClipPlane = 0.3f;
                mainCamCoroutine = StartCoroutine(
                    scriptedCameraAction.MoveCameraByPos(mainCameraTransform, Target.First, pos));
            }
        }

        public void CameraShake()
        {
            if (Target.Second) return;
            // 추후 구현
        }
    }
}
