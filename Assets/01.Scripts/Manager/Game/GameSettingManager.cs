using Capsule.Game.RollTheBall;
using UnityEngine;
using UnityEngine.UI;
namespace Capsule.Game
{
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
            Destroy(bgmIcon);
            Destroy(sfxIcon);
            Destroy(announceIcon);
            Destroy(bgmSlider);
            Destroy(sfxSlider);
            Destroy(announceSlider);
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
        [HideInInspector]
        public RollingBallMove ballMove = null;
        [HideInInspector]
        public PlayerRollTheBallMove playerMove = null;
        private bool isAutoAdjust = false;
        private Image toggleCheckImage = null;

        protected override void Start()
        {
            base.Start();
            Slider sliderRotate = GameObject.Find("Slider_Rotate").GetComponent<Slider>();
            sliderRotate.value = PlayerPrefs.GetFloat("PlayerRotSpeed", 65f);
            sliderRotate.onValueChanged.AddListener(delegate { OnSliderRotateValueChanged(sliderRotate); });
            isAutoAdjust = PlayerPrefs.GetInt("IsAutoAdjust", 1) == 1 ? true : false;
            toggleCheckImage = GameObject.Find("ToggleCheckImage").GetComponent<Image>();
            toggleCheckImage.color = new Color(1f, 1f, 1f, isAutoAdjust ? 1f : 0f);
        }

        public void OnSliderRotateValueChanged(Slider sliderRotate)
        {
            sliderRotate.value = Mathf.Clamp(sliderRotate.value, 10f, 120f);
            if (ballMove == null)
                ballMove = GameObject.Find("RollTheBallPlayer").GetComponent<RollingBallMove>();
            if (ballMove != null)
                ballMove.playerRotateSpeed = sliderRotate.value;
            PlayerPrefs.SetFloat("PlayerRotSpeed", sliderRotate.value);
        }

        public void ChangePositionAutoAdjust()
        {
            isAutoAdjust = !isAutoAdjust;
            toggleCheckImage.color = new Color(1f, 1f, 1f, isAutoAdjust ? 1f : 0f);
            if (playerMove == null)
            {
                if (ballMove == null)
                    ballMove = GameObject.Find("RollTheBallPlayer").GetComponent<RollingBallMove>();
                if (ballMove != null)
                    playerMove = ballMove.transform.GetChild(0).GetComponent<PlayerRollTheBallMove>();
                if (playerMove != null)
                    playerMove.IsAutoAdjust = isAutoAdjust;
            }
            else
                playerMove.IsAutoAdjust = isAutoAdjust;
            PlayerPrefs.SetInt("IsAutoAdjust", isAutoAdjust ? 1 : 0);
        }
    }
}

