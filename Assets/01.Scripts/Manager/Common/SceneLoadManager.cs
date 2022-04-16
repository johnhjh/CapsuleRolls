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
        CREDIT,
        IN_GAME,
    }

    public enum GameSceneType
    {
        COMMON_UI = 0,
        LEVEL,
        MODE_UI,
        KIND_UI,
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
        private readonly string CREDIT_SCENE_NAME = "CreditScene";

        // Game Common Scenes
        private readonly string COMMON_UI_MULTI = "CommonUI_Multi";
        private readonly string COMMON_UI_SOLO = "CommonUI_Solo";

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

        private void Awake()
        {
            if (sceneLoadMgr == null)
            {
                sceneLoadMgr = this;
                InitSceneLoadManager();
                DontDestroyOnLoad(this.gameObject);
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
                { LobbySceneType.SHOPPING, new SceneData(SHOPPING_SCENE_NAME, LoadSceneMode.Additive) },
                { LobbySceneType.CREDIT, new SceneData(CREDIT_SCENE_NAME, LoadSceneMode.Additive) }
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
                case LobbySceneType.CREDIT:
                    return CREDIT_SCENE_NAME;
                default:
                    return MAIN_LOBBY_SCENE_NAME;
            }
        }

        public IEnumerator LoadLobbyScene(LobbySceneType sceneType, bool isAuto)
        {
            ResetFields();
            if (isAuto)
                yield return StartCoroutine(FadeInLoading());
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
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneData.sceneName));
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
            string gameLevelSceneName = "Level_";
            switch (data.Mode)
            {
                case GameMode.ARCADE:
                    gameLevelSceneName += "Arcade_";
                    gameLevelSceneName += GetGameKindString(data.Kind);
                    break;
                case GameMode.STAGE:
                    gameLevelSceneName += "Stage" + ((int)data.Stage).ToString();
                    break;
                case GameMode.PRACTICE:
                    gameLevelSceneName += "Practice";
                    break;
                case GameMode.BOT:
                case GameMode.MULTI:
                case GameMode.RANK:
                case GameMode.CUSTOM:
                    gameLevelSceneName += "Multi_";
                    gameLevelSceneName += GetGameKindString(data.Kind);
                    break;
            }
            return gameLevelSceneName;
        }

        private string GetGameModeString(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.STAGE:
                    return "Stage";
                case GameMode.ARCADE:
                    return "Arcade";
                case GameMode.PRACTICE:
                    return "Practice";
                case GameMode.BOT:
                    return "Bot";
            }
            return "Stage";
        }

        private string GetGameKindString(GameKind kind)
        {
            switch (kind)
            {
                case GameKind.GOAL_IN:
                    return "GoalIn";
                case GameKind.BATTLE_ROYAL:
                    return "BattleRoyal";
                case GameKind.GOLDEN_BALL:
                    return "GoldenBall";
                case GameKind.NEXT_TARGET:
                    return "NextTarget";
                default:
                    return "GoalIn";
            }
        }

        private string GetGameSceneUIName(GameMode mode)
        {
            return "ModeUI_" + GetGameModeString(mode);
        }

        private string GetGameSceneUIName(GameKind kind)
        {
            return "KindUI_" + GetGameKindString(kind);
        }

        private string GetGameSceneLogicName(GameMode mode)
        {
            string gameLogicSceneName = "Logic_";
            switch (mode)
            {
                case GameMode.ARCADE:
                case GameMode.STAGE:
                case GameMode.PRACTICE:
                case GameMode.BOT:
                    gameLogicSceneName += "Solo";
                    break;
                case GameMode.MULTI:
                case GameMode.RANK:
                case GameMode.CUSTOM:
                    gameLogicSceneName += "Multi";
                    break;
                default:
                    gameLogicSceneName += "Solo";
                    break;
            }
            return gameLogicSceneName;
        }

        private Dictionary<GameSceneType, SceneData> MakeGameSceneDictionary(GameData data)
        {
            Dictionary<GameSceneType, SceneData> gameSceneDictionary = new Dictionary<GameSceneType, SceneData>()
            {
                { GameSceneType.LEVEL, new SceneData(GetGameSceneLevelName(data), LoadSceneMode.Additive) },
                { GameSceneType.COMMON_UI, new SceneData(GetCommonUIName(data.Mode), LoadSceneMode.Additive) },
                { GameSceneType.MODE_UI, new SceneData(GetGameSceneUIName(data.Mode), LoadSceneMode.Additive) },
                { GameSceneType.KIND_UI, new SceneData(GetGameSceneUIName(data.Kind), LoadSceneMode.Additive) },
                { GameSceneType.LOGIC, new SceneData(GetGameSceneLogicName(data.Mode), LoadSceneMode.Additive) },
            };

            Debug.Log(gameSceneDictionary[GameSceneType.COMMON_UI].sceneName);
            Debug.Log(gameSceneDictionary[GameSceneType.LEVEL].sceneName);
            Debug.Log(gameSceneDictionary[GameSceneType.MODE_UI].sceneName);
            Debug.Log(gameSceneDictionary[GameSceneType.KIND_UI].sceneName);
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
                case GameSceneType.MODE_UI:
                case GameSceneType.KIND_UI:
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

            yield return StartCoroutine(FadeOutLoading());
        }

        public IEnumerator ReLoadStageScene(GameData data)
        {
            ResetFields();
            yield return StartCoroutine(FadeInLoading());
            if (data.Stage != GameStage.TUTORIAL_1 && data.Stage != GameStage.STAGE_1)
                yield return SceneManager.UnloadSceneAsync(GetGameSceneLevelName(data));
            yield return SceneManager.UnloadSceneAsync(GetGameSceneLogicName(data.Mode));
            if (data.Stage != GameStage.TUTORIAL_1 && data.Stage != GameStage.STAGE_1)
                yield return SceneManager.LoadSceneAsync(GetGameSceneLevelName(data), LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(GetGameSceneLogicName(data.Mode), LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GetGameSceneLevelName(data)));
            yield return StartCoroutine(FadeOutLoading());
        }

        public IEnumerator LoadNextStageScene(GameData data)
        {
            ResetFields();
            yield return StartCoroutine(FadeInLoading(data.Kind));
            Text loadingInfoText = GameObject.Find(LOADING_INFO_TEXT).GetComponent<Text>();
            Image loadingProgressBar = GameObject.Find(LOADING_PROGRESS_BAR).GetComponent<Image>();
            Text loadingProgressText = GameObject.Find(LOADING_PROGRESS_TEXT).GetComponent<Text>();
            yield return SceneManager.UnloadSceneAsync(GetGameSceneLevelName(data));
            yield return SceneManager.UnloadSceneAsync(GetGameSceneLogicName(data.Mode));
            data.Stage = (GameStage)((int)data.Stage + 1);
            DataManager.Instance.CurrentGameData.Stage = data.Stage;
            Dictionary<GameSceneType, SceneData> gameSceneDictionary = MakeGameSceneDictionary(data);
            float totalProgress = 0f;
            foreach (var loadScene in gameSceneDictionary)
            {
                if (loadScene.Key == GameSceneType.COMMON_UI ||
                    loadScene.Key == GameSceneType.MODE_UI ||
                    loadScene.Key == GameSceneType.KIND_UI)
                {
                    totalProgress += 1f / gameSceneDictionary.Count;
                    continue;
                }
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
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameSceneDictionary[GameSceneType.LEVEL].sceneName));
            yield return StartCoroutine(FadeOutLoading());
        }

        public IEnumerator ReLoadGameScene(GameData data)
        {
            ResetFields();
            yield return StartCoroutine(FadeInLoading());
            yield return SceneManager.UnloadSceneAsync(GetGameSceneLevelName(data));
            yield return SceneManager.UnloadSceneAsync(GetGameSceneLogicName(data.Mode));
            yield return SceneManager.LoadSceneAsync(GetGameSceneLevelName(data), LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(GetGameSceneLogicName(data.Mode), LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GetGameSceneLevelName(data)));
            yield return StartCoroutine(FadeOutLoading());
        }

        public IEnumerator LoadLobbySceneFromGame(LobbySceneType sceneType)
        {
            ResetFields();
            yield return StartCoroutine(FadeInLoading());

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(
                SceneTypeToString(sceneType), LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = true;
            while (!asyncOperation.isDone)
            {
                yield return null;
                progressAmount = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)
                    isLoadingDone = true;
                InfiniteLoopDetector.Run();
            }

            Dictionary<GameSceneType, SceneData> gameSceneDictionary =
                MakeGameSceneDictionary(DataManager.Instance.CurrentGameData);
            foreach (var data in gameSceneDictionary)
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(data.Value.sceneName));

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneTypeToString(sceneType)));

            yield return StartCoroutine(FadeOutLoading());
        }

        private IEnumerator FadeLoading(float finalAlpha)
        {
            float fadeSpeed = Mathf.Abs(fadeLoadingCG.alpha - finalAlpha) / fadeDuration;
            while (!Mathf.Approximately(fadeLoadingCG.alpha, finalAlpha))
            {
                if (Time.timeScale == 0f) Time.timeScale = 1f;
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
                DataManager.Instance.GameKindDatas != null &&
                DataManager.Instance.GameKindDatas.Count >= (int)kind + 1)
            {
                GameObject.Find(LOADING_GAME_PREVIEW_IMAGE).GetComponent<Image>().sprite = DataManager.Instance.GameKindDatas[(int)kind].preview;
                GameObject.Find(LOADING_GAME_DETAIL_NAME).GetComponent<Text>().text = DataManager.Instance.GameKindDatas[(int)kind].name;
                GameObject.Find(LOADING_GAME_DETAIL_DESC).GetComponent<Text>().text = DataManager.Instance.GameKindDatas[(int)kind].desc;
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
