using Capsule.Entity;
using UnityEngine.EventSystems;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingTabCtrl : TabCtrl
    {
        public CustomizingType customizeType;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (base.IsFocused) return;
            base.OnPointerClick(eventData);
            ShoppingManager.Instance.ChangeFocusTab(base.tabTransform, customizeType);
        }
    }
}