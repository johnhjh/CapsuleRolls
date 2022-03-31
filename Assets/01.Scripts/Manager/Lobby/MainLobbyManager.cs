using Capsule.Audio;
using Capsule.Lobby.Player;
using Capsule.Lobby.Title;
using Capsule.SceneLoad;
using System.Collections;
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

        private bool blocksButtons = false;

        private void Start()
        {
            BGMManager.Instance.ChangeBGM(BGMType.MAIN);
            SFXManager.Instance.PlayOneShot(MenuSFX.LOAD_DONE);
            SceneLoadManager.Instance.CurrentScene = LobbySceneType.MAIN_LOBBY;
            PlayerLobbyTransform.Instance.SetPosition(new Vector3(2.29f, -0.17f, -5.4f));
            PlayerLobbyTransform.Instance.SetRotation(Quaternion.Euler(19.94f, 202f, -4.7f));
            PlayerLobbyTransform.Instance.SetScale(1.18f);
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                blocksButtons = true;
                StartCoroutine(UnblockButtons());
            }
            else
                blocksButtons = false;
        }

        public IEnumerator UnblockButtons()
        {
            yield return new WaitForSeconds(1.0f);
            blocksButtons = false;
        }

        public void MoveToCustomizeScene()
        {
            if (blocksButtons) return;
            MoveToScene(LobbySceneType.CUSTOMIZE);
        }

        public void MoveToSoloPlayScene()
        {
            if (blocksButtons) return;
            Destroy(UserInfoManager.Instance.gameObject);
            MoveToScene(LobbySceneType.SOLO);
        }

        public void MoveToMultiPlayScene()
        {
            if (blocksButtons) return;
            MoveToScene(LobbySceneType.MULTI);
        }

        public void MoveToShoppingScene()
        {
            if (blocksButtons) return;
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
            if (blocksButtons) return;
            SFXManager.Instance.PlayOneShot(MenuSFX.OK);
        }
    }
}
