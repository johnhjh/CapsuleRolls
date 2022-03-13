using UnityEngine;

public enum CameraView
{
    TPS = 0,
    TPS2,
    PLANE,
    QUATER,
    TOP,
    SHOULDER,
    FPS,
    LAST,
}
public class CameraFollow : MonoBehaviour
{
    [Header ("Current View")]
    public CameraView camView = CameraView.TPS; // 현재 뷰
    private int currentView = 0;

    private Transform cameraTransform;  // 카메라
    [Header ("Target")]
    public Transform targetTransform;   // 추적 대상
    [SerializeField]
    private Vector3 camPos = Vector3.zero;

    [Header ("Cam Setting")]
    public float distance = 4.0f;   // 거리
    public float height = 3.0f; // 높이
    public float moveDamping = 15.0f;   // 이동 속도
    public float rotateDamping = 5.0f;  // 회전 속도
    public float cameraRot = 0f;    // 카메라 높낮이 각도

    [Header ("Field of View")]
    public int MIN_FIELD_OF_VIEW = 60;
    public int MAX_FIELD_OF_VIEW = 100;
    public float magnification = 30f;

    private bool isChanged = false;

    void Start()
    {
        cameraTransform = GetComponent<Transform>();
        if (targetTransform == null)
        {
            var player = GameObject.Find("Player");
            if (player == null)
                player = GameObject.FindWithTag("Player");
            if (player != null)
                targetTransform = player.transform;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            camView = (CameraView)(currentView++ % (int)CameraView.LAST);
            isChanged = true;
        }
        switch (camView)
        {
            case CameraView.TPS:
                //cameraTransform.position = targetTransform.position - (rot * Vector3.forward * dist) + Vector3.up * height;
                camPos = targetTransform.position 
                    - (targetTransform.forward * distance) 
                    + (targetTransform.up * height);
                cameraTransform.position = Vector3.Slerp(cameraTransform.position,
                    camPos,
                    Time.deltaTime * moveDamping);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation,
                    targetTransform.rotation,
                    Time.deltaTime * rotateDamping);
                cameraTransform.LookAt(targetTransform.position 
                    + targetTransform.up * cameraRot);
                break;
            case CameraView.TPS2:
                cameraTransform.position = targetTransform.position - Vector3.forward * distance + Vector3.up * height;
                cameraTransform.LookAt(targetTransform.position 
                    + targetTransform.up * cameraRot);
                break;
            case CameraView.PLANE:
                if (isChanged)
                {
                    //cameraTransform.position = targetTransform.position + (Vector3.right * 3.3f) - Vector3.up * -2.6f;
                    camPos = targetTransform.position 
                        + (Vector3.right * 3.3f) 
                        - Vector3.up * -2.6f;
                    isChanged = false;
                }
                cameraTransform.position = Vector3.Slerp(cameraTransform.position,
                    camPos,
                    Time.deltaTime * moveDamping);
                //cameraTransform.rotation = Quaternion.Euler(17.165f, -88.734f, 0);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation,
                    Quaternion.Euler(17.165f, -88.734f, 0),
                    Time.deltaTime * rotateDamping);
                break;
            case CameraView.QUATER:
                //cameraTransform.position = new Vector3(0, height * 3, -8f) + targetTransform.position;
                camPos = new Vector3(0, height * 3, -8f) + targetTransform.position;
                cameraTransform.position = Vector3.Slerp(cameraTransform.position,
                    camPos,
                    Time.deltaTime * moveDamping);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation,
                    Quaternion.Euler(50, 0, 0),
                    Time.deltaTime * rotateDamping);
                break;
            case CameraView.TOP:
                cameraTransform.position = targetTransform.position + Vector3.up * height * 3;
                cameraTransform.LookAt(targetTransform.position + Vector3.forward * cameraRot);
                break;
            case CameraView.SHOULDER:
                Quaternion rot = Quaternion.Euler(0, Mathf.LerpAngle(transform.eulerAngles.y, targetTransform.eulerAngles.y, rotateDamping * Time.deltaTime), 0);
                cameraTransform.position = targetTransform.position - (rot * Vector3.forward * 2.0f) + Vector3.up * 1.5f;
                cameraTransform.rotation = targetTransform.rotation;
                break;
            case CameraView.FPS:
                cameraTransform.position = targetTransform.position + Vector3.up * 1.5f;
                cameraTransform.rotation = targetTransform.rotation;
                break;
        }
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel != 0)
        {
            Camera.main.fieldOfView += mouseWheel * magnification;
            if (Camera.main.fieldOfView > MAX_FIELD_OF_VIEW)
                Camera.main.fieldOfView = MAX_FIELD_OF_VIEW;
            else if (Camera.main.fieldOfView < MIN_FIELD_OF_VIEW)
                Camera.main.fieldOfView = MIN_FIELD_OF_VIEW;
        }
    }
}
