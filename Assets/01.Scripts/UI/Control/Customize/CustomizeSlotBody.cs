using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Customize
{
    public class CustomizeSlotBody : CustomizeSlot
    {
        public CustomizingBody bodyColor;
        public Material bodyMaterial;

        public override void Equip()
        {
            CustomizeManager.Instance.CurrentBody = this;
        }
    }
}