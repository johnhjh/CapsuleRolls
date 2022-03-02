using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Customize
{
    public class CustomizeSlotGlove : CustomizeSlot
    {
        public CustomizingGlove gloveItem;

        public override void Equip()
        {
            CustomizeManager.Instance.CurrentGlove = this;
        }
    }
}