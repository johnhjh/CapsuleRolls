using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class MobileViewDragCtrl : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        private static MobileViewDragCtrl mobileDragCtrl;
        public static MobileViewDragCtrl Instance
        {
            get
            {
                if (mobileDragCtrl == null)
                    mobileDragCtrl = FindObjectOfType<MobileViewDragCtrl>();
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

        private bool dragPressed = false;
        public bool DragPressed { get { return dragPressed; } }
        private CanvasGroup cg = null;

        public void OnPointerDown(PointerEventData eventData)
        {
            dragPressed = true;
            if (cg != null)
                cg.alpha = 1f;
            else
                cg = GetComponent<CanvasGroup>();
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (GameCameraManager.Instance != null)
                    GameCameraManager.Instance.SetCameraYSpeed(PlayerPrefs.GetFloat("ViewY", 1f));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragPressed = false;
            if (cg != null)
                cg.alpha = 0.6f;
            else
                cg = GetComponent<CanvasGroup>();
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (GameCameraManager.Instance != null)
                    GameCameraManager.Instance.SetCameraYSpeed(0f);
            }                
        }
    }
}
