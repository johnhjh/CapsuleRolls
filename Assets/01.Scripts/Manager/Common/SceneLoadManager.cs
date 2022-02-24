using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Capsule.SceneLoad
{
    public enum LobbySceneType
    {
        TITLE = 0,
        MAIN_LOBBY,
        CUSTOMIZE,
        SOLO,
    }

    public enum GameSceneType
    {

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
        public LobbySceneType CurrentScene { get; set; }

        private const string TITLE_SCENE_NAME = "TitleScene";
        private const string MAIN_LOBBY_SCENE_NAME = "MainLobbyScene";
        private const string CUSTOMIZE_SCENE_NAME = "CustomizeScene";
        private const string SOLO_SCENE_NAME = "SoloScene";

        private const string LOADING_SCENE_NAME = "LoadingScene";
        private CanvasGroup fadeLoadingCG;
        [Range(0.5f, 2.0f)]
        public float fadeDuration = 1.0f;

        private bool isLoadingDone = false;
        public bool IsLoadingDone { get { return isLoadingDone; } }
        private float progressAmount = 0f;
        public float Progress { get { return progressAmount; } }
        private bool allowNextScene = false;
        public bool AllowNextScene { set { allowNextScene = value; } }

        private WaitForSeconds ws10 = new WaitForSeconds(1.0f);

        private void Awake()
        {
            if (sceneLoadMgr == null)
            {
                sceneLoadMgr = this;
                DontDestroyOnLoad(this);
            }
            else if (sceneLoadMgr != this)
                Destroy(this.gameObject);
        }

        private string SceneTypeToString(LobbySceneType sceneType)
        {
            switch(sceneType)
            {
                case LobbySceneType.TITLE:
                    return TITLE_SCENE_NAME;
                case LobbySceneType.MAIN_LOBBY:
                    return MAIN_LOBBY_SCENE_NAME;
                case LobbySceneType.CUSTOMIZE:
                    return CUSTOMIZE_SCENE_NAME;
                case LobbySceneType.SOLO:
                    return SOLO_SCENE_NAME;
                default:
                    return MAIN_LOBBY_SCENE_NAME;
            }
        }

        public IEnumerator LoadLobbyScene(LobbySceneType sceneType, bool isAuto)
        {
            ResetFields();
            if (isAuto)
                yield return StartCoroutine(FadeInLoading());
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneTypeToString(sceneType), 
                isAuto ? LoadSceneMode.Additive : LoadSceneMode.Single);
            //asyncOperation.allowSceneActivation = false;
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
            if (isAuto)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
                yield return ws10;
                yield return StartCoroutine(FadeOutLoading());
            }
        }

        public IEnumerator LoadGameScene()
        {
            ResetFields();
            yield return null;
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
            fadeLoadingCG = GameObject.Find("LoadingScenePanel").GetComponent<CanvasGroup>();
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
