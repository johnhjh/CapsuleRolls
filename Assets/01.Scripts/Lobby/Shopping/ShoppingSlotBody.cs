using System.Collections;
using UnityEngine;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotBody : ShoppingSlot
    {
        public override void SelectSlot()
        {
            this.IsSelected = true;
        }
    }
}