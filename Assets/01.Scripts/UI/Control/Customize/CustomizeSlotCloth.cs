using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Customize
{
    public class CustomizeSlotCloth : CustomizeSlot
    {
        public CustomizingCloth clothNum;

        public override void Equip()
        {
            CustomizeManager.Instance.CurrentCloth = this;
        }
    }
}