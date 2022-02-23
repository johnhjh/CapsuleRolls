using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.Audio;
using Capsule.SceneLoad;

public class SettingManager : MonoBehaviour
{
    private static SettingManager settingMgr;
    public static SettingManager Instance
    {
        get
        {
            if (settingMgr == null)
                settingMgr = GameObject.FindObjectOfType<SettingManager>();
            return settingMgr;
        }
    }

    private CanvasGroup settingCG;

    private const string BGM_VOLUME = "BGM_VOLUME";
    private const string SFX_VOLUME = "SFX_VOLUME";

    [Header("Setting Icons")]
    public Sprite bgmOnSprite;
    public Sprite bgmOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private Image bgmIcon;
    private Image sfxIcon;
    private Slider bgmSlider;
    private Slider sfxSlider;

    private void Awake()
    {
        if (settingMgr == null)
            settingMgr = this;
        else if (settingMgr != this)
            Destroy(this.gameObject);
    }
    
    private void Start()
    {
        settingCG = GameObject.Find("Popup_Setting").GetComponent<CanvasGroup>();
        PopUpSetting(false);

        bgmIcon = GameObject.Find("Icon_BGM").GetComponent<Image>();
        sfxIcon = GameObject.Find("Icon_SFX").GetComponent<Image>();

        bgmSlider = GameObject.Find("Slider_BGM").GetComponent<Slider>();
        sfxSlider = GameObject.Find("Slider_SFX").GetComponent<Slider>();

        bgmSlider.value = PlayerPrefs.GetFloat(BGM_VOLUME, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);

        bgmIcon.sprite = bgmSlider.value == 0f ? bgmOffSprite : bgmOnSprite;
        sfxIcon.sprite = sfxSlider.value == 0f ? sfxOffSprite : sfxOnSprite;
    }

    public void OnBGMVolumeChanged()
    {
        SFXManager.Instance.PlaySFX(SFXEnum.HOVER);
        float volume = bgmSlider.value;
        //Debug.Log("BGM : " + volume);
        BGMManager.Instance.SetVolume(volume);
        PlayerPrefs.SetFloat(BGM_VOLUME, volume);
        if (volume == 0f)
            bgmIcon.sprite = bgmOffSprite;
        else if (bgmIcon.sprite == bgmOffSprite)
            bgmIcon.sprite = bgmOnSprite;
    }

    public void OnSFXVolumeChanged()
    {
        SFXManager.Instance.PlaySFX(SFXEnum.HOVER);
        float volume = sfxSlider.value;
        //Debug.Log("SFX : " + volume);
        SFXManager.Instance.SetVolume(volume);
        PlayerPrefs.SetFloat(SFX_VOLUME, volume);
        if (volume == 0f)
            sfxIcon.sprite = sfxOffSprite;
        else if (sfxIcon.sprite == sfxOffSprite)
            sfxIcon.sprite = sfxOnSprite;
    }

    public void PopUpSetting(bool isPopUp)
    {
        settingCG.interactable = isPopUp;
        settingCG.blocksRaycasts = isPopUp;
        settingCG.alpha = isPopUp ? 1f : 0f;
    }

    public void OnClickExitSetting()
    {
        SFXManager.Instance.PlaySFX(SFXEnum.BACK);
        PopUpSetting(false);
    }

    public void OnClickBackToTitle()
    {
        StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.TITLE, true));
    }

    public void OnClickExitGame()
    {
        Application.Quit();
    }
}
