using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class MobileRotateDragCtrl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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

        private CanvasGroup cg = null;
        [HideInInspector]
        public float rotate = 0f;

        public void OnBeginDrag(PointerEventData eventData)
        {
            cg.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rotate = Input.GetAxis("Mouse X");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            cg.alpha = 0f;
            rotate = 0f;
        }
    }
}


