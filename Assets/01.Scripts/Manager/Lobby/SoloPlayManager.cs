using Capsule.Audio;
using Capsule.Entity;
using Capsule.Lobby;
using Capsule.Player.Lobby;
using Capsule.SceneLoad;
using UnityEngine;
using UnityEngine.UI;

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
    /*
    private readonly string gameModeArcadeText = "아케이드 모드";
    private readonly string gameModeStageText = "스테이지 모드";
    private readonly string gameModePracticeText = "연습 모드";
    private readonly string gameModeBotText = "AI봇 대전 모드";

    private readonly string gameModeArcadeDetailText = "최고 스코어에 도전하자!";
    private readonly string gameModeStageDetailText = "스테이지 클리어에 도전하자!";
    private readonly string gameModePracticeDetailText = "자유롭게 연습해서 실력을 키우자!";
    private readonly string gameModeBotDetailText = "AI봇과 대전하며 실력을 키우자!";

    private readonly string gameKindRollTheBallDetailText = "공 굴려서 골인~!";
    private readonly string gameKindThrowingFeederDetailText = "먹이를 던져주자~!";
    private readonly string gameKindAttackInvaderDetailText = "침략자를 막자~!";
    */
    private Text gameModeText;
    private Text gameModeDetailText;
    private Text gameKindDetailText;
    private Image gameKindDetailImage;
    private Text gameScoreDetailText;
    private Text gameStageDetailText;
    private Image gameStageDetailImage;
    private Text gameHighestStageDetailText;

    private GameObject gameScoreUI;
    private GameObject gameKindUI;
    private GameObject gameHighestStageUI;
    private GameObject gameStageSelectUI;
    private GameObject gameBotDifficultyUI;

    public Sprite gameKindRollTheBallDetailImage;
    public Sprite gameKindThrowingFeederDetailImage;
    public Sprite gameKindAttackInvaderDetailImage;

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
        gameStageDetailText.text = DataManager.Instance.GetHighestStageString();
        gameStageDetailImage = GameObject.Find("GameStageDetailImage").GetComponent<Image>();
        gameHighestStageUI = GameObject.Find("GameHighestStageUI");
        gameHighestStageDetailText = GameObject.Find("GameHighestStageDetailText").GetComponent<Text>();
        gameHighestStageDetailText.text = DataManager.Instance.GetHighestStageString();
        // Bot
        gameBotDifficultyUI = GameObject.Find("GameBotDifficultyUI");

        MainMenuCtrl arcadeButtonCtrl = GameObject.Find("Button_Arcade").GetComponent<MainMenuCtrl>();
        arcadeButtonCtrl.IsSelected = true;
        arcadeButtonCtrl.finalAlpha = 1f;
        arcadeButtonCtrl.finalFontSize = 105f;
        SelectGameMode(GameMode.ARCADE);
        SelectGameKind(GameKind.ROLL_THE_BALL);
    }

    public void SelectGameMode(GameMode mode)
    {
        gameData.Mode = mode;
        gameModeText.text = DataManager.Instance.gameModeDatas[(int)mode].name;
        gameModeDetailText.text = DataManager.Instance.gameModeDatas[(int)mode].desc;
        switch (mode)
        {
            case GameMode.ARCADE:
                //gameModeText.text = gameModeArcadeText;
                //gameModeDetailText.text = gameModeArcadeDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(true);
                gameHighestStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(false);
                break;
            case GameMode.STAGE:
                //gameModeText.text = gameModeStageText;
                //gameModeDetailText.text = gameModeStageDetailText;
                gameKindUI.SetActive(false);
                gameScoreUI.SetActive(false);
                gameHighestStageUI.SetActive(true);
                gameStageSelectUI.SetActive(true);
                gameBotDifficultyUI.SetActive(false);
                break;
            case GameMode.PRACTICE:
                //gameModeText.text = gameModePracticeText;
                //gameModeDetailText.text = gameModePracticeDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(false);
                gameHighestStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(false);
                break;
            case GameMode.BOT:
                //gameModeText.text = gameModeBotText;
                //gameModeDetailText.text = gameModeBotDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(false);
                gameHighestStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(true);
                break;
            default:
                //gameModeText.text = gameModeArcadeText;
                //gameModeDetailText.text = gameModeArcadeDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(true);
                gameHighestStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(false);
                break;
        }

    }

    public void SelectGameKind(GameKind kind)
    {
        gameData.Kind = kind;
        gameKindDetailText.text = DataManager.Instance.gameKindDatas[(int)kind].desc;
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

    public void SelectGameStage(GameStage stage)
    {

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
        Destroy(UserInfoManager.Instance.gameObject);
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
