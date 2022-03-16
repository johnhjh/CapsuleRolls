using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Audio
{
    [System.Serializable]
    public class MenuSoundEffects
    {
        public AudioClip okClip;
        public AudioClip backClip;
        public AudioClip hoverClip;
        public AudioClip selectClip;
        public AudioClip selectDoneClip;
        public AudioClip loadDoneClip;
        public AudioClip buyClip;
        public AudioClip popupClip;
    }
    [System.Serializable]
    public class GameSoundEffects
    {
        public AudioClip moveClip;
        public AudioClip jumpClip;
    }
    [System.Serializable]
    public class AnnoucementSounds
    {
        public AudioClip enemyGoalClip;
        public AudioClip teamGoalClip;
    }

    public enum MenuSFX
    {
        OK = 0,
        BACK,
        HOVER,
        SELECT,
        SELECT_DONE,
        LOAD_DONE,
        BUY,
        POPUP,
    }

    public enum GameSFX
    { 
        MOVE = 0,
        JUMP,
    }

    public enum Announcements
    {

        ENEMY_GOAL,
        TEAM_GOAL,

    }


    public class SFXManager : MonoBehaviour
    {
        public MenuSoundEffects menuSoundEffects = new MenuSoundEffects();
        public GameSoundEffects gameSoundEffects = new GameSoundEffects();
        public AnnoucementSounds announceSounds = new AnnoucementSounds();

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

        private AudioClip GetAudioClip(MenuSFX sfx)
        {
            switch (sfx)
            {
                case MenuSFX.OK:
                    return menuSoundEffects.okClip;
                case MenuSFX.BACK:
                    return menuSoundEffects.backClip;
                case MenuSFX.HOVER:
                    return menuSoundEffects.hoverClip;
                case MenuSFX.SELECT:
                    return menuSoundEffects.selectClip;
                case MenuSFX.SELECT_DONE:
                    return menuSoundEffects.selectDoneClip;
                case MenuSFX.LOAD_DONE:
                    return menuSoundEffects.loadDoneClip;
                case MenuSFX.BUY:
                    return menuSoundEffects.buyClip;
                case MenuSFX.POPUP:
                    return menuSoundEffects.popupClip;
            }
            return null;
        }

        private AudioClip GetAudioClip(GameSFX sfx)
        {
            switch (sfx)
            {
                case GameSFX.MOVE:
                    return gameSoundEffects.moveClip;
                case GameSFX.JUMP:
                    return gameSoundEffects.jumpClip;
            }
            return null;
        }

        private AudioClip GetAudioClip(Announcements sfx)
        {
            switch (sfx)
            {
                case Announcements.ENEMY_GOAL:
                    return announceSounds.enemyGoalClip;
                case Announcements.TEAM_GOAL:
                    return announceSounds.teamGoalClip;
            }
            return null;
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip != null)
            {
                sfxAudioSource.clip = clip;
                if (!sfxAudioSource.isPlaying)
                    sfxAudioSource.Play();
            }
        }

        private void PlaySFX(AudioClip clip, float delay)
        {
            if (clip != null)
            {
                sfxAudioSource.clip = clip;
                if (!sfxAudioSource.isPlaying)
                    sfxAudioSource.PlayDelayed(delay);
            }
        }

        public void PlaySFX(MenuSFX sfx)
        {
            PlaySFX(GetAudioClip(sfx));
        }

        public void PlaySFX(GameSFX sfx)
        {
            PlaySFX(GetAudioClip(sfx));
        }

        public void PlaySFX(GameSFX sfx, float delay)
        {
            PlaySFX(GetAudioClip(sfx), delay);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip != null)
                sfxAudioSource.PlayOneShot(clip);
        }

        public void PlayOneShot(MenuSFX sfx)
        {
            PlayOneShot(GetAudioClip(sfx));
        }

        public void PlayOneShot(GameSFX sfx)
        {
            PlayOneShot(GetAudioClip(sfx));
        }

        public void PlayOneShot(Announcements announce)
        {
            PlayOneShot(GetAudioClip(announce));
        }

        public void SetVolume(float volume)
        {
            sfxAudioSource.volume = volume;
        }
    }
}
