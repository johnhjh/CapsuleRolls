using UnityEngine;

public class FreeMove : MonoBehaviour
{
    private Transform camTr;
    private float h = 0, v = 0;

    [Header("Movement")]
    public float MoveSpeed = 40f;
    public float NormalMoveSpeed = 40f;
    public float FastMoveSpeed = 80f;
    public float RotSpeed = 70f;
    public float XSensitivity = 100f; // 감도
    public float YSensitivity = 100f; // 감도
    public float YMinLimit = -45f;
    public float YMaxLimit = 45f;
    private float XRot = 0.0f;// 회전변수
    private float YRot = 0.0f;//
    private Vector3 moveVector = Vector3.zero;

    [Header("Transform")]
    public Vector3 initTransform = Vector3.zero;
    private Vector3 saveTransform;

    [Header("Different Ammount of Vector3")]
    public Vector3 diffVector = Vector3.one;

    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            MoveSpeed = FastMoveSpeed;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            MoveSpeed = NormalMoveSpeed;

        if (Input.GetKey(KeyCode.Z))
            transform.Translate(Vector3.up * 0.2f * MoveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.C))
            transform.Translate(Vector3.up * -0.2f * MoveSpeed * Time.deltaTime);

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * h * MoveSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * v * MoveSpeed * Time.deltaTime);

        XRot += Input.GetAxis("Mouse X") * XSensitivity * Time.deltaTime;
        YRot += Input.GetAxis("Mouse Y") * YSensitivity * Time.deltaTime;
        YRot = Mathf.Clamp(YRot, YMinLimit, YMaxLimit);
        //수학클래스 안에 값제한 함수 Clamp(what?,최소값,최대값);
        transform.localEulerAngles = new Vector3(-YRot, XRot, 0f); // 마우스 x축이면 캐릭터는 y축 마우스 y축이면 캐릭터 x축

        if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.LeftControl))
            camTr.position = initTransform;

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
            initTransform = camTr.position;

        // 비교할 위치 지정
        if (Input.GetKeyDown(KeyCode.Alpha1))
            saveTransform = camTr.position;
        if (saveTransform != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
                camTr.position = saveTransform;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                camTr.position = saveTransform + diffVector;
        }
    }
}
