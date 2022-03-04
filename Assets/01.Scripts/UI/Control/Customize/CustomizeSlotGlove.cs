using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Customize
{
    public class CustomizeSlotGlove : CustomizeSlot
    {
        public CustomizingGlove gloveNum;

        public override void Equip()
        {
            CustomizeManager.Instance.CurrentGlove = this;
        }
    }
}