using Capsule.Entity;

namespace Capsule.Lobby.Customize
{
    public class CustomizeSlotCloth : CustomizeSlot
    {
        public CustomizingCloth clothNum;

        public override void SelectSlot()
        {
            CustomizeManager.Instance.CurrentCloth = this;
        }
    }
}