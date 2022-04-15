using Capsule.Audio;
using Capsule.Dev;
using Capsule.Entity;
using Capsule.SceneLoad;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.Lobby
{
    public class LobbySettingManager : SettingManager
    {
        private static LobbySettingManager settingMgr;
        public static LobbySettingManager Instance
        {
            get
            {
                if (settingMgr == null)
                    settingMgr = GameObject.FindObjectOfType<LobbySettingManager>();
                return settingMgr;
            }
        }

        private CanvasGroup settingCG;

        private bool isSettingOpen = false;
        public bool OtherOpened { get; set; }

        private void Awake()
        {
            if (settingMgr == null)
                settingMgr = this;
            else if (settingMgr != this)
            {
                Destroy(settingMgr.gameObject);
                settingMgr = this;
                InitLobbySettingManager();
            }
        }

        private void OnDestroy()
        {
            if (DevToolManager.Instance != null)
                Destroy(DevToolManager.Instance.gameObject);
            if (settingCG != null)
                Destroy(settingCG.gameObject);
            if (settingMgr == this)
                settingMgr = null;
        }

        protected override void Start()
        {
            InitLobbySettingManager();
        }

        private void InitLobbySettingManager()
        {
            SetSoundSettings();
            settingCG = GameObject.Find("Popup_Setting").GetComponent<CanvasGroup>();
            PopUpSetting(false);

            Button buttonCloseSetting = GameObject.Find("Button_CloseSetting").GetComponent<Button>();
            buttonCloseSetting.onClick.AddListener(delegate { OnClickExitSetting(); });
            Button buttonBackToTitle = GameObject.Find("Button_BackToTitle").GetComponent<Button>();
            buttonBackToTitle.onClick.AddListener(delegate { OnClickBackToTitle(); });
            Button buttonExitGame = GameObject.Find("Button_ExitGame").GetComponent<Button>();
            buttonExitGame.onClick.AddListener(delegate { OnClickExitGame(); });
            Button buttonAbout = GameObject.Find("Button_About").GetComponent<Button>();
            buttonAbout.onClick.AddListener(delegate { OnClickAboutButton(); });

            Button buttonDevTool = GameObject.Find("Button_DevTool").GetComponent<Button>();
            if (DevToolManager.Instance == null)
                Destroy(buttonDevTool.gameObject);
            else
                buttonDevTool.onClick.AddListener(delegate { DevToolManager.Instance.PopupDevTool(true); });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isSettingOpen)
                    OnClickExitSetting();
                else
                {
                    if (OtherOpened) return;
                    SFXManager.Instance.PlayOneShot(MenuSFX.OK);
                    PopUpSetting(true);
                }
            }
        }

        public void PopUpSetting(bool isPopUp)
        {
            isSettingOpen = isPopUp;
            if (settingCG == null)
                InitLobbySettingManager();
            settingCG.interactable = isPopUp;
            settingCG.blocksRaycasts = isPopUp;
            settingCG.alpha = isPopUp ? 1f : 0f;
        }

        public void OnClickExitSetting()
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            PopUpSetting(false);
        }

        public void OnClickBackToTitle()
        {
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.TITLE, true));
        }

        public void OnClickExitGame()
        {
            DataManager.Instance.SaveBeforeQuit();

            //ProcessThreadCollection pt = Process.GetCurrentProcess().Threads;
            //foreach (ProcessThread p in pt)
            //    p.Dispose();
            //Process.GetCurrentProcess().Kill();

            Application.Quit();
        }

        public void OnClickAboutButton()
        {
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.CREDIT, true));
        }
    }
}
