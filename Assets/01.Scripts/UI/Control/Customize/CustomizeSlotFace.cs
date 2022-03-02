using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Customize
{
    public class CustomizeSlotFace : CustomizeSlot
    {
        public CustomizingFace faceItem;

        public override void Equip()
        {
            CustomizeManager.Instance.CurrentFace = this;
        }
    }
}