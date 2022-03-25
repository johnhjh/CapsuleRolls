using Capsule.Audio;
using Capsule.Lobby.Player;
using Capsule.SceneLoad;
using UnityEngine;

namespace Capsule.Lobby.Main
{
    public class MainLobbyManager : MonoBehaviour
    {
        private static MainLobbyManager mainLobbyMgr;

        public static MainLobbyManager Instance
        {
            get
            {
                if (mainLobbyMgr == null)
                    mainLobbyMgr = FindObjectOfType<MainLobbyManager>();
                return mainLobbyMgr;
            }
        }

        private void Awake()
        {
            if (mainLobbyMgr == null)
                mainLobbyMgr = this;
            else if (mainLobbyMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            BGMManager.Instance.ChangeBGM(BGMType.MAIN);
            SFXManager.Instance.PlayOneShot(MenuSFX.LOAD_DONE);
            SceneLoadManager.Instance.CurrentScene = LobbySceneType.MAIN_LOBBY;
            PlayerTransform.Instance.SetPosition(new Vector3(2.29f, -0.17f, -5.4f));
            PlayerTransform.Instance.SetRotation(Quaternion.Euler(19.94f, 202f, -4.7f));
            //PlayerTransform.Instance.SetScale(new Vector3(1.18f, 1.18f, 1.18f));
            PlayerTransform.Instance.SetScale(1.18f);
        }

        public void MoveToCustomizeScene()
        {
            MoveToScene(LobbySceneType.CUSTOMIZE);
        }

        public void MoveToSoloPlayScene()
        {
            Destroy(UserInfoManager.Instance.gameObject);
            MoveToScene(LobbySceneType.SOLO);
        }

        public void MoveToMultiPlayScene()
        {
            MoveToScene(LobbySceneType.MULTI);
        }

        public void MoveToShoppingScene()
        {
            MoveToScene(LobbySceneType.SHOPPING);
        }

        public void MoveToScene(LobbySceneType sceneType)
        {
            if (LobbySettingManager.Instance != null)
                Destroy(LobbySettingManager.Instance.gameObject);
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(sceneType, true));
        }

        public void MenuClick()
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.OK);
        }
    }
}
