using Capsule.Audio;
using Capsule.Entity;
using Capsule.SceneLoad;
using System.Collections;
using System.Collections.Generic;
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
        public bool IsSettingOpen { get; set; }
        private bool isLoading = false;
        [HideInInspector]
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                if (!value)
                    SetGameUIInfo();
            }
        }
        [HideInInspector]
        public bool IsPopupActive { get; set; }
        [HideInInspector]
        public bool IsUIHover { get; set; }

        private readonly Color transparentColor = new Color(1f, 1f, 1f, 0f);
        private readonly Color visibleColor = new Color(1f, 1f, 1f, 1f);

        private CanvasGroup gamePauseCG = null;
        private CanvasGroup gameSettingCG = null;
        private CanvasGroup gameStageClearCG = null;
        private CanvasGroup gameStageFailureCG = null;
        private CanvasGroup gameArcadeFinishCG = null;
        private CanvasGroup labelNewRecordCG = null;

        private Text readyGoText = null;
        private readonly float MIN_READY_GO_FONT_SIZE = 200f;
        private readonly float MAX_READY_GO_FONT_SIZE = 250f;

        private Text labelStageClearText = null;
        private Text userInfoLevelText = null;
        private Text userInfoExpText = null;
        private Image userInfoExpImage = null;
        private Text pausePlayInfoText = null;

        private Button buttonPauseGame = null;
        private GameObject buttonClearExitGame = null;
        private GameObject buttonClearNextStage = null;
        private GameObject buttonClearReplayGame = null;
        private GameObject buttonClearAllCleared = null;
        private Coroutine stageFailureCoroutine = null;

        private GameObject timeSoloScoreBoard = null;
        private GameObject timeTeamScoreBoard = null;
        private GameObject timeOnlyBoard = null;

        private Text gameDescText = null;
        private Text soloTimeText = null;
        private Text teamTimeText = null;
        private Text onlyTimeText = null;

        private Text currentScoreText = null;
        private Text currentWaveText = null;
        private Text remainEnemyText = null;
        private Text arcadeTimeResultText = null;
        private Text arcadeScoreResultText = null;
        private Text arcadeCoinEarnedText = null;
        private Text timePlusEffectText = null;
        private Coroutine arcadeShowCoroutine;

        private List<GameObject> rewardItems = null;

        private int remainedTime = 100;
        private int passedTime = 0;
        public int CurrentPassedTime
        {
            get { return passedTime; }
        }
        private Coroutine timeCoroutine;
        private Coroutine lastTimeCoroutine;
        private Coroutine addTimeCoroutine;
        private Coroutine readyGoCoroutine;

        public event System.Action OnPauseGame;

        private void Awake()
        {
            if (gameUIMgr == null)
                gameUIMgr = this;
            else if (gameUIMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            InitGameUIComponents();
            if (DataManager.Instance != null)
                SetGameUIInfo();
        }

        private void InitGameUIComponents()
        {
            IsPaused = false;
            IsSettingOpen = false;
            IsLoading = true;
            IsPopupActive = false;

            gamePauseCG = GameObject.Find("GameUIPause").GetComponent<CanvasGroup>();
            gameSettingCG = GameObject.Find("GameUISetting").GetComponent<CanvasGroup>();
            gameStageClearCG = GameObject.Find("GameUIStageClear").GetComponent<CanvasGroup>();
            gameStageFailureCG = GameObject.Find("GameUIStageFailure").GetComponent<CanvasGroup>();
            gameArcadeFinishCG = GameObject.Find("GameUIArcadeFinish").GetComponent<CanvasGroup>();
            labelNewRecordCG = GameObject.Find("Label_NewRecord").GetComponent<CanvasGroup>();

            readyGoText = GameObject.Find("Ready_Go_Text").GetComponent<Text>();
            labelStageClearText = GameObject.Find("Label_StageClear_Text").GetComponent<Text>();
            userInfoLevelText = GameObject.Find("User_Info_Level_Text").GetComponent<Text>();
            userInfoExpText = GameObject.Find("User_Info_Exp_Text").GetComponent<Text>();
            userInfoExpImage = GameObject.Find("User_Info_Exp_Fill").GetComponent<Image>();
            pausePlayInfoText = GameObject.Find("Pause_PlayInfo_Text").GetComponent<Text>();

            buttonPauseGame = GameObject.Find("Button_Pause_Game").GetComponent<Button>();
            buttonPauseGame.onClick.AddListener(delegate { PauseGame(true); });

            buttonClearExitGame = GameObject.Find("Button_Clear_ExitGame");
            buttonClearNextStage = GameObject.Find("Button_Clear_NextStage");
            buttonClearReplayGame = GameObject.Find("Button_Clear_ReplayGame");
            buttonClearAllCleared = GameObject.Find("Button_Clear_AllCleared");

            rewardItems = new List<GameObject>();
            Transform rewardsItemsTransform = GameObject.Find("RewardItems").transform;
            for (int i = 0; i < rewardsItemsTransform.childCount; i++)
                rewardItems.Add(rewardsItemsTransform.GetChild(i).gameObject);
            foreach (GameObject gameObj in rewardItems)
                gameObj.SetActive(false);

            gameDescText = GameObject.Find("GameUIDesc_Text").GetComponent<Text>();
            timeSoloScoreBoard = GameObject.Find("Time_Solo_Score_Board");
            soloTimeText = timeSoloScoreBoard.transform.GetChild(0).GetComponent<Text>();
            timeTeamScoreBoard = GameObject.Find("Time_Team_Score_Board");
            teamTimeText = timeTeamScoreBoard.transform.GetChild(0).GetComponent<Text>();
            timeOnlyBoard = GameObject.Find("Time_Only_Board");
            onlyTimeText = timeOnlyBoard.transform.GetChild(0).GetComponent<Text>();
            timePlusEffectText = GameObject.Find("TimePlusEffectText").GetComponent<Text>();

            currentWaveText = timeSoloScoreBoard.transform.GetChild(1).GetComponent<Text>();
            remainEnemyText = timeSoloScoreBoard.transform.GetChild(2).GetComponent<Text>();
            currentScoreText = timeSoloScoreBoard.transform.GetChild(3).GetComponent<Text>();
            arcadeTimeResultText = GameObject.Find("ArcadeTimeResultText").GetComponent<Text>();
            arcadeScoreResultText = GameObject.Find("ArcadeScoreResultText").GetComponent<Text>();
            arcadeCoinEarnedText = GameObject.Find("ArcadeCoinEarnedText").GetComponent<Text>();
        }

        public void SetRewardData(RewardData reward)
        {
            rewardItems[(int)reward.kind].SetActive(true);
            switch (reward.kind)
            {
                case RewardKind.COIN:
                case RewardKind.EXP:
                    rewardItems[(int)reward.kind].transform.GetChild(1).GetComponent<Text>().text = reward.amount.ToString();
                    break;
                case RewardKind.CUSTOMIZING_BODY:
                case RewardKind.CUSTOMIZING_HEAD:
                case RewardKind.CUSTOMIZING_FACE:
                case RewardKind.CUSTOMIZING_GLOVE:
                case RewardKind.CUSTOMIZING_CLOTH:
                    rewardItems[(int)reward.kind].transform.GetChild(2).GetComponent<Image>().sprite = reward.preview;
                    break;
            }
        }

        private void SetGameUIInfo()
        {
            int currentLevel = DataManager.Instance.CurrentPlayerData.Level;
            int currentExp = DataManager.Instance.CurrentPlayerData.Exp;
            int requiredExp = LevelExpCalc.GetExpData(currentLevel + 1);
            userInfoLevelText.text = currentLevel.ToString();
            userInfoExpImage.fillAmount = (float)currentExp / requiredExp;
            userInfoExpText.text = currentExp.ToString() + "/" + requiredExp.ToString();
            timePlusEffectText.text = "";
            timePlusEffectText.color = transparentColor;
            passedTime = 0;
            if (timeCoroutine != null)
                StopCoroutine(timeCoroutine);
            if (lastTimeCoroutine != null)
                StopCoroutine(lastTimeCoroutine);
            if (addTimeCoroutine != null)
                StopCoroutine(addTimeCoroutine);
            if (readyGoCoroutine != null)
                StopCoroutine(ReadyGoEffect());
            readyGoCoroutine = StartCoroutine(ReadyGoEffect());
            switch (DataManager.Instance.CurrentGameData.Mode)
            {
                case GameMode.ARCADE:
                    remainedTime = 30;
                    CanvasGroupOff(labelNewRecordCG);
                    pausePlayInfoText.text = "아케이드 모드";
                    gameDescText.text = "아케이드모드\n최고점수 :<color=#FF6767>" +
                        DataManager.Instance.CurrentPlayerGameData.HighestScore.ToString() +
                        "</color>";
                    timeSoloScoreBoard.SetActive(true);
                    timeTeamScoreBoard.SetActive(false);
                    timeOnlyBoard.SetActive(false);
                    SetTextColorAndSize(soloTimeText, 1f, 80);
                    soloTimeText.text = remainedTime.ToString();
                    currentWaveText.text = "<color=#FF6767>웨이브</color> : 01";
                    remainEnemyText.text = "<color=#FF6767>남은 캡슐</color> : 01";
                    currentScoreText.text = "000";
                    arcadeCoinEarnedText.text = "000";
                    timeCoroutine = StartCoroutine(SetTimeUIText(soloTimeText));
                    break;
                case GameMode.STAGE:
                    remainedTime = 60;
                    labelStageClearText.text = DataManager.Instance.GetCurrentStageString() + " 클리어!";
                    pausePlayInfoText.text = "스테이지 모드 - " + DataManager.Instance.GetCurrentStageString();
                    gameDescText.text = "스테이지 모드 - " + DataManager.Instance.GetCurrentStageString();
                    timeSoloScoreBoard.SetActive(false);
                    timeTeamScoreBoard.SetActive(false);
                    timeOnlyBoard.SetActive(true);
                    onlyTimeText.text = remainedTime.ToString();
                    SetTextColorAndSize(onlyTimeText, 1f, 80);
                    switch (DataManager.Instance.CurrentGameData.Stage)
                    {
                        case GameStage.TUTORIAL_1:
                            ActionButton1Ctrl.Instance.IsActivated = false;
                            ActionButton2Ctrl.Instance.IsActivated = false;
                            onlyTimeText.text = "∞";
                            onlyTimeText.fontSize = 200;
                            break;
                        case GameStage.STAGE_1:
                            ActionButton1Ctrl.Instance.IsActivated = false;
                            ActionButton2Ctrl.Instance.IsActivated = false;
                            timeCoroutine = StartCoroutine(SetTimeUIText(onlyTimeText));
                            break;
                        case GameStage.TUTORIAL_2:
                            ActionButton1Ctrl.Instance.IsActivated = true;
                            ActionButton2Ctrl.Instance.IsActivated = false;
                            onlyTimeText.text = "∞";
                            onlyTimeText.fontSize = 200;
                            break;
                        case GameStage.STAGE_2:
                            ActionButton1Ctrl.Instance.IsActivated = true;
                            ActionButton2Ctrl.Instance.IsActivated = false;
                            timeCoroutine = StartCoroutine(SetTimeUIText(onlyTimeText));
                            break;
                        case GameStage.TUTORIAL_3:
                            ActionButton1Ctrl.Instance.IsActivated = true;
                            ActionButton2Ctrl.Instance.IsActivated = true;
                            onlyTimeText.text = "∞";
                            onlyTimeText.fontSize = 200;
                            break;
                        default:
                            ActionButton1Ctrl.Instance.IsActivated = true;
                            ActionButton2Ctrl.Instance.IsActivated = true;
                            timeCoroutine = StartCoroutine(SetTimeUIText(onlyTimeText));
                            break;
                    }
                    if (DataManager.Instance.HasNextStage())
                    {
                        buttonClearExitGame.SetActive(true);
                        buttonClearNextStage.SetActive(true);
                        if (buttonClearReplayGame.activeSelf)
                            buttonClearReplayGame.SetActive(false);
                        if (buttonClearAllCleared.activeSelf)
                            buttonClearAllCleared.SetActive(false);
                    }
                    else
                        StageAllClearedUI();
                    break;
                case GameMode.BOT:
                    remainedTime = 100;
                    pausePlayInfoText.text = "AI봇 대전 모드";
                    gameDescText.text = "AI봇 대전 모드";
                    timeSoloScoreBoard.SetActive(false);
                    timeTeamScoreBoard.SetActive(true);
                    timeOnlyBoard.SetActive(false);
                    teamTimeText.text = remainedTime.ToString();
                    timeCoroutine = StartCoroutine(SetTimeUIText(teamTimeText));
                    break;
                case GameMode.PRACTICE:
                    pausePlayInfoText.text = "연습 모드";
                    gameDescText.text = "연습 모드";
                    timeSoloScoreBoard.SetActive(false);
                    timeTeamScoreBoard.SetActive(false);
                    timeOnlyBoard.SetActive(true);
                    onlyTimeText.text = "∞";
                    onlyTimeText.fontSize = 200;
                    break;
            }
        }

        public void UpdateWaveText()
        {
            if (GameManager.Instance != null)
                currentWaveText.text = "<color=#FF6767>웨이브</color> : " +
                    GameManager.Instance.CurrentWave.ToString("00");
        }

        public void UpdateRemainedEnemeyText()
        {
            if (GameManager.Instance != null)
                remainEnemyText.text = "<color=#FF6767>남은 캡슐</color> : " +
                    GameManager.Instance.EnemyCount.ToString("00");
        }

        public void UpdateScoreText()
        {
            currentScoreText.text = GameManager.Instance.ArcadeScore.ToString("###,###,000");
        }

        private void UpdateTimeText(int timeAmount)
        {
            if (GameManager.Instance != null)
            {
                if (addTimeCoroutine != null)
                    StopCoroutine(addTimeCoroutine);
                switch (GameManager.Instance.CurrentGameData.Mode)
                {
                    case GameMode.ARCADE:
                        addTimeCoroutine = StartCoroutine(AddTimeTextEffect(soloTimeText, timeAmount));
                        soloTimeText.text = remainedTime.ToString();
                        break;
                    case GameMode.STAGE:
                        onlyTimeText.text = remainedTime.ToString();
                        addTimeCoroutine = StartCoroutine(AddTimeTextEffect(onlyTimeText, timeAmount));
                        break;
                    case GameMode.BOT:
                        teamTimeText.text = remainedTime.ToString();
                        addTimeCoroutine = StartCoroutine(AddTimeTextEffect(teamTimeText, timeAmount));
                        break;
                    case GameMode.PRACTICE:
                        break;
                }
            }
        }

        public void AddTime(int timeAmount)
        {
            remainedTime += timeAmount;
            SFXManager.Instance.PlaySFX(MenuSFX.OK);
            UpdateTimeText(timeAmount);
        }

        private IEnumerator AddTimeTextEffect(Text text, int timeAmount)
        {
            timePlusEffectText.text = "+" + timeAmount.ToString();
            timePlusEffectText.color = visibleColor;
            float currentAlpha = 0.3f;
            float currentFontSize = 120f;
            float curAlphaSpeed = 1f / 0.7f;
            while (!Mathf.Approximately(currentFontSize, 80f))
            {
                currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, curAlphaSpeed * Time.deltaTime);
                currentFontSize = Mathf.MoveTowards(currentFontSize, 80f, 40f * Time.deltaTime);
                SetTextColorAndSize(text, currentAlpha, Mathf.RoundToInt(currentFontSize));
                yield return null;
            }
            SetTextColorAndSize(text, 1f, 80);
            timePlusEffectText.text = "";
            timePlusEffectText.color = transparentColor;
        }

        public void StageAllClearedUI()
        {
            labelStageClearText.text = "스테이지 올 클리어!!";
            if (buttonClearExitGame.activeSelf)
                buttonClearExitGame.SetActive(false);
            if (buttonClearNextStage.activeSelf)
                buttonClearNextStage.SetActive(false);
            buttonClearReplayGame.SetActive(true);
            buttonClearAllCleared.SetActive(true);
        }

        public bool CheckUIActive()
        {
            return IsPaused || IsSettingOpen || IsPopupActive || IsLoading;
        }

        private IEnumerator SetTimeUIText(Text timeText)
        {
            yield return new WaitForSeconds(3.0f);
            while (!GameManager.Instance.IsGameOver && remainedTime-- >= 0)
            {
                yield return new WaitForSeconds(1.0f);
                if (lastTimeCoroutine != null)
                    StopCoroutine(lastTimeCoroutine);
                SetTextColorAndSize(timeText, 1f, 80);
                passedTime++;
                timeText.text = remainedTime.ToString();
                if (remainedTime == 0)
                {
                    GameManager.Instance.TimeEnded();
                    SFXManager.Instance.PlayOneShot(Announcements.TIMES_UP);
                    yield break;
                }
                else if (remainedTime <= 3)
                {
                    SFXManager.Instance.PlayOneShot(Announcements.COUNT, remainedTime);
                    if (lastTimeCoroutine != null)
                        StopCoroutine(lastTimeCoroutine);
                    lastTimeCoroutine = StartCoroutine(Last3SecTextEffect(timeText));
                }
            }
        }

        private void SetTextColorAndSize(Text text, float alpha, int size)
        {
            text.color = new Color(1f, 1f, 1f, alpha);
            text.fontSize = size;
        }

        private IEnumerator ReadyGoEffect()
        {
            readyGoText.text = "Ready~";
            readyGoText.fontSize = Mathf.RoundToInt(MIN_READY_GO_FONT_SIZE);
            readyGoText.color = transparentColor;
            yield return new WaitForSeconds(1.0f);
            float currentAlpha = 0f;
            float currentFontSize = MIN_READY_GO_FONT_SIZE;
            float speed = MAX_READY_GO_FONT_SIZE - MIN_READY_GO_FONT_SIZE;
            while(!Mathf.Approximately(currentFontSize, MAX_READY_GO_FONT_SIZE))
            {
                currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime);
                currentFontSize = Mathf.MoveTowards(currentFontSize, MAX_READY_GO_FONT_SIZE, speed * Time.deltaTime);
                SetTextColorAndSize(readyGoText, currentAlpha, Mathf.RoundToInt(currentFontSize));
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
            currentAlpha = 0f;
            currentFontSize = MIN_READY_GO_FONT_SIZE;
            readyGoText.color = transparentColor;
            readyGoText.text = "Go~!!";
            readyGoText.fontSize = Mathf.RoundToInt(MIN_READY_GO_FONT_SIZE);
            while (!Mathf.Approximately(currentFontSize, MAX_READY_GO_FONT_SIZE))
            {
                currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime);
                currentFontSize = Mathf.MoveTowards(currentFontSize, MAX_READY_GO_FONT_SIZE, speed * Time.deltaTime);
                SetTextColorAndSize(readyGoText, currentAlpha, Mathf.RoundToInt(currentFontSize));
                yield return null;
            }
            yield return null;
            readyGoText.color = transparentColor;
        }

        private IEnumerator Last3SecTextEffect(Text text)
        {
            float currentAlpha = 0f;
            float currentFontSize = 250f;
            while (!Mathf.Approximately(currentAlpha, 1f))
            {
                currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime);
                currentFontSize = Mathf.MoveTowards(currentFontSize, 80f, 170f * Time.deltaTime);
                SetTextColorAndSize(text, currentAlpha, Mathf.RoundToInt(currentFontSize));
                yield return null;
            }
        }

        private IEnumerator FadeInCG(CanvasGroup cg)
        {
            IsPopupActive = true;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            while (!Mathf.Approximately(cg.alpha, 1f))
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, 1f, 3f * Time.deltaTime);
                yield return null;
            }
            //if (GameManager.Instance.CheckSoloGame() && GameManager.Instance.CurrentGameData.Mode != GameMode.ARCADE)
            //Time.timeScale = 0f;
        }

        public void OnArcadeFinished()
        {
            if (addTimeCoroutine != null)
                StopCoroutine(addTimeCoroutine);
            if (timeCoroutine != null)
                StopCoroutine(timeCoroutine);
            arcadeShowCoroutine = StartCoroutine(ArcadeScoreShow());
        }

        private IEnumerator ArcadeScoreShow()
        {
            arcadeTimeResultText.text = "생존시간 : 0초";
            arcadeScoreResultText.text = "점수 : 0점";
            yield return StartCoroutine(FadeInCG(gameArcadeFinishCG));
            float currentT = 0f;
            float currentS = 0f;
            float passedTimeSpeed = passedTime / 1.5f;
            float scoringSpeed = GameManager.Instance.ArcadeScore / 1.5f;
            while (!Mathf.Approximately(currentT, passedTime))
            {
                currentT = Mathf.MoveTowards(currentT, passedTime, passedTimeSpeed * Time.deltaTime);
                currentS = Mathf.MoveTowards(currentS, GameManager.Instance.ArcadeScore, scoringSpeed * Time.deltaTime);
                arcadeTimeResultText.text = "생존시간 : " + Mathf.RoundToInt(currentT).ToString("00") + "초";
                arcadeScoreResultText.text = "점수 : " + Mathf.RoundToInt(currentS).ToString("000") + "점";
                yield return null;
            }
            float currentC = 0f;
            int finalC = passedTime + Mathf.RoundToInt(GameManager.Instance.ArcadeScore / 10f);
            float coinSpeed = finalC / 1.5f;
            while (!Mathf.Approximately(currentC, finalC))
            {
                currentC = Mathf.MoveTowards(currentC, finalC, coinSpeed * Time.deltaTime);
                arcadeCoinEarnedText.text = Mathf.RoundToInt(currentC).ToString("###,###,000");
                yield return null;
            }
            SFXManager.Instance.PlaySFX(MenuSFX.BUY);
            arcadeCoinEarnedText.text = Mathf.RoundToInt(finalC).ToString("###,###,000") + " ~";
        }

        public void OnArcadeNewRecord()
        {
            StartCoroutine(FadeInCG(labelNewRecordCG));
        }

        public void OnStageClear()
        {
            if (timeCoroutine != null)
                StopCoroutine(timeCoroutine);
            StartCoroutine(FadeInCG(gameStageClearCG));
        }

        public void OnStageFailure()
        {
            if (timeCoroutine != null)
                StopCoroutine(timeCoroutine);
            stageFailureCoroutine = StartCoroutine(FadeInCG(gameStageFailureCG));
        }

        public void PauseGame()
        {
            PauseGame(!IsPaused);
        }

        public void PauseGame(bool isPaused)
        {
            if (this.IsLoading || this.IsPopupActive) return;
            if (isPaused) OnPauseGame?.Invoke();
            if (this.IsSettingOpen)
                ShowGameSetting(false);
            this.IsPaused = isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            gamePauseCG.alpha = isPaused ? 1f : 0f;
            gamePauseCG.blocksRaycasts = isPaused;
            gamePauseCG.interactable = isPaused;
        }

        public void ShowGameSetting()
        {
            ShowGameSetting(!this.IsSettingOpen);
        }

        public void ShowGameSetting(bool isSetting)
        {
            if (this.IsLoading || this.IsPopupActive) return;
            gameSettingCG.alpha = isSetting ? 1f : 0f;
            gameSettingCG.blocksRaycasts = isSetting;
            gameSettingCG.interactable = isSetting;
            this.IsSettingOpen = isSetting;
        }

        public void OnClickSetting()
        {
            ShowGameSetting(true);
        }

        public void OnClickExitGame()
        {
            if (OnPauseGame != null)
                OnPauseGame = null;
            if (GameManager.Instance.CheckSoloGame())
            {
                if (addTimeCoroutine != null)
                    StopCoroutine(addTimeCoroutine);
                if (arcadeShowCoroutine != null)
                    StopCoroutine(arcadeShowCoroutine);
                if (stageFailureCoroutine != null)
                    StopCoroutine(stageFailureCoroutine);
                if (lastTimeCoroutine != null)
                    StopCoroutine(lastTimeCoroutine);
                Time.timeScale = 1f;
                MoveToScene(LobbySceneType.SOLO);
            }
            else
                MoveToScene(LobbySceneType.MULTI);
        }

        public void OnClickExitGameToCredit()
        {
            Time.timeScale = 1f;
            MoveToScene(LobbySceneType.CREDIT);
        }

        public void OnClickNextStage()
        {
            if (OnPauseGame != null)
                OnPauseGame = null;
            foreach (GameObject gameObj in rewardItems)
                gameObj.SetActive(false);
            if (!DataManager.Instance.HasNextStage(GameManager.Instance.CurrentGameData.Stage))
            {
                SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
                StageAllClearedUI();
                return;
            }
            IsLoading = true;
            Time.timeScale = 1f;
            CanvasGroupOff(gameStageClearCG);
            IsPopupActive = false;
            StartCoroutine(SceneLoadManager.Instance.LoadNextStageScene(GameManager.Instance.CurrentGameData));
        }

        private void CanvasGroupOff(CanvasGroup cg)
        {
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        public void OnClickRestartGame()
        {
            if (OnPauseGame != null)
                OnPauseGame = null;
            if (addTimeCoroutine != null)
                StopCoroutine(addTimeCoroutine);
            if (arcadeShowCoroutine != null)
                StopCoroutine(arcadeShowCoroutine);
            if (stageFailureCoroutine != null)
                StopCoroutine(stageFailureCoroutine);
            if (lastTimeCoroutine != null)
                StopCoroutine(lastTimeCoroutine);
            PauseGame(false);
            IsPopupActive = false;
            IsLoading = true;
            Time.timeScale = 1f;
            switch (GameManager.Instance.CurrentGameData.Mode)
            {
                case GameMode.ARCADE:
                    if (arcadeShowCoroutine != null)
                        StopCoroutine(arcadeShowCoroutine);
                    CanvasGroupOff(labelNewRecordCG);
                    CanvasGroupOff(gameArcadeFinishCG);
                    if (Enemy.EnemySpawnManager.Instance != null)
                        Destroy(Enemy.EnemySpawnManager.Instance.gameObject);
                    StartCoroutine(SceneLoadManager.Instance.ReLoadGameScene(GameManager.Instance.CurrentGameData));
                    break;
                case GameMode.STAGE:
                    CanvasGroupOff(gameStageClearCG);
                    CanvasGroupOff(gameStageFailureCG);
                    StartCoroutine(SceneLoadManager.Instance.ReLoadStageScene(GameManager.Instance.CurrentGameData));
                    break;
                case GameMode.PRACTICE:
                    StartCoroutine(SceneLoadManager.Instance.ReLoadGameScene(GameManager.Instance.CurrentGameData));
                    break;
                case GameMode.BOT:
                    StartCoroutine(SceneLoadManager.Instance.ReLoadGameScene(GameManager.Instance.CurrentGameData));
                    break;
            }
        }

        public void MoveToScene(LobbySceneType sceneType)
        {
            Destroy(userInfoLevelText.gameObject);
            Destroy(userInfoExpText.gameObject);
            Destroy(userInfoExpImage.gameObject);
            if (GameSettingManager.Instance != null)
                Destroy(GameSettingManager.Instance);
            if (Effect.EffectQueueManager.Instance != null)
                Destroy(Effect.EffectQueueManager.Instance.gameObject);
            if (Enemy.EnemySpawnManager.Instance != null)
                Destroy(Enemy.EnemySpawnManager.Instance.gameObject);
            IsLoading = true;
            StartCoroutine(SceneLoadManager.Instance.LoadLobbySceneFromGame(sceneType));
        }
    }
}
