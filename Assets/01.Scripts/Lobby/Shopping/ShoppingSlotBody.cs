using Capsule.Entity;
using UnityEngine;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotBody : ShoppingSlot
    {
        public CustomizingBody bodyColor;
        public Material bodyMaterial;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentBody = this;
        }
        public override void ReselectSlot()
        {
            base.ReselectSlot();
            ShoppingManager.Instance.CurrentBody = null;
        }
    }
}