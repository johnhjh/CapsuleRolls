using Capsule.Entity;

namespace Capsule.Lobby.Customize
{
    public class CustomizeSlotGlove : CustomizeSlot
    {
        public CustomizingGlove gloveNum;

        public override void SelectSlot()
        {
            CustomizeManager.Instance.CurrentGlove = this;
        }
    }
}