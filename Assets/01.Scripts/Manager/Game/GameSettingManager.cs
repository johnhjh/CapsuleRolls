using Capsule.Audio;
using Capsule.Game.Lights;
using Capsule.Game.RollTheBall;
using UnityEngine;
using UnityEngine.UI;
namespace Capsule.Game
{
    public enum GameSettingKind
    {
        SOUND = 0,
        CONTROL,
        GRAPHIC,
        INTERFACE,
    }

    public class GameSettingManager : SettingManager
    {
        private static GameSettingManager settingMgr;
        public static GameSettingManager Instance
        {
            get
            {
                if (settingMgr == null)
                    settingMgr = GameObject.FindObjectOfType<GameSettingManager>();
                return settingMgr;
            }
        }

        private void OnDestroy()
        {
            if (bgmIcon != null)
                Destroy(bgmIcon.gameObject);
            if (sfxIcon != null)
                Destroy(sfxIcon.gameObject);
            if (announceIcon != null)
                Destroy(announceIcon.gameObject);
            if (bgmSlider != null)
                Destroy(bgmSlider.gameObject);
            if (sfxSlider != null)
                Destroy(sfxSlider.gameObject);
            if (announceSlider != null)
                Destroy(announceSlider.gameObject);
        }

        private void Awake()
        {
            if (settingMgr == null)
                settingMgr = this;
            else if (settingMgr != this)
            {
                Destroy(settingMgr.gameObject);
                settingMgr = this;
                SetSoundSettings();
            }
        }

        private GameSettingKind currentSetting = GameSettingKind.SOUND;
        private GameSettingKind CurrentSetting
        {
            get { return currentSetting; }
            set
            {
                MenuSelected(currentSetting, false);
                MenuSelected(value, true);
                currentSetting = value;
            }
        }

        private Color selectedMenuColor = new Color(0.9647059f, 0.6588235f, 0.07843138f, 1f);
        private Color visibleColor = new Color(1f, 1f, 1f, 1f);
        private Color invisibleColor = new Color(1f, 1f, 1f, 0f);

        private Image menuSoundImage = null;
        private Image menuControlImage = null;
        private Image menuGraphicImage = null;
        private Image menuInterfaceImage = null;
        private CanvasGroup menuSoundCG = null;
        private CanvasGroup menuControlCG = null;
        private CanvasGroup menuGraphicCG = null;
        private CanvasGroup menuInterfaceCG = null;

        [HideInInspector]
        public RollingBallMove ballMove = null;
        [HideInInspector]
        public PlayerRollTheBallMove playerMove = null;

        private bool isAutoAdjust = false;
        private Image imageToggleCheckAutoAdjust = null;

        private bool usingVibration = false;
        public bool UsingVibration
        {
            get { return usingVibration; }
        }
        private Image imageToggleCheckVibration = null;

        [HideInInspector]
        public LightCtrl MainLight { get; set; }
        private bool usingLight = false;
        public bool UsingLight
        {
            get { return usingLight; }
        }
        private Image imageToggleCheckUsingLight = null;
        private float lightIntensity = 1f;
        public Button[] lightShadowButtons;
        private int lightShadow = 1;

        private bool usingJoystick = false;
        public bool UsingJoystick
        {
            get { return usingJoystick; }
        }
        private Image imageToggleCheckJoystick = null;
        private bool usingCursor = false;
        public bool UsingCursor
        {
            get { return usingCursor; }
        }
        private Image imageToggleCheckUsingCursor = null;

        protected override void Start()
        {
            base.Start();
            SetComponents();
        }

