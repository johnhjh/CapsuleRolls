using System.Collections;
using UnityEngine;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotCloth : ShoppingSlot
    {
        public override void SelectSlot()
        {
            this.IsSelected = true;
        }
    }
}