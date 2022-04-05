using Capsule.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule
{
    public class SettingManager : MonoBehaviour
    {
        protected readonly string BGM_VOLUME = "BGM_VOLUME";
        protected readonly string SFX_VOLUME = "SFX_VOLUME";
        protected readonly string ANNOUCE_VOLUME = "ANNOUNCE_VOLUME";

        [Header("Setting Icons")]
        public Sprite bgmOnSprite;
        public Sprite bgmOffSprite;
        public Sprite sfxOnSprite;
        public Sprite sfxOffSprite;

        protected Image bgmIcon;
        protected Image sfxIcon;
        protected Image announceIcon;
        protected Slider bgmSlider;
        protected Slider sfxSlider;
        protected Slider announceSlider;

        protected virtual void Start()
        {
            SetSoundSettings();
        }

        protected void SetSoundSettings()
        {
            bgmIcon = GameObject.Find("Icon_BGM").GetComponent<Image>();
            sfxIcon = GameObject.Find("Icon_SFX").GetComponent<Image>();
            announceIcon = GameObject.Find("Icon_ANNOUNCE").GetComponent<Image>();

            bgmSlider = GameObject.Find("Slider_BGM").GetComponent<Slider>();
            sfxSlider = GameObject.Find("Slider_SFX").GetComponent<Slider>();
            announceSlider = GameObject.Find("Slider_ANNOUNCE").GetComponent<Slider>();

            if (bgmSlider != null)
                bgmSlider.value = PlayerPrefs.GetFloat(BGM_VOLUME, 1f);
            if (sfxSlider != null)
                sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
            if (announceSlider != null)
                announceSlider.value = PlayerPrefs.GetFloat(ANNOUCE_VOLUME, 1f);

            if (bgmIcon != null)
                bgmIcon.sprite = bgmSlider.value == 0f ? bgmOffSprite : bgmOnSprite;
            if (sfxIcon != null)
                sfxIcon.sprite = sfxSlider.value == 0f ? sfxOffSprite : sfxOnSprite;
            if (announceIcon != null)
                announceIcon.sprite = announceSlider.value == 0f ? sfxOffSprite : sfxOnSprite;

            if (bgmSlider != null)
                bgmSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChanged(); });
            if (sfxSlider != null)
                sfxSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChanged(); });
            if (announceSlider != null)
                announceSlider.onValueChanged.AddListener(delegate { OnAnnounceVolumeChanged(); });
        }

        public void OnBGMVolumeChanged()
        {
            SFXManager.Instance.PlaySFX(MenuSFX.HOVER);
            float volume = bgmSlider.value;
            BGMManager.Instance.SetVolume(volume);
            PlayerPrefs.SetFloat(BGM_VOLUME, volume);
            if (volume == 0f)
                bgmIcon.sprite = bgmOffSprite;
            else if (bgmIcon.sprite == bgmOffSprite)
                bgmIcon.sprite = bgmOnSprite;
        }

        public void OnSFXVolumeChanged()
        {
            SFXManager.Instance.PlaySFX(MenuSFX.HOVER);
            float volume = sfxSlider.value;
            SFXManager.Instance.SetVolume(volume);
            PlayerPrefs.SetFloat(SFX_VOLUME, volume);
            if (volume == 0f)
                sfxIcon.sprite = sfxOffSprite;
            else if (sfxIcon.sprite == sfxOffSprite)
                sfxIcon.sprite = sfxOnSprite;
        }

        public void OnAnnounceVolumeChanged()
        {
            float volume = announceSlider.value;
            SFXManager.Instance.SetAnnounceVolume(volume);
            SFXManager.Instance.PlayAnnouncementTest();
            PlayerPrefs.SetFloat(ANNOUCE_VOLUME, volume);
            if (volume == 0f)
                announceIcon.sprite = sfxOffSprite;
            else if (announceIcon.sprite == sfxOffSprite)
                announceIcon.sprite = sfxOnSprite;
        }
    }
}