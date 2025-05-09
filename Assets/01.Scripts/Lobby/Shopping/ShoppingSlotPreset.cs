﻿using Capsule.Entity;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingSlotPreset : ShoppingSlot
    {
        public CustomizingPreset prestNum;
        public override void SelectSlot()
        {
            this.IsSelected = true;
            ShoppingManager.Instance.CurrentPreset = this;
        }
        public override void ReselectSlot()
        {
            base.ReselectSlot();
            ShoppingManager.Instance.CurrentPreset = null;
        }
    }
}