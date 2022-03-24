using Capsule.Audio;
using Capsule.Entity;
using Capsule.Lobby.Player;
using Capsule.SceneLoad;
using System.Collections;
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
        //private RewardCtrl gameStageSelectPopupDetailReward;

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
                gameStageSelectPopupDetailKindPreview.sprite = DataManager.Instance.gameKindDatas[(int)CurrentStageSlot.data.kind].preview;
                gameStageSelectPopupDetailKindDesc.text = DataManager.Instance.gameKindDatas[(int)CurrentStageSlot.data.kind].name;
                gameStageSelectPopupDetailDesc.text = CurrentStageSlot.data.desc;
            }
            else
            {
                gameStageSelectPopupDetailName.text = stageSlot.data.name;
                gameStageSelectPopupDetailKindPreview.sprite = DataManager.Instance.gameKindDatas[(int)stageSlot.data.kind].preview;
                gameStageSelectPopupDetailKindDesc.text = DataManager.Instance.gameKindDatas[(int)stageSlot.data.kind].name;
                gameStageSelectPopupDetailDesc.text = stageSlot.data.desc;
            }
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
            PlayerTransform.Instance.SetPosition(new Vector3(0.07f, -0.4f, -4.34f));
            PlayerTransform.Instance.SetRotation(Quaternion.Euler(8.2f, 177.6f, 0f));
            PlayerTransform.Instance.SetScale(1.18f);

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

            MainMenuCtrl stageButtonCtrl = GameObject.Find("Button_Stage").GetComponent<MainMenuCtrl>();
            stageButtonCtrl.IsSelected = true;
            stageButtonCtrl.finalAlpha = 1f;
            stageButtonCtrl.finalFontSize = 105f;

            GameKindSlot kindSlot = GameObject.Find("Game_Kind_List").transform.GetChild(0).GetComponent<GameKindSlot>();
            kindSlot.SelectSlot();

            gameData.Difficulty = AIDifficulty.EASY;

            SelectGameMode(GameMode.STAGE);
            SelectGameKind(GameKind.ROLL_THE_BALL);

            StartCoroutine(InitStageSlots());
        }

        private IEnumerator InitStageSlots()
        {
            yield return new WaitForSeconds(1.0f);
            int counter = 0;
            Transform stageSlotContents = GameObject.Find("StageSlotContents").transform;
            int nextStage = DataManager.Instance.CurrentPlayerGameData.HighestStage + 1;
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
            SFXManager.Instance.PlayOneShot(isOpen ? MenuSFX.OK : MenuSFX.BACK);
            gameKindSelectPopupCG.alpha = isOpen ? 1.0f : 0f;
            gameKindSelectPopupCG.blocksRaycasts = isOpen;
            gameKindSelectPopupCG.interactable = isOpen;
        }

        public void PopupGameStageSelect(bool isOpen)
        {
            SFXManager.Instance.PlayOneShot(isOpen ? MenuSFX.OK : MenuSFX.BACK);
            gameStageSelectPopupCG.alpha = isOpen ? 1.0f : 0f;
            gameStageSelectPopupCG.blocksRaycasts = isOpen;
            gameStageSelectPopupCG.interactable = isOpen;
        }

        public void SelectGameMode(GameMode mode)
        {
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
                    gameKindUI.SetActive(true);
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
            gameKindDetailText.text = DataManager.Instance.gameKindDatas[(int)kind].name;
            gameKindDetailImage.sprite = DataManager.Instance.gameKindDatas[(int)kind].preview;
            /*
            switch (kind)
            {
                case GameKind.ROLL_THE_BALL:
                    gameKindDetailText.text = gameKindRollTheBallDetailText;
                    gameKindDetailImage.sprite = gameKindRollTheBallDetailImage;
                    break;
                case GameKind.THROWING_FEEDER:
                    gameKindDetailText.text = gameKindThrowingFeederDetailText;
                    gameKindDetailImage.sprite = gameKindThrowingFeederDetailImage;
                    break;
                case GameKind.ATTACK_INVADER:
                    gameKindDetailText.text = gameKindAttackInvaderDetailText;
                    gameKindDetailImage.sprite = gameKindAttackInvaderDetailImage;
                    break;
                default:
                    gameKindDetailText.text = gameKindRollTheBallDetailText;
                    gameKindDetailImage.sprite = gameKindRollTheBallDetailImage;
                    break;
            }
            */
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
            SelectGameMode(GameMode.ARCADE);
        }

        public void OnClickButtonStage()
        {
            OnClickAnyButton();
            SelectGameMode(GameMode.STAGE);
        }

        public void OnClickButtonPractice()
        {
            OnClickAnyButton();
            SelectGameMode(GameMode.PRACTICE);
        }

        public void OnClickButtonBot()
        {
            OnClickAnyButton();
            SelectGameMode(GameMode.BOT);
        }

        public void StartSoloGame()
        {
            Destroy(GameObject.Find("Player"));
            Destroy(GameObject.Find("Stage"));
            Destroy(UserInfoManager.Instance.gameObject);
            DataManager.Instance.CurrentGameData = gameData;
            SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            StartCoroutine(SceneLoadManager.Instance.LoadGameScene(gameData));
        }

        public void BackToMainLobby()
        {
            Destroy(UserInfoManager.Instance.gameObject);
            SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
        }
    }
}
