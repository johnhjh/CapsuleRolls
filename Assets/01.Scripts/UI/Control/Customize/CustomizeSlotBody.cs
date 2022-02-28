using System.Collections;
using UnityEngine;

namespace Capsule.Customize
{
    public class CustomizeSlotBody : CustomizeSlot
    {
        public Material bodyMaterial;

        public override void Equip()
        {
            CustomizeManager.Instance.CurrentBody = this;
        }
    }
}