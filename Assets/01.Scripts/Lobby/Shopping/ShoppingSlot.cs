using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Capsule.Entity;
using Capsule.Lobby;
using Capsule.Audio;

namespace Capsule.Lobby.Shopping
{
    public abstract class ShoppingSlot : AbstractSlot
    {
        public CustomizingData data;
        public int price;
        private bool isPurchased;
        public bool IsPurchased
        {
            get { return isPurchased; }
            set
            {
                if (isPurchased) return;
                Purchase();
            }
        }

        public virtual void ReselectSlot()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            IsSelected = false;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (isPurchased) return;
            if (!IsSelected)
                base.OnPointerClick(eventData);
            else
                ReselectSlot();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isPurchased) return;
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (isPurchased) return;
            base.OnPointerExit(eventData);
        }

        private void Purchase()
        {
            isPurchased = true;
            base.canvasGroup.alpha = 1f;
            base.canvasGroup.blocksRaycasts = false;
            base.canvasGroup.interactable = false;
            GameObject.Instantiate(ShoppingManager.Instance.purchasedCover, base.transform);
        }
    }
}