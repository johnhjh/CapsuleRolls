using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Audio
{
    [System.Serializable]
    public class SoundEffects
    {
        public AudioClip okClip;
        public AudioClip backClip;
        public AudioClip hoverClip;
        public AudioClip selectClip;
        public AudioClip selectDoneClip;
        public AudioClip loadDoneClip;
    }

    public enum SFXType
    {
        OK = 0,
        BACK,
        HOVER,
        SELECT,
        SELECT_DONE,
        LOAD_DONE,
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

        private AudioClip GetAudioClipByType(SFXType sfx)
        {
            switch (sfx)
            {
                case SFXType.OK:
                    return soundEffects.okClip;
                case SFXType.BACK:
                    return soundEffects.backClip;
                case SFXType.HOVER:
                    return soundEffects.hoverClip;
                case SFXType.SELECT:
                    return soundEffects.selectClip;
                case SFXType.SELECT_DONE:
                    return soundEffects.selectDoneClip;
                case SFXType.LOAD_DONE:
                    return soundEffects.loadDoneClip;
            }
            return null;
        }

        public void PlaySFX(SFXType sfx)
        {
            AudioClip clip = GetAudioClipByType(sfx);
            if (clip != null)
            {
                sfxAudioSource.clip = clip;
                if (!sfxAudioSource.isPlaying)
                    sfxAudioSource.Play();
            }
        }

        public void PlayOneShotSFX(SFXType sfx)
        {
            AudioClip clip = GetAudioClipByType(sfx);
            if (clip != null)
                sfxAudioSource.PlayOneShot(clip);
        }

        public void SetVolume(float volume)
        {
            sfxAudioSource.volume = volume;
        }
    }
}
