using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class MobileRotateDragCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        private static MobileRotateDragCtrl mobileDragCtrl;
        public static MobileRotateDragCtrl Instace
        {
            get
            {
                if (mobileDragCtrl == null)
                    mobileDragCtrl = FindObjectOfType<MobileRotateDragCtrl>();
                return mobileDragCtrl;
            }
        }

        private void Awake()
        {
            if (mobileDragCtrl == null)
            {
                mobileDragCtrl = this;
                cg = GetComponent<CanvasGroup>();
            }
            else if (mobileDragCtrl != this)
                Destroy(this.gameObject);
        }

        [HideInInspector]
        public float rotate = 0f;
        private bool dragPanelPressed = false;
        private int touchID = -1;
        private float touchX = 0f;
        private CanvasGroup cg = null;

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
                HandleTouchInput();
            else
                HandleDrag();
        }

        private void HandleDrag()
        {
            if (dragPanelPressed)
                rotate = Input.GetAxis("Mouse X");
            else
                rotate = 0f;
        }

        private void HandleTouchInput()
        {
            int i = 0;
            if (!dragPanelPressed) rotate = 0f;
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    i++;
                    Vector2 touchPos = new Vector2(touch.position.x, touch.position.y);
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (touch.position.x >= Screen.width / 2 &&
                            touch.position.y <= Screen.height * 0.7f)
                        {
                            cg.alpha = 1f;
                            touchID = i;
                            touchX = touch.position.x;
                            dragPanelPressed = true;
                        }
                    }
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (touchID == i)
                        {
                            rotate = (touchX - touch.position.x) / -50f;
                            rotate = Mathf.Clamp(rotate, -1f, 1f);
                        }
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        if (touchID == i)
                        {
                            cg.alpha = 0.6f;
                            touchID = -1;
                            touchX = 0f;
                            rotate = 0f;
                            dragPanelPressed = false;
                        }
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Application.platform != RuntimePlatform.Android &&
                Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (!dragPanelPressed)
                    cg.alpha = 0.8f;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Application.platform != RuntimePlatform.Android &&
                Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (!dragPanelPressed)
                    cg.alpha = 0.6f;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Application.platform != RuntimePlatform.Android &&
                Application.platform != RuntimePlatform.IPhonePlayer)
            {
                cg.alpha = 1f;
                dragPanelPressed = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Application.platform != RuntimePlatform.Android &&
                Application.platform != RuntimePlatform.IPhonePlayer)
            {
                cg.alpha = 0.6f;
                rotate = 0f;
                dragPanelPressed = false;
            }
        }
    }
}


