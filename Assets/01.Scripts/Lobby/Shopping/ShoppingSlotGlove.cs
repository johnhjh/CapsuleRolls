using System.Collections;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotGlove : ShoppingSlot
    {
        public CustomizingGlove gloveNum;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentGlove = this;
        }
    }
}