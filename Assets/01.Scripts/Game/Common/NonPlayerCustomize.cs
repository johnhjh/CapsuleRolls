﻿using Capsule.Entity;
using Capsule.Game.Player;
using UnityEngine;

namespace Capsule.Game
{
    public class NonPlayerCustomize : PlayerCustomize
    {
        public bool isRagdoll = false;
        public bool isRandom = false;
        private PlayerCustomizeData data = null;
        public CustomizingBody body;
        public CustomizingHead head;
        public CustomizingFace face;
        public CustomizingGlove glove;
        public CustomizingCloth cloth;

        protected override void Start()
        {
            if (isRandom)
            {
                if (!isRagdoll)
                    RandomCustomize();
            }
            else
            {
                data = new PlayerCustomizeData(
                    (int)body,
                    (int)head,
                    (int)face,
                    (int)glove,
                    (int)cloth);
                PlayerCustomizeInit();
                if (isRagdoll)
                    this.gameObject.SetActive(false);
            }
        }

        private void SetRagdollCustomizeInit()
        {
            if (transform.parent.GetChild(1).TryGetComponent(out RagdollController controller))
            {
                if (controller.ragdollObj.TryGetComponent(out NonPlayerCustomize customize))
                    customize.CopyCustomizeDataFromOrigin(data);
            }
        }

        public void RandomCustomize()
        {
            data = new PlayerCustomizeData(
                Random.Range(1, (int)CustomizingBody.STAR),
                Random.Range(1, (int)CustomizingHead.OUTLAW),
                Random.Range(1, (int)CustomizingFace.SANTA),
                Random.Range(0, (int)CustomizingGlove.HOOK),
                Random.Range(1, (int)CustomizingCloth.SANTA));
            PlayerCustomizeInit();
            SetRagdollCustomizeInit();
        }

        public void CopyCustomizeDataFromOrigin(PlayerCustomizeData customzieData)
        {
            data = customzieData;
            PlayerCustomizeInit();
            this.gameObject.SetActive(false);
        }

        private void PlayerCustomizeInit()
        {
            ChangeBody((CustomizingBody)data.Body);
            ChangeHead((CustomizingHead)data.Head);
            ChangeFace((CustomizingFace)data.Face);
            ChangeGloves((CustomizingGlove)data.Glove);
            ChangeCloth((CustomizingCloth)data.Cloth);
        }
    }
}