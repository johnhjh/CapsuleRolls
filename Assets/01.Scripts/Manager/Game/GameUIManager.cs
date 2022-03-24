using Capsule.Entity;
using Capsule.SceneLoad;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

        [HideInInspector]
        public bool IsPaused { get; set; }
        [HideInInspector]
        public bool IsSetting { get; set; }
        [HideInInspector]
        public bool IsLoading { get; set; }
        [HideInInspector]
        public bool IsStagePopupActive { get; set; }
        [HideInInspector]
        public bool IsUIHover { get; set; }

        private CanvasGroup gamePauseCG = null;
        private CanvasGroup gameSettingCG = null;
        private CanvasGroup gameStageClearCG = null;
        private CanvasGroup gameStageFailureCG = null;

        private Text userInfoLevelText = null;
        private Text userInfoExpText = null;
        private Image userInfoExpImage = null;

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
            IsLoading = false;
            IsStagePopupActive = false;

            gamePauseCG = GameObject.Find("GameUIPause").GetComponent<CanvasGroup>();
            gameSettingCG = GameObject.Find("GameUISetting").GetComponent<CanvasGroup>();
            gameStageClearCG = GameObject.Find("GameUIStageClear").GetComponent<CanvasGroup>();
            gameStageFailureCG = GameObject.Find("GameUIStageFailure").GetComponent<CanvasGroup>();
            userInfoLevelText = GameObject.Find("User_Info_Level_Text").GetComponent<Text>();
            userInfoExpText = GameObject.Find("User_Info_Exp_Text").GetComponent<Text>();
            userInfoExpImage = GameObject.Find("User_Info_Exp_Fill").GetComponent<Image>();

            if (DataManager.Instance != null)
            {
                int currentLevel = DataManager.Instance.CurrentPlayerData.Level;
                int currentExp = DataManager.Instance.CurrentPlayerData.Exp;
                int requiredExp = LevelExpCalc.GetExpData(currentLevel + 1);
                userInfoLevelText.text = currentLevel.ToString();
                userInfoExpImage.fillAmount = (float)currentExp / requiredExp;
                userInfoExpText.text = currentExp.ToString() + "/" + requiredExp.ToString();
            }
        }

        public bool CheckUIActive()
        {
            return IsPaused || IsSetting || IsStagePopupActive || IsLoading;
        }

        private IEnumerator FadeInCG(CanvasGroup cg)
        {
            IsStagePopupActive = true;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            while (!Mathf.Approximately(cg.alpha, 1f))
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, 1f, 3f * Time.deltaTime);
                yield return null;
            }
            if (GameManager.Instance.CheckSoloGame())
                Time.timeScale = 0f;
        }

        public void OnStageClear()
        {
            StartCoroutine(FadeInCG(gameStageClearCG));
        }

        public void OnStageFailure()
        {
            StartCoroutine(FadeInCG(gameStageFailureCG));
        }

        public void PauseGame()
        {
            PauseGame(!IsPaused);
        }

        public void PauseGame(bool isPaused)
        {
            if (this.IsLoading || this.IsStagePopupActive) return;
            if (this.IsSetting)
                ShowGameSetting(false);
            this.IsPaused = isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            gamePauseCG.alpha = isPaused ? 1f : 0f;
            gamePauseCG.blocksRaycasts = isPaused;
            gamePauseCG.interactable = isPaused;
        }

        public void ShowGameSetting()
        {
            ShowGameSetting(!this.IsSetting);
        }

        public void ShowGameSetting(bool isSetting)
        {
            if (this.IsLoading || this.IsStagePopupActive) return;
            gameSettingCG.alpha = isSetting ? 1f : 0f;
            gameSettingCG.blocksRaycasts = isSetting;
            gameSettingCG.interactable = isSetting;
            this.IsSetting = isSetting;
        }

        public void OnClickSetting()
        {
            ShowGameSetting(true);
        }

        public void OnClickExitGame()
        {
            if (GameManager.Instance.CheckSoloGame())
            {
                Time.timeScale = 1f;
                MoveToScene(LobbySceneType.SOLO);
            }
            else
                MoveToScene(LobbySceneType.MULTI);
        }

        public void OnClickNextStage()
        {
            IsLoading = true;
            Time.timeScale = 1f;
            gameStageClearCG.alpha = 0f;
            gameStageClearCG.blocksRaycasts = false;
            gameStageClearCG.interactable = false;
            IsStagePopupActive = false;
            StartCoroutine(SceneLoadManager.Instance.LoadNextStageScene(GameManager.Instance.CurrentGameData));
        }

        public void OnClickRestartGame()
        {
            PauseGame(false);
            IsLoading = true;
            Time.timeScale = 1f;
            gameStageFailureCG.alpha = 0f;
            gameStageFailureCG.blocksRaycasts = false;
            gameStageFailureCG.interactable = false;
            IsStagePopupActive = false;
            StartCoroutine(SceneLoadManager.Instance.ReLoadStageScene(GameManager.Instance.CurrentGameData));
        }

        public void MoveToScene(LobbySceneType sceneType)
        {
            IsLoading = true;
            StartCoroutine(SceneLoadManager.Instance.LoadLobbySceneFromGame(sceneType));
        }
    }
}
