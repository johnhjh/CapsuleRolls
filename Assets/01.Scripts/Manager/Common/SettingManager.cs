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