using Capsule.Entity;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotFace : ShoppingSlot
    {
        public CustomizingFace faceItem;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentFace = this;
        }
        public override void ReselectSlot()
        {
            base.ReselectSlot();
            ShoppingManager.Instance.CurrentFace = null;
        }
    }
}