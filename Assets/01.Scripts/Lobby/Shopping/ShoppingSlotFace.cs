using System.Collections;
using UnityEngine;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotFace : ShoppingSlot
    {
        public override void SelectSlot()
        {
            this.IsSelected = true;
        }
    }
}