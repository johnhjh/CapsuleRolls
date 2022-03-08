using Capsule.Entity;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotCloth : ShoppingSlot
    {
        public CustomizingCloth clothNum;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentCloth = this;
        }
    }
}