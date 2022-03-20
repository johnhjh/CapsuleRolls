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

    private readonly string gameModeArcadeText = "아케이드 모드";
    private readonly string gameModeStageText = "스테이지 모드";
    private readonly string gameModePracticeText = "연습 모드";
    private readonly string gameModeBotText = "AI봇 대전 모드";

    private readonly string gameModeArcadeDetailText = "최고 스코어에 도전하자!";
    private readonly string gameModeStageDetailText = "스테이지 클리어에 도전하자!";
    private readonly string gameModePracticeDetailText = "자유롭게 연습해서 실력상승!";
    private readonly string gameModeBotDetailText = "AI봇과 대전하며 실력상승!";

    private Text gameModeText;
    private Text gameModeDetailText;
    private Text gameKindDetailText;
    private Image gameKindDetailImage;
    private Text gameScoreDetailText;

    private GameObject gameScoreUI;
    private GameObject gameKindUI;
    private GameObject gameStageUI;
    private GameObject gameStageSelectUI;
    private GameObject gameBotDifficultyUI;

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
        // Stage
        gameStageUI = GameObject.Find("GameStageUI");
        gameStageSelectUI = GameObject.Find("GameStageSelectUI");
        // Bot
        gameBotDifficultyUI = GameObject.Find("GameBotDifficultyUI");

        MainMenuCtrl arcadeButtonCtrl = GameObject.Find("Button_Arcade").GetComponent<MainMenuCtrl>();
        arcadeButtonCtrl.IsSelected = true;
        arcadeButtonCtrl.finalAlpha = 1f;
        arcadeButtonCtrl.finalFontSize = 105f;
        SelectGameMode(GameMode.ARCADE);
    }

    public void SelectGameMode(GameMode mode)
    {
        gameData.Mode = mode;
        switch (mode)
        {
            case GameMode.ARCADE:
                gameModeText.text = gameModeArcadeText;
                gameModeDetailText.text = gameModeArcadeDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(true);
                gameStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(false);
                break;
            case GameMode.STAGE:
                gameModeText.text = gameModeStageText;
                gameModeDetailText.text = gameModeStageDetailText;
                gameKindUI.SetActive(false);
                gameScoreUI.SetActive(false);
                gameStageUI.SetActive(true);
                gameStageSelectUI.SetActive(true);
                gameBotDifficultyUI.SetActive(false);
                break;
            case GameMode.PRACTICE:
                gameModeText.text = gameModePracticeText;
                gameModeDetailText.text = gameModePracticeDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(false);
                gameStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(false);
                break;
            case GameMode.BOT:
                gameModeText.text = gameModeBotText;
                gameModeDetailText.text = gameModeBotDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(false);
                gameStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(true);
                break;
            default:
                gameModeText.text = gameModeArcadeText;
                gameModeDetailText.text = gameModeArcadeDetailText;
                gameKindUI.SetActive(true);
                gameScoreUI.SetActive(true);
                gameStageUI.SetActive(false);
                gameStageSelectUI.SetActive(false);
                gameBotDifficultyUI.SetActive(false);
                break;
        }

    }

    public void OnClickButtonArcade()
    {
        SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
        SelectGameMode(GameMode.ARCADE);
    }

    public void OnClickButtonStage()
    {
        SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
        SelectGameMode(GameMode.STAGE);
    }

    public void OnClickButtonPractice()
    {
        SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
        SelectGameMode(GameMode.PRACTICE);
    }

    public void OnClickButtonBot()
    {
        SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
        SelectGameMode(GameMode.BOT);
    }

    public void SelectGameKind(GameKind kind)
    {
        gameData.Kind = kind;
        switch (kind)
        {
            case GameKind.ROLL_THE_BALL:

                break;
            case GameKind.THROWING:

                break;
            case GameKind.KILLINGROBOT:

                break;
            default:

                break;
        }
    }

    public void StartGame()
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
