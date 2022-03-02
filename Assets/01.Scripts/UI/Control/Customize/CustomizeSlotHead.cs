using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Customize
{
    public class CustomizeSlotHead : CustomizeSlot
    {
        public CustomizingHead headItem;

        public override void Equip()
        {
            CustomizeManager.Instance.CurrentHead = this;
        }
    }
}