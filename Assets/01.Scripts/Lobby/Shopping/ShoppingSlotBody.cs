using System.Collections;
using UnityEngine;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotBody : ShoppingSlot
    {
        public Material bodyMaterial;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentBody = this;
        }
    }
}