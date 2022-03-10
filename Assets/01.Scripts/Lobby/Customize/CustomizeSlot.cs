using UnityEngine;
using UnityEngine.EventSystems;
using Capsule.Entity;

namespace Capsule.Lobby.Customize
{
    [RequireComponent (typeof (CanvasGroup))]
    public abstract class CustomizeSlot : AbstractSlot
    {
        public CustomizingData data;
        private GameObject lockImage = null;

        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set 
            {
                isLocked = value;
                if (lockImage == null)
                    lockImage = transform.GetChild(1).gameObject;
                lockImage.SetActive(value);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (isLocked) return;
            base.OnPointerClick(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isLocked) return;
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (isLocked) return;
            base.OnPointerExit(eventData);
        }
    }
}
