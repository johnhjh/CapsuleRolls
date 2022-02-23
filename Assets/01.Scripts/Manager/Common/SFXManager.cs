using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundEffects
{
    public AudioClip okClip;
    public AudioClip backClip;
    public AudioClip hoverClip;
    public AudioClip selectClip;
}

public enum SFXEnum
{
    OK = 0,
    BACK,
    HOVER,
    SELECT,
}

public class SFXManager : MonoBehaviour
{
    public SoundEffects soundEffects = new SoundEffects();
    private static SFXManager sfxManager;
    public static SFXManager Instance
    {
        get
        {
            if (sfxManager == null)
                sfxManager = GameObject.FindObjectOfType<SFXManager>();
            return sfxManager;
        }
    }

    private AudioSource sfxAudioSource;

    private void Awake()
    {
        if (sfxManager == null)
        {
            sfxManager = this;
            sfxAudioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(sfxManager);
        }
        else if (sfxManager != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        sfxAudioSource.volume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);
    }

    public void PlaySFX(SFXEnum sfx)
    {
        switch(sfx)
        {
            case SFXEnum.OK:
                sfxAudioSource.PlayOneShot(soundEffects.okClip);
                break;
            case SFXEnum.BACK:
                sfxAudioSource.PlayOneShot(soundEffects.backClip);
                break;
            case SFXEnum.HOVER:
                sfxAudioSource.PlayOneShot(soundEffects.hoverClip);
                break;
            case SFXEnum.SELECT:
                sfxAudioSource.PlayOneShot(soundEffects.selectClip);
                break;
        }
        
    }
}
