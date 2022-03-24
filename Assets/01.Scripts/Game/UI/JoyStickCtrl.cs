using Capsule.Game.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class JoyStickCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform touchPad;
        private int touchID = -1;
        private Vector3 startPos = Vector3.zero;
        public float dragRadius = 120f;

        private PlayerInput playerInput = null;
        public PlayerInput InputPlayer { set { playerInput = value; } }

        [HideInInspector]
        public float horizontal;
        [HideInInspector]
        public float vertical;
        [HideInInspector]
        public float rotate;

        private bool joyStickPressed = false;
        public bool JoyStickPressed
        {
            get { return joyStickPressed; }
            private set { joyStickPressed = value; }
        }

        void Start()
        {
            touchPad = GetComponent<RectTransform>();
            startPos = touchPad.position;
            joyStickPressed = false;
        }

        void Update()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                HandleTouchInput();
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                if (joyStickPressed)
                    HandleInput(Input.mousePosition);
                else if (playerInput != null)
                    HandleInput(
                        startPos +
                        (dragRadius * playerInput.vertical * Vector3.up) +
                        (dragRadius * playerInput.horizontal * Vector3.right));
                else
                    HandleInput(startPos);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            joyStickPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            joyStickPressed = false;
            HandleInput(startPos);
        }

        void HandleTouchInput()
        {
            int i = 0;
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    i++;
                    Vector2 touchPos = new Vector2(touch.position.x, touch.position.y);
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (touch.position.x <= (startPos.x + dragRadius) &&
                            touch.position.y <= (startPos.y + dragRadius))
                            touchID = i;
                    }
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (touchID == i)
                            HandleInput(touchPos);
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        if (touchID == i)
                            touchID = -1;
                    }
                }
            }
        }
        void HandleInput(Vector3 input)
        {
            Vector3 diffVector = (input - startPos);
            if (diffVector.sqrMagnitude > dragRadius * dragRadius)
            {
                diffVector.Normalize();
                touchPad.position = startPos + diffVector * dragRadius;
            }
            else
                touchPad.position = input;

            Vector3 diff = touchPad.position - startPos;
            horizontal = diff.x / dragRadius;
            vertical = diff.y / dragRadius;
            rotate = diff.y / dragRadius;
        }
    }
}


/*
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick_Ctrl : MonoBehaviour, IDragHandler
    , IPointerUpHandler, IPointerDownHandler
{
    private Image OuterImg;
    private Image InnerImg;
    private Vector3 inputVector;
    private Rocket_Ctrl rocketCtrl;

    void Start()
    {
        OuterImg = GetComponent<Image>();
        // gameObject.transform.
        //InnerImg = transform.GetChild(0).GetComponent<Image>();
        InnerImg = GetComponentsInChildren<Image>()[1];
        rocketCtrl = GameObject.Find("Rocket").GetComponent<Rocket_Ctrl>();
    }

    //throw new System.NotImplementedException()    : 상속 받았을 시 Override 를 안하면 예외 발생

    // OnBeginDrag  : Drag 를 시작하는 지점을 잡아 호출되는 콜백 함수
    // OnEndDrag    : Drag 를 끝내는 지점을 잡아 호출되는 콜백 함수

    // OnDrag : Drag 가 되는 동안 스스로 호출되는 콜백 함수
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        // RectTransform 을 사용하기 위한 헬퍼 메서드가 포함 된 유틸리티 클래스
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(OuterImg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            // 스크린 공간 점을 직사각형 평면에 있는 RectTransform 의 로컬 공간에 있는 위치로 변환
            pos.x = (pos.x / OuterImg.rectTransform.sizeDelta.x);
            //이 RectTransform의 크기는, 엥커 간의 거리에 관련합니다.
            // 앵커가 함께 있으면 sizeDelta는 크기와 같습니다. 앵커가 부모의 네 모서리에있는 경우 sizeDelta는 부모와 비교하여
            //사각형의 크기가 얼마나 크거나 작으면됩니다.
            pos.y = (pos.y / OuterImg.rectTransform.sizeDelta.y);
            inputVector = new Vector3(pos.x * 2 + 1, pos.y * 2 - 1, 0f);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            // 터치패드를 누르고 있을 때 실행할 onDrag(PointerEventData ped) 함수를 구현합니다. 
            //  bgImg영역에 터치가 발생했을 때
            //  (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos)가 true일 때), 
            //  터치된 로컬 좌표값을 pos에 할당하고 bgImg 직사각형의 sizeDelta값으로 나누어 pos.x는 0~-1, pos.y는 0~1사이의 값으로 만듭니다.
            //  joystickImg를 기준으로 좌우로 움직였을 때
            //  pos.x는 - 1~1 사이의 값으로, 상하로 움직였을 때 pos.y는 - 1~1의 값으로 변환하기 
            //  위해 pos.x * 2 + 1, pos.y * 2 - 1 처리를 합니다.
            //  이 값을 inputVector에 대입하고 단위벡터로 만듭니다.]
            //  마지막으로 joystickImg를 터치한 좌표값으로 이동시킵니다.
            //    
            InnerImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (OuterImg.rectTransform.sizeDelta.x / 3),
                inputVector.y * (OuterImg.rectTransform.sizeDelta.y / 3));
        }
    }

    // OnPointerDown : 누를 시
    public void OnPointerDown(PointerEventData eventData)
    {
        rocketCtrl.isJoyStick = true;
        OnDrag(eventData);
    }

    // OnPointerUp : 뗄 시
    public void OnPointerUp(PointerEventData eventData)
    {
        rocketCtrl.isJoyStick = false;
        inputVector = Vector3.zero;
        InnerImg.rectTransform.anchoredPosition = Vector3.zero;
        // 원래 위치로 돌아간다.
        OuterImg.rectTransform.anchoredPosition = new Vector3(-50f, 30f, 0f);
    }
    public float GetHorizontalValue()
    {
        return inputVector.x;
    }
    public float GetVerticalValue()
    {
        return inputVector.y;
    }
}

 
 */