using Capsule.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Capsule.SceneLoad
{
    public enum LobbySceneType
    {
        TITLE = 0,
        MAIN_LOBBY,
        CUSTOMIZE,
        SOLO,
        MULTI,
        SHOPPING,
    }

    public enum GameSceneType
    {
        COMMON_UI = 0,
        LEVEL,
        UI,
        LOGIC,
    }

    public struct SceneData
    {
        public string sceneName;
        public LoadSceneMode sceneMode;
        public SceneData(string name, LoadSceneMode mode)
        {
            sceneName = name;
            sceneMode = mode;
        }
    }

    public class SceneLoadManager : MonoBehaviour
    {
        private static SceneLoadManager sceneLoadMgr;
        public static SceneLoadManager Instance
        {
            get
            {
                if (sceneLoadMgr == null)
                    sceneLoadMgr = FindObjectOfType<SceneLoadManager>();
                return sceneLoadMgr;
            }
        }

        [SerializeField]
        private LobbySceneType currentScene;
        public LobbySceneType CurrentScene
        {
            get { return currentScene; }
            set { currentScene = value; }
        }

        // Lobby Scenes
        private readonly string TITLE_SCENE_NAME = "TitleScene";
        private readonly string MAIN_LOBBY_SCENE_NAME = "MainLobbyScene";
        private readonly string CUSTOMIZE_SCENE_NAME = "CustomizeScene";
        private readonly string SOLO_PLAY_SCENE_NAME = "SoloPlayScene";
        private readonly string MULTI_PLAY_SCENE_NAME = "MultiPlayScene";
        private readonly string LOADING_SCENE_NAME = "LoadingScene";
        private readonly string SHOPPING_SCENE_NAME = "ShoppingScene";

        // Game Scenes
        private readonly string COMMON_UI_MULTI = "CommonUI_Multi";
        private readonly string COMMON_UI_SOLO = "CommonUI_Solo";
        private readonly string STRING_ROLL_THE_BALL = "RollTheBall";
        private readonly string STRING_ATTACK_INVADER = "AttackInvader";
        private readonly string STRING_THROWING_FEEDER = "ThrowingFeeder";

        // Lobby Loading
        private readonly string LOADING_LOBBY_SCENE_PANEL = "LoadingLobbyScenePanel";
        // Game Loading
        private readonly string LOADING_GAME_SCENE_PANEL = "LoadingGameScenePanel";
        private readonly string LOADING_GAME_PREVIEW_IMAGE = "LoadingGamePreviewImage";
        private readonly string LOADING_GAME_DETAIL_NAME = "LoadingGameDetailName";
        private readonly string LOADING_GAME_DETAIL_DESC = "LoadingGameDetailDesc";
        private readonly string LOADING_INFO_TEXT = "Loading_Info_Text";
        private readonly string LOADING_PROGRESS_BAR = "Loading_Progress_Bar";
        private readonly string LOADING_PROGRESS_TEXT = "Loading_Progress_Text";

        private Dictionary<LobbySceneType, SceneData> lobbySceneDictionary;

        private CanvasGroup fadeLoadingCG;
        [Range(0.5f, 2.0f)]
        public float fadeDuration = 1.0f;

        private bool isLoadingDone = false;
        public bool IsLoadingDone { get { return isLoadingDone; } }
        private float progressAmount = 0f;
        public float Progress { get { return progressAmount; } }
        private bool allowNextScene = false;
        public bool AllowNextScene { set { allowNextScene = value; } }

        //private WaitForSeconds ws10 = new WaitForSeconds(1.0f);

        private void Awake()
        {
            if (sceneLoadMgr == null)
            {
                sceneLoadMgr = this;
                InitSceneLoadManager();
                DontDestroyOnLoad(this);
            }
            else if (sceneLoadMgr != this)
                Destroy(this.gameObject);
        }

        private void InitSceneLoadManager()
        {
            lobbySceneDictionary = new Dictionary<LobbySceneType, SceneData>
            {
                { LobbySceneType.TITLE, new SceneData(TITLE_SCENE_NAME, LoadSceneMode.Single) },
                { LobbySceneType.MAIN_LOBBY, new SceneData(MAIN_LOBBY_SCENE_NAME, LoadSceneMode.Additive) },
                { LobbySceneType.CUSTOMIZE, new SceneData(CUSTOMIZE_SCENE_NAME, LoadSceneMode.Additive) },
                { LobbySceneType.SOLO, new SceneData(SOLO_PLAY_SCENE_NAME, LoadSceneMode.Additive) },
                { LobbySceneType.MULTI, new SceneData(MULTI_PLAY_SCENE_NAME, LoadSceneMode.Additive) },
                { LobbySceneType.SHOPPING, new SceneData(SHOPPING_SCENE_NAME, LoadSceneMode.Additive) }
            };
        }

        private string SceneTypeToString(LobbySceneType sceneType)
        {
            switch (sceneType)
            {
                case LobbySceneType.TITLE:
                    return TITLE_SCENE_NAME;
                case LobbySceneType.MAIN_LOBBY:
                    return MAIN_LOBBY_SCENE_NAME;
                case LobbySceneType.CUSTOMIZE:
                    return CUSTOMIZE_SCENE_NAME;
                case LobbySceneType.SOLO:
                    return SOLO_PLAY_SCENE_NAME;
                default:
                    return MAIN_LOBBY_SCENE_NAME;
            }
        }

        public IEnumerator LoadLobbyScene(LobbySceneType sceneType, bool isAuto)
        {
            ResetFields();
            if (isAuto)
                yield return StartCoroutine(FadeInLoading());
            //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneTypeToString(sceneType), 
            //isAuto ? LoadSceneMode.Additive : LoadSceneMode.Single);
            if (!lobbySceneDictionary.TryGetValue(sceneType, out SceneData sceneData))
                yield break;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneData.sceneName,
                isAuto ? sceneData.sceneMode : LoadSceneMode.Single);
            asyncOperation.allowSceneActivation = isAuto;
            while (!asyncOperation.isDone)
            {
                yield return null;
                progressAmount = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)
                {
                    isLoadingDone = true;
                    if (!isAuto && allowNextScene)
                        asyncOperation.allowSceneActivation = true;
                }
                InfiniteLoopDetector.Run();
            }
            if (isAuto && sceneData.sceneMode == LoadSceneMode.Additive)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
                yield return StartCoroutine(FadeOutLoading());
            }
        }

        public void ReLoadScene(LobbySceneType sceneType)
        {
            SceneManager.LoadScene(SceneTypeToString(sceneType), LoadSceneMode.Single);
        }

        private string GetCommonUIName(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.ARCADE:
                case GameMode.STAGE:
                case GameMode.PRACTICE:
                case GameMode.BOT:
                    return COMMON_UI_SOLO;
                case GameMode.MULTI:
                case GameMode.RANK:
                case GameMode.CUSTOM:
                    return COMMON_UI_MULTI;
            }
            return COMMON_UI_SOLO;
        }

        private string GetGameSceneLevelName(GameData data)
        {
            string gameLevelSceneName = "";
            switch (data.Kind)
            {
                case GameKind.ROLL_THE_BALL:
                    gameLevelSceneName += STRING_ROLL_THE_BALL;
                    break;
                case GameKind.THROWING_FEEDER:
                    gameLevelSceneName += STRING_THROWING_FEEDER;
                    break;
                case GameKind.ATTACK_INVADER:
                    gameLevelSceneName += STRING_ATTACK_INVADER;
                    break;
                default:
                    gameLevelSceneName += STRING_ROLL_THE_BALL;
                    break;
            }
            switch (data.Mode)
            {
                case GameMode.ARCADE:
                    gameLevelSceneName += "Level_Arcade";
                    break;
                case GameMode.STAGE:
                    gameLevelSceneName += "Level_Stage" + ((int)data.Stage).ToString();
                    break;
                case GameMode.PRACTICE:
                    gameLevelSceneName += "Level_Practice";
                    break;
                case GameMode.BOT:
                case GameMode.MULTI:
                case GameMode.RANK:
                case GameMode.CUSTOM:
                    gameLevelSceneName += "Level_Multi";
                    switch (data.Map)
                    {
                        case GameMap.CUSHION:
                            gameLevelSceneName += "_Cushion";
                            break;
                        case GameMap.BEACH:
                            gameLevelSceneName += "_Beach";
                            break;
                        case GameMap.LARVA:
                            gameLevelSceneName += "_Larva";
                            break;
                    }
                    break;
            }
            return gameLevelSceneName;
        }

        private string GetGameSceneUIName(GameKind kind)
        {
            switch (kind)
            {
                case GameKind.ROLL_THE_BALL:
                    return STRING_ROLL_THE_BALL + "UI";
                case GameKind.THROWING_FEEDER:
                    return STRING_THROWING_FEEDER + "UI";
                case GameKind.ATTACK_INVADER:
                    return STRING_ATTACK_INVADER + "UI";
                default:
                    return STRING_ROLL_THE_BALL + "UI";
            }
        }

        private string GetGameSceneLogicName(GameMode mode, GameKind kind)
        {
            string gameLogicSceneName = "";
            switch (kind)
            {
                case GameKind.ROLL_THE_BALL:
                    gameLogicSceneName += STRING_ROLL_THE_BALL;
                    break;
                case GameKind.THROWING_FEEDER:
                    gameLogicSceneName += STRING_THROWING_FEEDER;
                    break;
                case GameKind.ATTACK_INVADER:
                    gameLogicSceneName += STRING_ATTACK_INVADER;
                    break;
                default:
                    gameLogicSceneName += STRING_ROLL_THE_BALL;
                    break;
            }
            switch (mode)
            {
                case GameMode.ARCADE:
                case GameMode.STAGE:
                case GameMode.PRACTICE:
                case GameMode.BOT:
                    gameLogicSceneName += "Logic_Solo";
                    break;
                case GameMode.MULTI:
                case GameMode.RANK:
                case GameMode.CUSTOM:
                    gameLogicSceneName += "Logic_Multi";
                    break;
                default:
                    gameLogicSceneName += "Logic_Solo";
                    break;
            }
            return gameLogicSceneName;
        }

        private Dictionary<GameSceneType, SceneData> MakeGameSceneDictionary(GameData data)
        {
            Dictionary<GameSceneType, SceneData> gameSceneDictionary = new Dictionary<GameSceneType, SceneData>()
            {
                { GameSceneType.COMMON_UI, new SceneData(GetCommonUIName(data.Mode), LoadSceneMode.Additive) },
                { GameSceneType.LEVEL, new SceneData(GetGameSceneLevelName(data), LoadSceneMode.Additive) },
                { GameSceneType.UI, new SceneData(GetGameSceneUIName(data.Kind), LoadSceneMode.Additive) },
                { GameSceneType.LOGIC, new SceneData(GetGameSceneLogicName(data.Mode, data.Kind), LoadSceneMode.Additive) },
            };

            Debug.Log(gameSceneDictionary[GameSceneType.COMMON_UI].sceneName);
            Debug.Log(gameSceneDictionary[GameSceneType.LEVEL].sceneName);
            Debug.Log(gameSceneDictionary[GameSceneType.UI].sceneName);
            Debug.Log(gameSceneDictionary[GameSceneType.LOGIC].sceneName);

            return gameSceneDictionary;
        }

        private string GetLoadingInfoText(GameSceneType type)
        {
            switch (type)
            {
                case GameSceneType.COMMON_UI:
                    return "공통 UI 불러오는 중..";
                case GameSceneType.LEVEL:
                    return "게임 맵 불러오는 중..";
                case GameSceneType.UI:
                    return "게임 UI 불러오는 중 ..";
                case GameSceneType.LOGIC:
                    return "게임 로직 불러오는 중..";
                default:
                    return "";
            }
        }

        public IEnumerator LoadGameScene(GameData data)
        {
            ResetFields();
            yield return StartCoroutine(FadeInLoading(data.Kind));
            Text loadingInfoText = GameObject.Find(LOADING_INFO_TEXT).GetComponent<Text>();
            Image loadingProgressBar = GameObject.Find(LOADING_PROGRESS_BAR).GetComponent<Image>();
            Text loadingProgressText = GameObject.Find(LOADING_PROGRESS_TEXT).GetComponent<Text>();

            // GameScene Load Logic 작성할 자리
            Dictionary<GameSceneType, SceneData> gameSceneDictionary = MakeGameSceneDictionary(data);
            float totalProgress = 0f;
            foreach (var loadScene in gameSceneDictionary)
            {
                loadingInfoText.text = GetLoadingInfoText(loadScene.Key);
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(loadScene.Value.sceneName, loadScene.Value.sceneMode);
                asyncOperation.allowSceneActivation = true;
                while (!asyncOperation.isDone)
                {
                    yield return null;
                    progressAmount = asyncOperation.progress;
                    loadingProgressBar.fillAmount = totalProgress + progressAmount / gameSceneDictionary.Count;
                    loadingProgressText.text = "LOADING .. " + (loadingProgressBar.fillAmount * 100).ToString("000") + ("%");
                    InfiniteLoopDetector.Run();
                }
                totalProgress += 1f / gameSceneDictionary.Count;
            }
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameSceneDictionary[GameSceneType.LEVEL].sceneName));
            // GameScene Load Logic 끝

            yield return StartCoroutine(FadeOutLoading());
        }

        private IEnumerator FadeLoading(float finalAlpha)
        {
            float fadeSpeed = Mathf.Abs(fadeLoadingCG.alpha - finalAlpha) / fadeDuration;
            while (!Mathf.Approximately(fadeLoadingCG.alpha, finalAlpha))
            {
                fadeLoadingCG.alpha = Mathf.MoveTowards(fadeLoadingCG.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator FadeInLoading()
        {
            yield return SceneManager.LoadSceneAsync(LOADING_SCENE_NAME, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(LOADING_SCENE_NAME));
            fadeLoadingCG = GameObject.Find(LOADING_LOBBY_SCENE_PANEL).GetComponent<CanvasGroup>();
            if (fadeLoadingCG != null)
            {
                fadeLoadingCG.blocksRaycasts = true;
                fadeLoadingCG.interactable = true;
                yield return StartCoroutine(FadeLoading(1f));
            }
        }

        private IEnumerator FadeInLoading(GameKind kind)
        {
            yield return SceneManager.LoadSceneAsync(LOADING_SCENE_NAME, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(LOADING_SCENE_NAME));
            fadeLoadingCG = GameObject.Find(LOADING_GAME_SCENE_PANEL).GetComponent<CanvasGroup>();
            if (DataManager.Instance != null &&
                DataManager.Instance.gameKindDatas != null &&
                DataManager.Instance.gameKindDatas.Count >= (int)kind + 1)
            {
                GameObject.Find(LOADING_GAME_PREVIEW_IMAGE).GetComponent<Image>().sprite = DataManager.Instance.gameKindDatas[(int)kind].preview;
                GameObject.Find(LOADING_GAME_DETAIL_NAME).GetComponent<Text>().text = DataManager.Instance.gameKindDatas[(int)kind].name;
                GameObject.Find(LOADING_GAME_DETAIL_DESC).GetComponent<Text>().text = DataManager.Instance.gameKindDatas[(int)kind].desc;
            }
            if (fadeLoadingCG != null)
            {
                fadeLoadingCG.blocksRaycasts = true;
                fadeLoadingCG.interactable = true;
                yield return StartCoroutine(FadeLoading(1f));
            }
        }

        private IEnumerator FadeOutLoading()
        {
            if (fadeLoadingCG == null)
                yield return null;
            else
            {
                yield return StartCoroutine(FadeLoading(0f));
                fadeLoadingCG.blocksRaycasts = false;
                fadeLoadingCG.interactable = false;
                fadeLoadingCG = null;
            }
            SceneManager.UnloadSceneAsync(LOADING_SCENE_NAME);
        }

        private void ResetFields()
        {
            isLoadingDone = false;
            allowNextScene = false;
            progressAmount = 0f;
        }
    }
}
