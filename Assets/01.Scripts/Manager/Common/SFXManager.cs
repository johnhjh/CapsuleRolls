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
        public AudioClip[] bounceClips;
        public AudioClip fallingClip;
        public AudioClip popClip;
    }
    [System.Serializable]
    public class AnnoucementSounds
    {
        public AudioClip[] outClips;
        public AudioClip[] successClips;
        public AudioClip enemyGoalClip;
        public AudioClip teamGoalClip;
        public AudioClip readyClip;
        public AudioClip goClip;
    }
    [System.Serializable]
    public class CrowdSounds
    {
        public AudioClip[] groanClips;
        public AudioClip[] applauseClips;
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
        BOUNCE,
        FALLING,
        POP,
    }

    public enum Crowds
    {
        APPLOUSE = 0,
        GROAN,
    }

    public enum Announcements
    {
        OUT = 0,
        SUCCESS,
        ENEMY_GOAL,
        TEAM_GOAL,
        READY,
        GO,
    }


    public class SFXManager : MonoBehaviour
    {
        public MenuSoundEffects menuSoundEffects = new MenuSoundEffects();
        public GameSoundEffects gameSoundEffects = new GameSoundEffects();
        public AnnoucementSounds announceSounds = new AnnoucementSounds();
        public CrowdSounds crowdSounds = new CrowdSounds();

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

        public AudioClip GetAudioClip(GameSFX sfx)
        {
            switch (sfx)
            {
                case GameSFX.MOVE:
                    return gameSoundEffects.moveClip;
                case GameSFX.JUMP:
                    return gameSoundEffects.jumpClip;
                case GameSFX.BOUNCE:
                    return gameSoundEffects.bounceClips[Random.Range(0, gameSoundEffects.bounceClips.Length)];
                case GameSFX.FALLING:
                    return gameSoundEffects.fallingClip;
                case GameSFX.POP:
                    return gameSoundEffects.popClip;
            }
            return null;
        }

        private AudioClip GetAudioClip(Announcements sfx)
        {
            switch (sfx)
            {
                case Announcements.OUT:
                    return announceSounds.outClips[Random.Range(0, announceSounds.outClips.Length)];
                case Announcements.SUCCESS:
                    return announceSounds.successClips[Random.Range(0, announceSounds.successClips.Length)];
                case Announcements.ENEMY_GOAL:
                    return announceSounds.enemyGoalClip;
                case Announcements.TEAM_GOAL:
                    return announceSounds.teamGoalClip;
                case Announcements.READY:
                    return announceSounds.readyClip;
                case Announcements.GO:
                    return announceSounds.goClip;
            }
            return null;
        }

        private AudioClip GetAudioClip(Crowds sfx)
        {
            switch (sfx)
            {
                case Crowds.APPLOUSE:
                    return crowdSounds.applauseClips[Random.Range(0, crowdSounds.applauseClips.Length)];
                case Crowds.GROAN:
                    return crowdSounds.groanClips[Random.Range(0, crowdSounds.groanClips.Length)];
            }
            return null;
        }

        public void StopSFX()
        {
            if (sfxAudioSource.isPlaying)
                sfxAudioSource.Stop();
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip != null)
            {
                if (!sfxAudioSource.isPlaying)
                {
                    sfxAudioSource.clip = clip;
                    sfxAudioSource.Play();
                }
            }
        }

        private void PlaySFX(AudioClip clip, bool forceToPlay)
        {
            if (clip != null)
            {
                if (forceToPlay || !sfxAudioSource.isPlaying)
                {
                    sfxAudioSource.clip = clip;
                    sfxAudioSource.Play();
                }
            }
        }

        private void PlaySFX(AudioClip clip, float delay)
        {
            if (clip != null)
            {
                if (!sfxAudioSource.isPlaying)
                {
                    sfxAudioSource.clip = clip;
                    sfxAudioSource.PlayDelayed(delay);
                }
            }
        }

        private void PlaySFX(AudioClip clip, float delay, bool forceToPlay)
        {
            if (clip != null)
            {
                if(forceToPlay || !sfxAudioSource.isPlaying)
                {
                    sfxAudioSource.clip = clip;
                    sfxAudioSource.PlayDelayed(delay);
                }
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

        public void PlaySFX(Crowds sfx)
        {
            PlaySFX(GetAudioClip(sfx));
        }

        public void PlaySFX(Crowds sfx, bool forceToPlay)
        {
            PlaySFX(GetAudioClip(sfx), forceToPlay);
        }

        public void PlaySFX(Announcements sfx)
        {
            PlaySFX(GetAudioClip(sfx), true);
        }

        public void PlaySFX(Announcements sfx, float delay)
        {
            PlaySFX(GetAudioClip(sfx), delay, true);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip != null)
                sfxAudioSource.PlayOneShot(clip);
        }

        private void PlayOneShot(AudioClip clip, float volume)
        {
            if (clip != null)
                sfxAudioSource.PlayOneShot(clip, volume);
        }

        public void PlayOneShot(MenuSFX sfx)
        {
            PlayOneShot(GetAudioClip(sfx));
        }

        public void PlayOneShot(GameSFX sfx)
        {
            PlayOneShot(GetAudioClip(sfx));
        }

        public void PlayOneShot(GameSFX sfx, float volume)
        {
            PlayOneShot(GetAudioClip(sfx), volume);
        }

        public void PlayOneShot(Announcements announce)
        {
            PlayOneShot(GetAudioClip(announce));
        }

        public void PlayOneShot(Announcements announce, float volume)
        {
            PlayOneShot(GetAudioClip(announce), volume);
        }

        public void PlayOneShot(Crowds crowd)
        {
            PlayOneShot(GetAudioClip(crowd));
        }

        public void SetVolume(float volume)
        {
            sfxAudioSource.volume = volume;
        }
    }
}
