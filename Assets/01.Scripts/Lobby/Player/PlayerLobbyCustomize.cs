using Capsule.Entity;

namespace Capsule.Lobby.Player
{
    public class PlayerLobbyCustomize : PlayerCustomize
    {
        private static PlayerLobbyCustomize playerCustomize;
        public static PlayerLobbyCustomize Instance
        {
            get
            {
                if (playerCustomize == null)
                    playerCustomize = FindObjectOfType<PlayerLobbyCustomize>();
                return playerCustomize;
            }
        }

        protected override void Awake()
        {
            if (playerCustomize == null)
            {
                playerCustomize = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (playerCustomize != this)
                Destroy(this.gameObject);
        }

        protected override void Start()
        {
            PlayerCustomizeInit();
        }

        private void PlayerCustomizeInit()
        {
            PlayerCustomizeData data = DataManager.Instance.CurrentPlayerCustomizeData;

            ChangeBody((CustomizingBody)data.Body);
            ChangeHead((CustomizingHead)data.Head);
            ChangeFace((CustomizingFace)data.Face);
            ChangeGloves((CustomizingGlove)data.Glove);
            ChangeCloth((CustomizingCloth)data.Cloth);
        }
    }
}

