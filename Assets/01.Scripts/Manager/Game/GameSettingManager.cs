using Capsule.Audio;
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

        protected override void Start()
        {
            base.Start();
        }
    }
}

