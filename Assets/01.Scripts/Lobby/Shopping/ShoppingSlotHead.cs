using Capsule.Entity;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotHead : ShoppingSlot
    {
        public CustomizingHead headItem;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentHead = this;
        }
        public override void ReselectSlot()
        {
            base.ReselectSlot();
            ShoppingManager.Instance.CurrentHead = null;
        }
    }
}