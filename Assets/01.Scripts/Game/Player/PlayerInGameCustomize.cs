using Capsule.Entity;

namespace Capsule.Game.Player
{
    public class PlayerInGameCustomize : PlayerCustomize
    {
        public bool isMine = false;
        public bool isRagdoll = false;
        private PlayerCustomizeData data = null;
        public PlayerCustomizeData Data
        {
            set
            {
                data = value;
                PlayerCustomizeInit();
            }
        }

        protected override void Start()
        {
            if (isMine)
            {
                data = DataManager.Instance.CurrentPlayerCustomizeData;
                PlayerCustomizeInit();
            }
            if (isRagdoll)
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