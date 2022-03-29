using Capsule.Audio;
using Capsule.SceneLoad;
using Capsule.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.Lobby.Title
{
    public class TitleManager : MonoBehaviour
    {
        private static TitleManager startManager;
        public static TitleManager Instance
        {
            get
            {
                if (startManager == null)
                    startManager = GameObject.FindObjectOfType<TitleManager>();
                return startManager;
            }
        }

        public event Action OnOpeningDone;
        public event Action OnReadyToStart;

        private bool isOpeningDone;
        public bool IsOpeningDone
        {
            get { return isOpeningDone; }
        }
        private bool isLoadingDone;

        private readonly Color basicColor = new Color(1f, 1f, 1f, 1f);
        private Image loadingBar;
        private Image loadingBG;
        private Text loadingText;

        private void Awake()
        {
            if (startManager == null)
                startManager = this;
            else if (startManager != this)
                Destroy(this.gameObject);
        }

        void Start()
        {
            SceneLoadManager.Instance.CurrentScene = LobbySceneType.TITLE;
            isOpeningDone = false;
            isLoadingDone = false;

            loadingBG = GameObject.Find("Loading_BG_Bar").GetComponent<Image>();
            loadingBar = GameObject.Find("Loading_Progress_Bar").GetComponent<Image>();
            loadingText = GameObject.Find("Loading_Progress_Text").GetComponent<Text>();

            GameObject.Find("Text_Version").GetComponent<Text>().text = VersionCtrl.CurrentVersion;

            StartCoroutine(StartOpening());
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                if (!isOpeningDone)
                    OpeningDone();
                else if (isLoadingDone && SceneLoadManager.Instance.IsLoadingDone)
                {
                    SFXManager.Instance.PlayOneShot(MenuSFX.OK);
                    SceneLoadManager.Instance.AllowNextScene = true;
                }
            }
        }

        private IEnumerator ActivateLoadingProgress()
        {
            loadingBG.color = basicColor;
            loadingText.color = basicColor;
            loadingBar.color = basicColor;
            loadingBar.fillAmount = 0f;
            loadingText.text = "LOADING .. 000%";
            loadingText.GetComponent<BlinkText>().enabled = true;
            while (!isLoadingDone)
            {
                yield return null;
                loadingBar.fillAmount = SceneLoadManager.Instance.Progress;
                loadingText.text = "LOADING .. " + (SceneLoadManager.Instance.Progress * 100).ToString("000") + ("%");
                if (SceneLoadManager.Instance.Progress >= 0.9f)
                {
                    loadingBar.fillAmount = 1f;
                    loadingText.text = "LOADING .. 100%";
                    isLoadingDone = true;
                    StartCoroutine(ReadyToStart());
                    break;
                }
            }
        }

        IEnumerator StartOpening()
        {
            yield return new WaitForSeconds(3f);
            if (!isOpeningDone)
                OpeningDone();
        }

        private void OpeningDone()
        {
            isOpeningDone = true;
            OnOpeningDone?.Invoke();

            StartCoroutine(ActivateLoadingProgress());
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, false));
        }

        IEnumerator ReadyToStart()
        {
            yield return new WaitForSeconds(0.3f);
            loadingBar.gameObject.SetActive(false);
            loadingBG.gameObject.SetActive(false);
            loadingText.gameObject.SetActive(false);

            OnReadyToStart?.Invoke();
        }
    }
}
