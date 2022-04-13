using Capsule.Game.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Capsule.Game.UI
{
    public class JoyStickCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform touchPad;
        private int touchID = -1;
        private Vector3 startPos = Vector3.zero;
        private float dragRadius;
        private float touchableRadius;

        private PlayerInput playerInput = null;
        public PlayerInput InputPlayer { set { playerInput = value; } }

        [HideInInspector]
        public float horizontal;
        [HideInInspector]
        public float vertical;
        [HideInInspector]
        public float rotate;

        private Image topLeftFocus = null;
        private Image topRightFocus = null;
        private Image bottomLeftFocus = null;
        private Image bottomRightFocus = null;

        private readonly Color unFocusedColor = new Color(1f, 1f, 1f, 0f);
        private readonly Color focusedColor = new Color(1f, 1f, 1f, 1f);

        private RectTransform joystickBack;

        private bool joyStickPressed = false;
        public bool JoyStickPressed
        {
            get { return joyStickPressed; }
            private set { joyStickPressed = value; }
        }

        private void Start()
        {
            touchPad = GetComponent<RectTransform>();
            startPos = touchPad.position;
            joyStickPressed = false;
            topLeftFocus = transform.parent.GetChild(0).GetComponent<Image>();
            topRightFocus = transform.parent.GetChild(1).GetComponent<Image>();
            bottomLeftFocus = transform.parent.GetChild(2).GetComponent<Image>();
            bottomRightFocus = transform.parent.GetChild(3).GetComponent<Image>();

            joystickBack = transform.parent.GetComponent<RectTransform>();
            joystickBack.sizeDelta = new Vector2(600f, 600f);
            //dragRadius = Screen.height * 0.12f;
            dragRadius = Screen.height * 0.16f;
            //Debug.Log("drag : " + dragRadius);
            //touchableRadius = Screen.height * 0.14f;
            touchableRadius = Screen.height * 0.18f;
            //Debug.Log("touchable : " + touchableRadius);

        }

        private void Update()
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
                        (playerInput.usingHorizontal ?
                        (dragRadius * playerInput.horizontal * Vector3.right) :
                        (dragRadius * playerInput.rotate * Vector3.right)));
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

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    Vector2 touchPos = new Vector2(touch.position.x, touch.position.y);
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (touch.position.x <= (startPos.x + touchableRadius) &&
                            touch.position.y <= (startPos.y + touchableRadius))
                        {
                            JoyStickPressed = true;
                            touchID = touch.fingerId;
                            HandleInput(touchPos);
                        }
                    }
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (touchID == touch.fingerId)
                            HandleInput(touchPos);
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        if (touchID == touch.fingerId)
                        {
                            touchID = -1;
                            JoyStickPressed = false;
                            HandleInput(startPos);
                        }
                    }
                }
            }
        }

        private void HandleInput(Vector3 input)
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
            if (playerInput != null && playerInput.usingHorizontal)
            {
                horizontal = diff.x / dragRadius;
                rotate = 0f;
            }
            else
            {
                horizontal = 0f;
                rotate = diff.x / dragRadius;
            }
            vertical = diff.y / dragRadius;

            HandleFocus(diff.x, diff.y);
        }

        private void HandleFocus(float x, float y)
        {
            topLeftFocus.color = unFocusedColor;
            topRightFocus.color = unFocusedColor;
            bottomLeftFocus.color = unFocusedColor;
            bottomRightFocus.color = unFocusedColor;

            if (x > 0 && y > 0)
                topRightFocus.color = focusedColor;
            else if (x < 0 && y > 0)
                topLeftFocus.color = focusedColor;
            else if (x > 0 && y < 0)
                bottomRightFocus.color = focusedColor;
            else if (x < 0 && y < 0)
                bottomLeftFocus.color = focusedColor;
        }
    }
}
