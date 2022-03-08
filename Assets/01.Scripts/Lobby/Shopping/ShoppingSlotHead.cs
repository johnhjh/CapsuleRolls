using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotHead : ShoppingSlot
    {
        public CustomizingHead headItem;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentHead = this;
        }
    }
}