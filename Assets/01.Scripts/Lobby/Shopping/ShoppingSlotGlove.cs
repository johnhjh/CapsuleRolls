using System.Collections;
using UnityEngine;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotGlove : ShoppingSlot
    {
        public override void SelectSlot()
        {
            this.IsSelected = true;
        }
    }
}