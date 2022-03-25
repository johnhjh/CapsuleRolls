using Capsule.Game.Player;
using System.Collections;
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
            joystickBack.sizeDelta = new Vector2(500f, 500f);
            dragRadius = Screen.height * 0.12f;
            //Debug.Log("drag : " + dragRadius);
            touchableRadius = Screen.height * 0.14f;
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

        private void HandleTouchInput()
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
                        if (touch.position.x <= (startPos.x + touchableRadius) &&
                            touch.position.y <= (startPos.y + touchableRadius))
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
                        {
                            touchID = -1;
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
            horizontal = 0f;
            vertical = diff.y / dragRadius;
            rotate = diff.x / dragRadius;

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
