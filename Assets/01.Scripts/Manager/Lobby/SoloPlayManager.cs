using Capsule.Audio;
using Capsule.Entity;
using Capsule.Lobby.Player;
using Capsule.SceneLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.Lobby.SoloPlay
{
    public class SoloPlayManager : MonoBehaviour
    {
        private static SoloPlayManager soloPlayMgr;
        public static SoloPlayManager Instance
        {
            get
            {
                if (soloPlayMgr == null)
                    soloPlayMgr = FindObjectOfType<SoloPlayManager>();
                return soloPlayMgr;
            }
        }

        private GameData gameData = new GameData();
        private RectTransform gameSettingUIList;
        private Button buttonStartGame;

        private Text gameModeText;
        private Text gameModeDetailText;
        private Text gameKindDetailText;
        private Image gameKindDetailImage;
        private Text gameScoreDetailText;
        private Text gameStageDetailText;
        private Text gameHighestStageDetailText;

        private GameObject gameScoreUI;
        private GameObject gameKindUI;
        private GameObject gameHighestStageUI;
        private GameObject gameStageSelectUI;
        private GameObject gameBotDifficultyUI;

        private Button gameBotDifficultyEasy;
        private Button gameBotDifficultyMedium;
        private Button gameBotDifficultyHard;
        private RectTransform gameBotDifficultySelected;

        // Game Kind Select PopUp
        private bool gameKindSelectPopupIsOpened = false;
        private CanvasGroup gameKindSelectPopupCG;
        private GameKindSlot currentKindSlot = null;
        public GameKindSlot CurrentKindSlot
        {
            get { return currentKindSlot; }
            set
            {
                if (currentKindSlot != null)
                {
                    currentKindSlot.IsSelected = false;
                    currentKindSlot.CancelSelect();
                }
                currentKindSlot = value;
            }
        }

        // Game Stage Select PopUp
        private bool gameStageSelectPopupIsOpened = false;
        private CanvasGroup gameStageSelectPopupCG;
        public Sprite unlockedStageSlot;
        public Sprite lockedStageSlot;
        public Sprite focusedStageSlot;
        private Color lockedStageColor = new Color(0.3921569f, 0.4941176f, 0.5686275f, 1f);
        public Color LockedStageTextColor { get { return lockedStageColor; } }
        private Text gameStageSelectPopupDetailName;
        private Image gameStageSelectPopupDetailKindPreview;
        private Text gameStageSelectPopupDetailKindDesc;
        private Text gameStageSelectPopupDetailDesc;
        private List<Transform> gameStageSelectPopupDetailRewardList;

        private GameStageSlot currentStageSlot = null;
        public GameStageSlot CurrentStageSlot
        {
            get { return currentStageSlot; }
            set
            {
                if (currentStageSlot != null)
                {
                    currentStageSlot.IsSelected = false;
                    currentStageSlot.CancelSelect();
                }
                currentStageSlot = value;
                SetGameStageSelectPopupDetail(value);
            }
        }
        public GameStageSlot CurrentHoverStageSlot
        {
            set { SetGameStageSelectPopupDetail(value); }
        }

        public void SetGameStageSelectPopupDetail(GameStageSlot stageSlot)
        {
            if (stageSlot == null)
            {
                gameStageSelectPopupDetailName.text = CurrentStageSlot.data.name;
                gameStageSelectPopupDetailKindPreview.sprite = DataManager.Instance.GameKindDatas[(int)CurrentStageSlot.data.kind].preview;
                gameStageSelectPopupDetailKindDesc.text = DataManager.Instance.GameKindDatas[(int)CurrentStageSlot.data.kind].name;
                gameStageSelectPopupDetailDesc.text = CurrentStageSlot.data.desc;
                SetGameStageSelectPopulRewardDetail(CurrentStageSlot);
            }
            else
            {
                gameStageSelectPopupDetailName.text = stageSlot.data.name;
                gameStageSelectPopupDetailKindPreview.sprite = DataManager.Instance.GameKindDatas[(int)stageSlot.data.kind].preview;
                gameStageSelectPopupDetailKindDesc.text = DataManager.Instance.GameKindDatas[(int)stageSlot.data.kind].name;
                gameStageSelectPopupDetailDesc.text = stageSlot.data.desc;
                SetGameStageSelectPopulRewardDetail(stageSlot);
            }
        }

        private void RewardListSetActiveFalse()
        {
            for (int i = 0; i < gameStageSelectPopupDetailRewardList.Count; i++)
                gameStageSelectPopupDetailRewardList[i].gameObject.SetActive(false);
        }

        private void SetGameStageSelectPopulRewardDetail(GameStageSlot stageSlot)
        {
            RewardListSetActiveFalse();
            foreach (RewardData data in stageSlot.data.rewards)
            {
                gameStageSelectPopupDetailRewardList[(int)data.kind].gameObject.SetActive(true);
                if (data.preview != null)
                    gameStageSelectPopupDetailRewardList[(int)data.kind].GetChild(0).GetComponent<Image>().sprite = data.preview;
                if (data.kind == RewardKind.COIN || data.kind == RewardKind.EXP)
                    gameStageSelectPopupDetailRewardList[(int)data.kind].GetChild(1).GetComponent<Text>().text = data.amount.ToString();
            }
        }

        // Game Mode Not Available
        private CanvasGroup notAvailableCG = null;

        private void ShowHideNotAvailable(bool isShow)
        {
            notAvailableCG.alpha = isShow ? 1f : 0f;
            notAvailableCG.blocksRaycasts = isShow;
            notAvailableCG.interactable = isShow;
        }

        private void ShowHideNotAvailable(bool isShow, GameMode mode)
        {
            switch (mode)
            {
                case GameMode.ARCADE:
                    notAvailableCG.transform.GetChild(0).GetComponent<Text>().text = "스테이지 모드를\n먼저 진행해 주세요.\n\n[튜토리얼 3] 클리어시 개방!";
                    break;
                case GameMode.PRACTICE:
                    notAvailableCG.transform.GetChild(0).GetComponent<Text>().text = "스테이지 모드를\n먼저 진행해 주세요.\n\n[스테이지 4] 클리어시 개방!";
                    break;
                case GameMode.BOT:
                    break;
            }
            ShowHideNotAvailable(isShow);
        }

        private void Awake()
        {
            if (soloPlayMgr == null)
                soloPlayMgr = this;
            else if (soloPlayMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            BGMManager.Instance.ChangeBGM(BGMType.BATTLE);
            SFXManager.Instance.PlayOneShot(MenuSFX.LOAD_DONE);
            SceneLoadManager.Instance.CurrentScene = LobbySceneType.SOLO;
            PlayerLobbyTransform.Instance.SetPosition(new Vector3(0.07f, -0.4f, -4.34f));
            PlayerLobbyTransform.Instance.SetRotation(Quaternion.Euler(8.2f, 177.6f, 0f));
            PlayerLobbyTransform.Instance.SetScale(1.18f);

            gameSettingUIList = GameObject.Find("GameSettingUIList").GetComponent<RectTransform>();
            buttonStartGame = GameObject.Find("Button_Start").GetComponent<Button>();
            buttonStartGame.onClick.AddListener(delegate { StartSoloGame(); });
            gameModeText = GameObject.Find("GameModeText").GetComponent<Text>();
            gameModeDetailText = GameObject.Find("GameModeDetailText").GetComponent<Text>();
            gameKindDetailText = GameObject.Find("GameKindDetailText").GetComponent<Text>();
            gameKindDetailImage = GameObject.Find("GameKindDetailImage").GetComponent<Image>();
            gameKindUI = GameObject.Find("GameKindUI");
            // Arcade
            gameScoreUI = GameObject.Find("GameScoreUI");
            gameScoreDetailText = gameScoreUI.transform.GetChild(2).GetComponent<Text>();
            gameScoreDetailText.text = DataManager.Instance.CurrentPlayerGameData.HighestScore.ToString() + " 점";
            // Stage
            gameStageSelectUI = GameObject.Find("GameStageSelectUI");
            gameStageDetailText = GameObject.Find("GameStageDetailText").GetComponent<Text>();
            gameStageDetailText.text = DataManager.Instance.GetNextStageString();
            gameHighestStageUI = GameObject.Find("GameHighestStageUI");
            gameHighestStageDetailText = GameObject.Find("GameHighestStageDetailText").GetComponent<Text>();
            gameHighestStageDetailText.text = DataManager.Instance.GetHighestStageString();
            // Bot
            gameBotDifficultyUI = GameObject.Find("GameBotDifficultyUI");
            gameBotDifficultySelected = GameObject.Find("GameBotDifficultySelected").GetComponent<RectTransform>();
            gameBotDifficultyEasy = GameObject.Find("GameBotDifficultyEasy").GetComponent<Button>();
            gameBotDifficultyEasy.onClick.AddListener(
                delegate { ChangeAIDifficulty(AIDifficulty.EASY, gameBotDifficultyEasy.transform); });
            gameBotDifficultyMedium = GameObject.Find("GameBotDifficultyMedium").GetComponent<Button>();
            gameBotDifficultyMedium.onClick.AddListener(
                delegate { ChangeAIDifficulty(AIDifficulty.MEDIUM, gameBotDifficultyMedium.transform); });
            gameBotDifficultyHard = GameObject.Find("GameBotDifficultyHard").GetComponent<Button>();
            gameBotDifficultyHard.onClick.AddListener(
                delegate { ChangeAIDifficulty(AIDifficulty.HARD, gameBotDifficultyHard.transform); });

            // Popups
            gameKindSelectPopupCG = GameObject.Find("Popup_GameKind_Select").GetComponent<CanvasGroup>();
            gameStageSelectPopupCG = GameObject.Find("Popup_GameStage_Select").GetComponent<CanvasGroup>();
            gameStageSelectPopupDetailName = GameObject.Find("Popup_GameStage_Detail_Name").GetComponent<Text>();
            gameStageSelectPopupDetailKindPreview = GameObject.Find("Popup_GameStage_Detail_Kind_Preview").GetComponent<Image>();
            gameStageSelectPopupDetailKindDesc = GameObject.Find("Popup_GameStage_Detail_Kind_Desc").GetComponent<Text>();
            gameStageSelectPopupDetailDesc = GameObject.Find("Popup_GameStage_Detail_Desc").GetComponent<Text>();

            // Reward List
            GameObject rewardListGameObj = GameObject.Find("Popup_GameStage_Detail_Reward_List");
            gameStageSelectPopupDetailRewardList = new List<Transform>();
            for (int i = 0; i < rewardListGameObj.transform.childCount; i++)
                gameStageSelectPopupDetailRewardList.Add(
                    rewardListGameObj.transform.GetChild(i));

            MainMenuCtrl stageButtonCtrl = GameObject.Find("Button_Stage").GetComponent<MainMenuCtrl>();
            stageButtonCtrl.IsSelected = true;
            stageButtonCtrl.finalAlpha = 1f;
            stageButtonCtrl.finalFontSize = 105f;

            GameKindSlot kindSlot = GameObject.Find("Game_Kind_List").transform.GetChild(0).GetComponent<GameKindSlot>();
            kindSlot.SelectSlot();

            gameData.Difficulty = AIDifficulty.EASY;

            // Not Available Setting
            notAvailableCG = GameObject.Find("NotAvailableUI").GetComponent<CanvasGroup>();
            ShowHideNotAvailable(false);

            SelectGameMode(GameMode.STAGE);
            SelectGameKind(GameKind.GOAL_IN);

            StartCoroutine(InitStageSlots());
        }

        private void Update()
        {
            if (!gameKindSelectPopupIsOpened && !gameStageSelectPopupIsOpened) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gameKindSelectPopupIsOpened)
                    PopupGameKindSelect(false);
                if (gameStageSelectPopupIsOpened)
                    PopupGameStageSelect(false);
            }
        }

        private IEnumerator InitStageSlots()
        {
            yield return new WaitForSeconds(1.0f);
            int counter = 0;
            Transform stageSlotContents = GameObject.Find("StageSlotContents").transform;

            int nextStage = DataManager.Instance.CurrentPlayerGameData.HighestStage;
            if (DataManager.Instance.HasNextStage(nextStage))
                nextStage++;
            foreach (GameStageSlot slot in stageSlotContents.GetComponentsInChildren<GameStageSlot>())
            {
                slot.data = DataManager.Instance.GameStageDatas[counter];
                if (counter == nextStage)
                {
                    slot.IsLocked = false;
                    slot.SelectSlot();
                    counter++;
                    continue;
                }
                slot.IsLocked = !DataManager.Instance.CurrentPlayerStageClearData.ClearData[counter];
                counter++;
            }
            gameData.Stage = (GameStage)nextStage;
        }

        private void ChangeAIDifficulty(AIDifficulty difficulty, Transform parent)
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.OK);
            gameData.Difficulty = difficulty;
            gameBotDifficultySelected.SetParent(parent);
            gameBotDifficultySelected.localPosition = new Vector2(100, -50);
        }

        public void PopupGameKindSelect(bool isOpen)
        {
            if (LobbySettingManager.Instance != null)
                LobbySettingManager.Instance.OtherOpened = isOpen;
            gameKindSelectPopupIsOpened = isOpen;
            SFXManager.Instance.PlayOneShot(isOpen ? MenuSFX.OK : MenuSFX.BACK);
            gameKindSelectPopupCG.alpha = isOpen ? 1.0f : 0f;
            gameKindSelectPopupCG.blocksRaycasts = isOpen;
            gameKindSelectPopupCG.interactable = isOpen;
        }

        public void PopupGameStageSelect(bool isOpen)
        {
            if (LobbySettingManager.Instance != null)
                LobbySettingManager.Instance.OtherOpened = isOpen;
            gameStageSelectPopupIsOpened = isOpen;
            SFXManager.Instance.PlayOneShot(isOpen ? MenuSFX.OK : MenuSFX.BACK);
            gameStageSelectPopupCG.alpha = isOpen ? 1.0f : 0f;
            gameStageSelectPopupCG.blocksRaycasts = isOpen;
            gameStageSelectPopupCG.interactable = isOpen;
        }

        public void SelectGameMode(GameMode mode)
        {
            gameSettingUIList.position -= 800f * Vector3.up;
            gameData.Mode = mode;
            gameModeText.text = DataManager.Instance.gameModeDatas[(int)mode].name;
            gameModeDetailText.text = DataManager.Instance.gameModeDatas[(int)mode].desc;
            switch (mode)
            {
                case GameMode.ARCADE:
                    gameKindUI.SetActive(true);
                    gameScoreUI.SetActive(true);
                    gameHighestStageUI.SetActive(false);
                    gameStageSelectUI.SetActive(false);
                    gameBotDifficultyUI.SetActive(false);
                    break;
                case GameMode.STAGE:
                    gameKindUI.SetActive(false);
                    gameScoreUI.SetActive(false);
                    gameHighestStageUI.SetActive(true);
                    gameStageSelectUI.SetActive(true);
                    gameBotDifficultyUI.SetActive(false);
                    break;
                case GameMode.PRACTICE:
                    gameKindUI.SetActive(false);
                    gameScoreUI.SetActive(false);
                    gameHighestStageUI.SetActive(false);
                    gameStageSelectUI.SetActive(false);
                    gameBotDifficultyUI.SetActive(false);
                    break;
                case GameMode.BOT:
                    gameKindUI.SetActive(true);
                    gameScoreUI.SetActive(false);
                    gameHighestStageUI.SetActive(false);
                    gameStageSelectUI.SetActive(false);
                    gameBotDifficultyUI.SetActive(true);
                    break;
                default:
                    gameKindUI.SetActive(true);
                    gameScoreUI.SetActive(true);
                    gameHighestStageUI.SetActive(false);
                    gameStageSelectUI.SetActive(false);
                    gameBotDifficultyUI.SetActive(false);
                    break;
            }
        }

        public void OnClickConfirmSelectKind()
        {
            if (CurrentKindSlot != null)
                SelectGameKind(CurrentKindSlot.kind);
            SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            gameKindSelectPopupCG.alpha = 0f;
            gameKindSelectPopupCG.blocksRaycasts = false;
            gameKindSelectPopupCG.interactable = false;
        }

        public void SelectGameKind(GameKind kind)
        {
            gameData.Kind = kind;
            gameKindDetailText.text = DataManager.Instance.GameKindDatas[(int)kind].name;
            gameKindDetailImage.sprite = DataManager.Instance.GameKindDatas[(int)kind].preview;
        }

        public void OnClickConfirmSelectStage()
        {
            if (CurrentStageSlot != null)
                SelectGameStage(CurrentStageSlot.stage);
            SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            gameStageSelectPopupCG.alpha = 0f;
            gameStageSelectPopupCG.blocksRaycasts = false;
            gameStageSelectPopupCG.interactable = false;
        }

        public void SelectGameStage(GameStage stage)
        {
            gameData.Stage = stage;
            gameStageDetailText.text = DataManager.Instance.GameStageDatas[(int)stage].name;
        }

        public void OnClickAnyButton()
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
        }

        public void OnClickButtonArcade()
        {
            OnClickAnyButton();
            if (PlayerPrefs.GetInt("IsArcadeModeOpen", 0) == 0)
            {
                ShowHideNotAvailable(true, GameMode.ARCADE);
                StartButtonAvailable(false);
            }
            else
                StartButtonAvailable(true);
            SelectGameKind(GameKind.BATTLE_ROYAL);
            SelectGameMode(GameMode.ARCADE);
        }

        public void OnClickButtonStage()
        {
            OnClickAnyButton();
            StartButtonAvailable(true);
            SelectGameKind(GameKind.GOAL_IN);
            SelectGameMode(GameMode.STAGE);
        }

        public void OnClickButtonPractice()
        {
            OnClickAnyButton();
            if (PlayerPrefs.GetInt("IsPracticeModeOpen", 0) == 0)
            {
                ShowHideNotAvailable(true, GameMode.PRACTICE);
                StartButtonAvailable(false);
            }
            else
                StartButtonAvailable(true);
            SelectGameMode(GameMode.PRACTICE);
        }

        public void OnClickButtonBot()
        {
            OnClickAnyButton();
            if (PlayerPrefs.GetInt("IsBotModeOpen", 0) == 0)
            {
                ShowHideNotAvailable(true, GameMode.BOT);
                StartButtonAvailable(false);
            }
            else
                StartButtonAvailable(true);
            SelectGameMode(GameMode.BOT);
        }

        public void StartButtonAvailable(bool isAvailable)
        {
            buttonStartGame.interactable = isAvailable;
            if (isAvailable)
                ShowHideNotAvailable(false);
        }

        public void StartSoloGame()
        {
            Destroy(GameObject.Find("Player"));
            Destroy(GameObject.Find("Stage"));
            if (UserInfoManager.Instance != null)
                Destroy(UserInfoManager.Instance.gameObject);
            if (LobbySettingManager.Instance != null)
                Destroy(LobbySettingManager.Instance.gameObject);
            DataManager.Instance.CurrentGameData = gameData;
            SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            StartCoroutine(SceneLoadManager.Instance.LoadGameScene(gameData));
        }

        public void BackToMainLobby()
        {
            if (UserInfoManager.Instance != null)
                Destroy(UserInfoManager.Instance.gameObject);
            if (LobbySettingManager.Instance != null)
                Destroy(LobbySettingManager.Instance.gameObject);
            SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
        }
    }
}
