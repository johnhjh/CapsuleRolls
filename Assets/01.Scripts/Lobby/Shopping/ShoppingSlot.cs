using System.Collections;
using UnityEngine;
using Capsule.Entity;
using Capsule.Lobby;

namespace Capsule.Lobby.Shopping
{
    public abstract class ShoppingSlot : AbstractSlot
    {
        public CustomizingData data;
        public int price;
        private bool isPurchased;
        public bool IsPurchased
        {
            set { isPurchased = value; }
        }

        public void Purchase()
        {

        }
    }
}