        private void SetComponents()
        {
            // Components
            menuSoundImage = GameObject.Find("GameSetting_Button_Sound").GetComponent<Image>();
            menuControlImage = GameObject.Find("GameSetting_Button_Control").GetComponent<Image>();
            menuGraphicImage = GameObject.Find("GameSetting_Button_Graphic").GetComponent<Image>();
            GameObject menuInterface = GameObject.Find("GameSetting_Button_Interface");
            if (menuInterface != null)
                menuInterfaceImage = menuInterface.GetComponent<Image>();
            menuSoundCG = GameObject.Find("GameSetting_Group_Sound").GetComponent<CanvasGroup>();
            menuControlCG = GameObject.Find("GameSetting_Group_Control").GetComponent<CanvasGroup>();
            menuGraphicCG = GameObject.Find("GameSetting_Group_Graphic").GetComponent<CanvasGroup>();
            menuInterfaceCG = GameObject.Find("GameSetting_Group_Interface").GetComponent<CanvasGroup>();

            // Control Settings
            Slider sliderRotate = GameObject.Find("Slider_Rotate").GetComponent<Slider>();
            sliderRotate.value = PlayerPrefs.GetFloat("PlayerRotSpeed", 100f);
            sliderRotate.onValueChanged.AddListener(delegate { OnSliderRotateValueChanged(sliderRotate); });

            Slider sliderViewY = GameObject.Find("Slider_ViewY").GetComponent<Slider>();
            sliderViewY.value = PlayerPrefs.GetFloat("ViewY", 1f);
            sliderViewY.onValueChanged.AddListener(delegate { OnSliderViewYValueChanged(sliderViewY); });

            isAutoAdjust = PlayerPrefs.GetInt("IsAutoAdjust", 1) == 1;
            imageToggleCheckAutoAdjust = GameObject.Find("Image_ToggleCheck_AutoAdjust").GetComponent<Image>();
            imageToggleCheckAutoAdjust.color = isAutoAdjust ? visibleColor : invisibleColor;

            usingVibration = PlayerPrefs.GetInt("UsingVibration", 1) == 1;
            imageToggleCheckVibration = GameObject.Find("Image_ToggleCheck_Vibration").GetComponent<Image>();
            imageToggleCheckVibration.color = usingVibration ? visibleColor : invisibleColor;

            // Graphic Settings
            usingLight = PlayerPrefs.GetInt("UsingLight", (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) ? 1 : 0) == 1;
            imageToggleCheckUsingLight = GameObject.Find("Image_ToggleCheck_UsingLight").GetComponent<Image>();
            imageToggleCheckUsingLight.color = usingLight ? visibleColor : invisibleColor;

            lightIntensity = PlayerPrefs.GetFloat("LightIntensity", 1f);
            Slider sliderLightIntensity = GameObject.Find("Slider_Intensity").GetComponent<Slider>();
            sliderLightIntensity.value = lightIntensity;
            sliderLightIntensity.onValueChanged.AddListener(delegate { OnSliderIntensityValueChanged(sliderLightIntensity); });

            lightShadow = PlayerPrefs.GetInt("LightShadow", 1);
            if (lightShadowButtons != null && lightShadowButtons.Length > 0)
            {
                for (int i = 0; i < lightShadowButtons.Length; i++)
                {
                    if (i != lightShadow)
                        lightShadowButtons[i].GetComponent<Image>().color = invisibleColor;
                }
                if (lightShadowButtons.Length > 2)
                {
                    lightShadowButtons[0].onClick.AddListener(delegate { OnClickShadowButton(0); });
                    lightShadowButtons[1].onClick.AddListener(delegate { OnClickShadowButton(1); });
                    lightShadowButtons[2].onClick.AddListener(delegate { OnClickShadowButton(2); });
                }
            }

            // Interface Settings
            imageToggleCheckJoystick = GameObject.Find("Image_ToggleCheck_JoyStick").GetComponent<Image>();
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                usingJoystick = true;
                PlayerPrefs.SetInt("UsingJoyStick", 1);
            }
            else
            {
                usingJoystick = PlayerPrefs.GetInt("UsingJoyStick", 1) == 1;
                imageToggleCheckJoystick.color = usingJoystick ? visibleColor : invisibleColor;
            }

            usingCursor = PlayerPrefs.GetInt("UsingCursor", 1) == 1;
            imageToggleCheckUsingCursor = GameObject.Find("Image_ToggleCheck_UsingCursor").GetComponent<Image>();
            imageToggleCheckUsingCursor.color = usingCursor ? visibleColor : invisibleColor;
        }

        public void OnSliderRotateValueChanged(Slider sliderRotate)
        {
            sliderRotate.value = Mathf.Clamp(sliderRotate.value, 10f, 200f);
            PlayerPrefs.SetFloat("PlayerRotSpeed", sliderRotate.value);
            if (ballMove == null)
            {
                GameObject rollTheBallPlayer = GameObject.Find("RollTheBallPlayer");
                if (rollTheBallPlayer != null)
                    ballMove = rollTheBallPlayer.GetComponent<RollingBallMove>();
            }
            if (ballMove != null)
                ballMove.playerRotateSpeed = sliderRotate.value;
        }

