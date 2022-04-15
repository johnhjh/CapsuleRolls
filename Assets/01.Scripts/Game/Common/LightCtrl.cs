using UnityEngine;

namespace Capsule.Game.Lights
{
    public class LightCtrl : MonoBehaviour
    {
        private static LightCtrl mainLightCtrl;
        public static LightCtrl Instance
        {
            get
            {
                if (mainLightCtrl == null)
                    mainLightCtrl = FindObjectOfType<LightCtrl>();
                return mainLightCtrl;
            }
        }

        private Light lightCtrl = null;

        private void Awake()
        {
            if (mainLightCtrl == null)
                mainLightCtrl = this;
            else if (mainLightCtrl != this)
            {
                Destroy(mainLightCtrl.gameObject);
                mainLightCtrl = this;
                SetLightCtrl();
            }
        }

        private void Start()
        {
            SetLightCtrl();
        }

        private void SetLightCtrl()
        {
            lightCtrl = GetComponent<Light>();
            if (GameSettingManager.Instance != null)
                GameSettingManager.Instance.MainLight = this;
            if (lightCtrl != null)
                lightCtrl.intensity = PlayerPrefs.GetFloat("LightIntensity", 1f);
            if (lightCtrl != null)
                lightCtrl.shadows = (LightShadows)PlayerPrefs.GetInt("LightShadow", 1);
            if (PlayerPrefs.GetInt("UsingLight", (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) ? 1 : 0) != 1)
                gameObject.SetActive(false);
        }

        public void SetLightShadow(int shadow)
        {
            if (lightCtrl != null)
                lightCtrl.shadows = (LightShadows)shadow;
        }

        public void SetIntensity(float intensity)
        {
            if (lightCtrl != null)
                lightCtrl.intensity = intensity;
        }

        public void SetLightActive(bool usingLight)
        {
            if (lightCtrl != null)
                gameObject.SetActive(usingLight);
        }
    }
}
