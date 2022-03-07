using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Lobby.Customize
{
    public class CustomizeSlotHead : CustomizeSlot
    {
        public CustomizingHead headItem;

        public override void SelectSlot()
        {
            CustomizeManager.Instance.CurrentHead = this;
        }
    }
}