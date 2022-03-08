using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotBody : ShoppingSlot
    {
        public CustomizingBody bodyColor;
        public Material bodyMaterial;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentBody = this;
        }
    }
}