        public void OnSliderViewYValueChanged(Slider sliderViewY)
        {
            sliderViewY.value = Mathf.Clamp(sliderViewY.value, 0f, 2f);
            PlayerPrefs.SetFloat("ViewY", sliderViewY.value);
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (GameCameraManager.Instance != null)
                    GameCameraManager.Instance.SetCameraYSpeed(sliderViewY.value);
            }
        }

        public void ChangePositionAutoAdjust()
        {
            isAutoAdjust = !isAutoAdjust;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(isAutoAdjust ? MenuSFX.SELECT : MenuSFX.BACK);
            imageToggleCheckAutoAdjust.color = isAutoAdjust ? visibleColor : invisibleColor;
            if (playerMove == null)
            {
                if (ballMove == null)
                {
                    GameObject rollTheBallPlayer = GameObject.Find("RollTheBallPlayer");
                    if (rollTheBallPlayer != null)
                        ballMove = rollTheBallPlayer.GetComponent<RollingBallMove>();
                }
                if (ballMove != null)
                    playerMove = ballMove.transform.GetChild(0).GetComponent<PlayerRollTheBallMove>();
                if (playerMove != null)
                    playerMove.IsAutoAdjust = isAutoAdjust;
            }
            else
                playerMove.IsAutoAdjust = isAutoAdjust;
            PlayerPrefs.SetInt("IsAutoAdjust", isAutoAdjust ? 1 : 0);
        }

        public void ChangeUsingVibration()
        {
            usingVibration = !usingVibration;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(usingVibration ? MenuSFX.SELECT : MenuSFX.BACK);
            imageToggleCheckVibration.color = usingVibration ? visibleColor : invisibleColor;
            PlayerPrefs.SetInt("UsingVibration", usingVibration ? 1 : 0);
            if (usingVibration)
                Util.Vibration.Vibrate(1000);
        }

        public void ChangeUsingLight()
        {
            usingLight = !usingLight;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(usingLight ? MenuSFX.SELECT : MenuSFX.BACK);
            imageToggleCheckUsingLight.color = usingLight ? visibleColor : invisibleColor;
            PlayerPrefs.SetInt("UsingLight", usingLight ? 1 : 0);
            if (MainLight == null)
                MainLight = LightCtrl.Instance;
            if (MainLight != null)
                MainLight.SetLightActive(usingLight);
        }

        public void OnSliderIntensityValueChanged(Slider sliderIntensity)
        {
            sliderIntensity.value = Mathf.Clamp(sliderIntensity.value, 0f, 2f);
            lightIntensity = sliderIntensity.value;
            PlayerPrefs.SetFloat("LightIntensity", sliderIntensity.value);
            if (MainLight == null)
                MainLight = LightCtrl.Instance;
            if (MainLight != null)
                MainLight.SetIntensity(lightIntensity);
        }

        public void OnClickShadowButton(int shadow)
        {
            if (lightShadow == shadow) return;
            if (lightShadowButtons == null || lightShadowButtons.Length <= shadow) return;
            lightShadowButtons[lightShadow].GetComponent<Image>().color = invisibleColor;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            lightShadow = shadow;
            lightShadowButtons[lightShadow].GetComponent<Image>().color = visibleColor;
            PlayerPrefs.SetInt("LightShadow", lightShadow);
            if (MainLight == null)
                MainLight = LightCtrl.Instance;
            if (MainLight != null)
                MainLight.SetLightShadow(shadow);
        }

        public void ChangeUsingJoyStick()
        {
            usingJoystick = !usingJoystick;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(usingJoystick ? MenuSFX.SELECT : MenuSFX.BACK);
            imageToggleCheckJoystick.color = usingJoystick ? visibleColor : invisibleColor;
            PlayerPrefs.SetInt("UsingJoyStick", usingJoystick ? 1 : 0);
            if (UI.MobilePadsCtrl.Instance != null)
                UI.MobilePadsCtrl.Instance.UseMobilePads(usingJoystick);
        }

        public void ChangeUsingCursor()
        {
            usingCursor = !usingCursor;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(usingCursor ? MenuSFX.SELECT : MenuSFX.BACK);
            imageToggleCheckUsingCursor.color = usingCursor ? visibleColor : invisibleColor;
            PlayerPrefs.SetInt("UsingCursor", usingCursor ? 1 : 0);
        }

        private void CanvasGroupOnOff(CanvasGroup cg, bool isOn)
        {
            cg.alpha = isOn ? 1f : 0f;
            cg.blocksRaycasts = isOn;
            cg.interactable = isOn;
        }

        private void MenuSelected(GameSettingKind gk, bool isSelected)
        {
            switch (gk)
            {
                case GameSettingKind.SOUND:
                    menuSoundImage.color = isSelected ? selectedMenuColor : invisibleColor;
                    CanvasGroupOnOff(menuSoundCG, isSelected);
                    break;
                case GameSettingKind.CONTROL:
                    menuControlImage.color = isSelected ? selectedMenuColor : invisibleColor;
                    CanvasGroupOnOff(menuControlCG, isSelected);
                    break;
                case GameSettingKind.GRAPHIC:
                    menuGraphicImage.color = isSelected ? selectedMenuColor : invisibleColor;
                    CanvasGroupOnOff(menuGraphicCG, isSelected);
                    break;
                case GameSettingKind.INTERFACE:
                    if (menuInterfaceImage != null)
                        menuInterfaceImage.color = isSelected ? selectedMenuColor : invisibleColor;
                    CanvasGroupOnOff(menuInterfaceCG, isSelected);
                    break;
            }
        }

        public void OnClickGameSoundSettingMenu()
        {
            if (CurrentSetting == GameSettingKind.SOUND) return;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.OK);
            CurrentSetting = GameSettingKind.SOUND;
        }

        public void OnClickGameControlSettingMenu()
        {
            if (CurrentSetting == GameSettingKind.CONTROL) return;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.OK);
            CurrentSetting = GameSettingKind.CONTROL;
        }

        public void OnClickGameGraphicSettingMenu()
        {
            if (CurrentSetting == GameSettingKind.GRAPHIC) return;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.OK);
            CurrentSetting = GameSettingKind.GRAPHIC;
        }

        public void OnClickGameInterfaceSettingMenu()
        {
            if (CurrentSetting == GameSettingKind.INTERFACE) return;
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.OK);
            CurrentSetting = GameSettingKind.INTERFACE;
        }
    }
}

