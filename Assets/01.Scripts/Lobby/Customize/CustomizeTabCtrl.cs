using UnityEngine.EventSystems;
using Capsule.Entity;

namespace Capsule.Lobby.Customize
{
    public class CustomizeTabCtrl : TabCtrl
    {
        public CustomizingType customizeType;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (base.IsFocused) return;
            base.OnPointerClick(eventData);
            CustomizeManager.Instance.ChangeFocusTab(base.tabTransform, customizeType);
        }
    }
}
