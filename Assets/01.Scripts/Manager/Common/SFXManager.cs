using System.Collections;
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
        public AudioClip fireworkClip;
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
        public AudioClip clearClip;
        public AudioClip congratulateClip;
        public AudioClip[] countClips;
        public AudioClip timesUpClip;
        public AudioClip newRecordClip;
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
        FIREWORK,
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
        CLEAR,
        CONGRAT,
        COUNT,
        TIMES_UP,
        NEW_RECORD,
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
        private AudioSource announceSource;
        private bool announceTesting = false;

        private void Awake()
        {
            if (sfxManager == null)
            {
                sfxManager = this;
                sfxAudioSource = GetComponent<AudioSource>();
                announceSource = transform.GetChild(0).GetComponent<AudioSource>();
                DontDestroyOnLoad(sfxManager);
            }
            else if (sfxManager != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            sfxAudioSource.volume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);
            announceSource.volume = PlayerPrefs.GetFloat("ANNOUNCE_VOLUME", 1f);
        }

        private void OnDestroy()
        {
            if (sfxManager == this)
            {
                PlayerPrefs.SetFloat("SFX_VOLUME", sfxAudioSource.volume);
                PlayerPrefs.SetFloat("ANNOUNCE_VOLUME", announceSource.volume);
            }
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
                case GameSFX.FIREWORK:
                    return gameSoundEffects.fireworkClip;
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
                case Announcements.CLEAR:
                    return announceSounds.clearClip;
                case Announcements.CONGRAT:
                    return announceSounds.congratulateClip;
                case Announcements.TIMES_UP:
                    return announceSounds.timesUpClip;
                case Announcements.NEW_RECORD:
                    return announceSounds.newRecordClip;
            }
            return null;
        }

        private AudioClip GetCountAudioClip(int count)
        {
            if (count <= announceSounds.countClips.Length)
                return announceSounds.countClips[count - 1];
            else
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

        // Plays
        private void PlaySFX(AudioClip clip)
        {
            if (sfxAudioSource.volume == 0f) return;
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
            if (sfxAudioSource.volume == 0f) return;
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
            if (sfxAudioSource.volume == 0f) return;
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
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null)
            {
                if (forceToPlay || !sfxAudioSource.isPlaying)
                {
                    sfxAudioSource.clip = clip;
                    sfxAudioSource.PlayDelayed(delay);
                }
            }
        }

        public void PlaySFX(AudioClip clip, AudioSource source)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null && source != null)
            {
                if (source.volume != sfxAudioSource.volume)
                    source.volume = sfxAudioSource.volume;
                if (!source.isPlaying)
                {
                    source.clip = clip;
                    source.Play();
                }
            }
        }

        public void PlaySFX(AudioClip clip, AudioSource source, bool forceToPlay)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null && source != null)
            {
                if (source.volume != sfxAudioSource.volume)
                    source.volume = sfxAudioSource.volume;
                if (forceToPlay || !source.isPlaying)
                {
                    source.clip = clip;
                    source.Play();
                }
            }
        }

        public void PlaySFX(AudioClip clip, AudioSource source, float delay)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null && source != null)
            {
                if (source.volume != sfxAudioSource.volume)
                    source.volume = sfxAudioSource.volume;
                if (!source.isPlaying)
                {
                    source.clip = clip;
                    source.PlayDelayed(delay);
                }
            }
        }

        public void PlaySFX(AudioClip clip, AudioSource source, float delay, bool forceToPlay)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null && source != null)
            {
                if (source.volume != sfxAudioSource.volume)
                    source.volume = sfxAudioSource.volume;
                if (forceToPlay || !source.isPlaying)
                {
                    source.clip = clip;
                    source.PlayDelayed(delay);
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

        public void PlaySFX(GameSFX sfx, AudioSource source)
        {
            PlaySFX(GetAudioClip(sfx), source);
        }

        public void PlaySFX(GameSFX sfx, AudioSource source, bool forceToPlay)
        {
            PlaySFX(GetAudioClip(sfx), source, forceToPlay);
        }

        public void PlaySFX(GameSFX sfx, AudioSource source, float delay)
        {
            PlaySFX(GetAudioClip(sfx), source, delay);
        }

        public void PlaySFX(GameSFX sfx, AudioSource source, float delay, bool forceToPlay)
        {
            PlaySFX(GetAudioClip(sfx), source, delay, forceToPlay);
        }

        public void PlaySFX(Crowds sfx)
        {
            PlaySFX(GetAudioClip(sfx));
        }

        public void PlaySFX(Crowds sfx, bool forceToPlay)
        {
            PlaySFX(GetAudioClip(sfx), forceToPlay);
        }

        public void PlaySFX(Announcements sfx, float delay)
        {
            if (announceSource.volume == 0f) return;
            StartCoroutine(PlayOneShotDelayed(GetAudioClip(sfx), announceSource, delay));
        }

        // PlayOneShots
        private void PlayOneShot(AudioClip clip)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null)
                sfxAudioSource.PlayOneShot(clip);
        }

        private void PlayOneShot(AudioClip clip, float volume)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null && volume > 0f)
                sfxAudioSource.PlayOneShot(clip, sfxAudioSource.volume * volume);
        }

        private void PlayOneShot(AudioClip clip, Vector3 position)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null)
            {
                GameObject soundObj = new GameObject("sfx");
                soundObj.transform.position = position;
                AudioSource audiosource = soundObj.AddComponent<AudioSource>();
                audiosource.clip = clip;
                audiosource.minDistance = 1f;
                audiosource.maxDistance = 30f;
                audiosource.volume = sfxAudioSource.volume;
                audiosource.Play();
                Destroy(soundObj, clip.length);
            }
        }

        private void PlayOneShot(AudioClip clip, Vector3 position, float volume)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null)
            {
                GameObject soundObj = new GameObject("sfx");
                soundObj.transform.position = position;
                AudioSource audiosource = soundObj.AddComponent<AudioSource>();
                audiosource.clip = clip;
                audiosource.minDistance = 1f;
                audiosource.maxDistance = 30f;
                audiosource.volume = sfxAudioSource.volume * volume;
                audiosource.Play();
                Destroy(soundObj, clip.length);
            }
        }

        private void PlayOneShot(AudioClip clip, AudioSource source)
        {
            if (source == announceSource)
            {
                if (announceSource.volume == 0f) return;
                if (clip != null)
                    source.PlayOneShot(clip);
            }
            else
            {
                if (sfxAudioSource.volume == 0f) return;
                if (clip != null && source != null)
                {
                    if (source.volume != sfxAudioSource.volume)
                        source.volume = sfxAudioSource.volume;
                    source.PlayOneShot(clip, sfxAudioSource.volume);
                }
            }
        }

        private void PlayOneShot(AudioClip clip, AudioSource source, float volume)
        {
            if (sfxAudioSource.volume == 0f) return;
            if (clip != null && source != null)
            {
                if (source.volume != sfxAudioSource.volume)
                    source.volume = sfxAudioSource.volume;
                source.PlayOneShot(clip, sfxAudioSource.volume * volume);
            }
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

        public void PlayOneShot(GameSFX sfx, Vector3 position)
        {
            PlayOneShot(GetAudioClip(sfx), position);
        }

        public void PlayOneShot(GameSFX sfx, Vector3 position, float volume)
        {
            PlayOneShot(GetAudioClip(sfx), position, volume);
        }

        public void PlayOneShot(GameSFX sfx, AudioSource source)
        {
            PlayOneShot(GetAudioClip(sfx), source);
        }

        public void PlayOneShot(GameSFX sfx, AudioSource source, float volume)
        {
            PlayOneShot(GetAudioClip(sfx), source, volume);
        }

        public void PlayOneShot(Announcements announce)
        {
            if (announceSource.volume == 0f) return;
                announceSource.PlayOneShot(GetAudioClip(announce));
        }

        public void PlayOneShot(Announcements announce, int count)
        {
            if (announceSource.volume == 0f) return;
            if (announce == Announcements.COUNT)
                announceSource.PlayOneShot(GetCountAudioClip(count));
        }

        public void PlayOneShot(Crowds crowd)
        {
            PlayOneShot(GetAudioClip(crowd));
        }

        private IEnumerator PlayOneShotDelayed(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayOneShot(clip);
        }

        private IEnumerator PlayOneShotDelayed(AudioClip clip, AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayOneShot(clip, source);
        }

        // Volume Settings
        public void SetVolume(float volume)
        {
            sfxAudioSource.volume = volume;
        }

        public void SetAnnounceVolume(float volume)
        {
            announceSource.volume = volume;
        }

        public void PlayAnnouncementTest()
        {
            if (announceTesting) return;
            announceTesting = true;
            AudioClip announceClip = GetAudioClip(Announcements.READY);
            announceSource.PlayOneShot(announceClip);
            StartCoroutine(AnnouncementTest(announceClip.length));
        }

        private IEnumerator AnnouncementTest(float duration)
        {
            yield return new WaitForSeconds(duration);
            announceTesting = false;
        }
    }
}
