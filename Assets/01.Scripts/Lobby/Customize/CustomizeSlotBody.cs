using Capsule.Entity;
using UnityEngine;

namespace Capsule.Lobby.Customize
{
    public class CustomizeSlotBody : CustomizeSlot
    {
        public CustomizingBody bodyColor;
        public Material bodyMaterial;

        public override void SelectSlot()
        {
            CustomizeManager.Instance.CurrentBody = this;
        }
    }
}