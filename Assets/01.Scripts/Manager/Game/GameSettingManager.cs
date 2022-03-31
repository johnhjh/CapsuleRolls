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

        protected override void Start()
        {
            base.Start();
            Slider sliderRotate = GameObject.Find("Slider_Rotate").GetComponent<Slider>();
            sliderRotate.value = PlayerPrefs.GetFloat("PlayerRotSpeed", 60f);
            sliderRotate.onValueChanged.AddListener(delegate { OnSliderRotateValueChanged(sliderRotate); });
        }

        public void OnSliderRotateValueChanged(Slider sliderRotate)
        {
            if (ballMove == null)
                ballMove = GameObject.Find("RollTheBallPlayer").GetComponent<RollingBallMove>();
            ballMove.playerRotateSpeed = sliderRotate.value;
            PlayerPrefs.SetFloat("PlayerRotSpeed", sliderRotate.value);
        }
    }
}

