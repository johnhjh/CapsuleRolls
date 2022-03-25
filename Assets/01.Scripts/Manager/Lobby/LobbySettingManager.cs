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
            if (settingMgr == this)
                settingMgr = null;
        }

        private void Start()
        {
            InitLobbySettingManager();
        }

        private void InitLobbySettingManager()
        {
            settingCG = GameObject.Find("Popup_Setting").GetComponent<CanvasGroup>();
            PopUpSetting(false);

            bgmIcon = GameObject.Find("Icon_BGM").GetComponent<Image>();
            sfxIcon = GameObject.Find("Icon_SFX").GetComponent<Image>();
            announceIcon = GameObject.Find("Icon_ANNOUNCE").GetComponent<Image>();

            bgmSlider = GameObject.Find("Slider_BGM").GetComponent<Slider>();
            sfxSlider = GameObject.Find("Slider_SFX").GetComponent<Slider>();
            announceSlider = GameObject.Find("Slider_ANNOUNCE").GetComponent<Slider>();

            bgmSlider.value = PlayerPrefs.GetFloat(BGM_VOLUME, 1f);
            sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
            announceSlider.value = PlayerPrefs.GetFloat(ANNOUCE_VOLUME, 1f);

            bgmIcon.sprite = bgmSlider.value == 0f ? bgmOffSprite : bgmOnSprite;
            sfxIcon.sprite = sfxSlider.value == 0f ? sfxOffSprite : sfxOnSprite;
            announceIcon.sprite = announceSlider.value == 0f ? sfxOffSprite : sfxOnSprite;

            bgmSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChanged(); });
            sfxSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChanged(); });
            announceSlider.onValueChanged.AddListener(delegate { OnAnnounceVolumeChanged(); });

            Button buttonCloseSetting = GameObject.Find("Button_CloseSetting").GetComponent<Button>();
            buttonCloseSetting.onClick.AddListener(delegate { OnClickExitSetting(); });
            Button buttonBackToTitle = GameObject.Find("Button_BackToTitle").GetComponent<Button>();
            buttonBackToTitle.onClick.AddListener(delegate { OnClickBackToTitle(); });
            Button buttonExitGame = GameObject.Find("Button_ExitGame").GetComponent<Button>();
            buttonExitGame.onClick.AddListener(delegate { OnClickExitGame(); });

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
            Application.Quit();
        }
    }
}
