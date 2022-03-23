using Capsule.SceneLoad;
using UnityEngine;

namespace Capsule.Game.UI
{
    public class GameUIManager : MonoBehaviour
    {
        private static GameUIManager gameUIMgr;
        public static GameUIManager Instance
        {
            get
            {
                if (gameUIMgr == null)
                    gameUIMgr = FindObjectOfType<GameUIManager>();
                return gameUIMgr;
            }
        }

        public bool IsPaused { get; set; }
        public bool IsSetting { get; set; }

        private CanvasGroup gamePauseCanvasGroup = null;
        private CanvasGroup gameSettingCanvasGroup = null;

        private void Awake()
        {
            if (gameUIMgr == null)
                gameUIMgr = this;
            else if (gameUIMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            IsPaused = false;
            IsSetting = false;
            gamePauseCanvasGroup = GameObject.Find("GameUIPause").GetComponent<CanvasGroup>();
            gameSettingCanvasGroup = GameObject.Find("GameUISetting").GetComponent<CanvasGroup>();

        }

        public void OnClickSetting()
        {
            ShowGameSetting(true);
        }

        public void OnClickExitGame()
        {
            if (GameManager.Instance.CheckSoloGame())
                MoveToScene(LobbySceneType.SOLO);
            else
                MoveToScene(LobbySceneType.MULTI);
        }

        public void MoveToScene(LobbySceneType sceneType)
        {
            StartCoroutine(SceneLoadManager.Instance.LoadLobbySceneFromGame(sceneType));
        }

        public void PauseGame()
        {
            PauseGame(!IsPaused);
        }

        public void PauseGame(bool isPaused)
        {
            if (this.IsSetting)
                ShowGameSetting(false);
            this.IsPaused = isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            gamePauseCanvasGroup.alpha = isPaused ? 1f : 0f;
            gamePauseCanvasGroup.blocksRaycasts = isPaused;
            gamePauseCanvasGroup.interactable = isPaused;
        }

        public void ShowGameSetting()
        {
            ShowGameSetting(!this.IsSetting);
        }

        public void ShowGameSetting(bool isSetting)
        {
            this.IsSetting = isSetting;
            gameSettingCanvasGroup.alpha = isSetting ? 1f : 0f;
            gameSettingCanvasGroup.blocksRaycasts = isSetting;
            gameSettingCanvasGroup.interactable = isSetting;
        }
    }
}
