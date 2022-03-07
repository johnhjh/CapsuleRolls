using UnityEngine;
using UnityEngine.EventSystems;
using Capsule.Entity;

namespace Capsule.Lobby.Customize
{
    [RequireComponent (typeof (CanvasGroup))]
    public abstract class CustomizeSlot : AbstractSlot
    {
        public CustomizingData data;

        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set { isLocked = value; }
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
