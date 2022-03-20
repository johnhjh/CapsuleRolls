using Capsule.Entity;

namespace Capsule.Lobby.Customize
{
    public class CustomizeSlotFace : CustomizeSlot
    {
        public CustomizingFace faceItem;

        public override void SelectSlot()
        {
            CustomizeManager.Instance.CurrentFace = this;
        }
    }
}