using UnityEngine;

namespace Capsule.Audio
{
    public enum BGMType
    {
        MAIN = 0,
        CUSTOMIZE,
        BATTLE,
        CREDIT,
        ARCADE,
        GAMEOVER,
    }

    public class BGMManager : MonoBehaviour
    {
        public BGMType currentBGM = BGMType.MAIN;

        public AudioClip mainThemeMusic;
        public AudioClip customizeMusic;
        public AudioClip battleMusic;
        public AudioClip creditMusic;
        public AudioClip arcadeMusic;
        public AudioClip gameOverMusic;

        private static BGMManager bgmManager;
        public static BGMManager Instance
        {
            get
            {
                if (bgmManager == null)
                    bgmManager = GameObject.FindObjectOfType<BGMManager>();
                return bgmManager;
            }
        }

        private AudioSource bgmAudioSource;

        private void Awake()
        {
            if (bgmManager == null)
            {
                bgmManager = this;
                bgmAudioSource = GetComponent<AudioSource>();
                DontDestroyOnLoad(bgmManager);
            }
            else if (bgmManager != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            bgmAudioSource.volume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
            bgmAudioSource.clip = GetBGMAudioClip(currentBGM);
            bgmAudioSource.Play();
        }

        private AudioClip GetBGMAudioClip(BGMType bgm)
        {
            switch (bgm)
            {
                case BGMType.MAIN:
                    return mainThemeMusic;
                case BGMType.CUSTOMIZE:
                    return customizeMusic;
                case BGMType.BATTLE:
                    return battleMusic;
                case BGMType.CREDIT:
                    return creditMusic;
                case BGMType.ARCADE:
                    return arcadeMusic;
                case BGMType.GAMEOVER:
                    return gameOverMusic;
                default:
                    return mainThemeMusic;
            }
        }

        public void ChangeBGM(BGMType bgm)
        {
            if (currentBGM == bgm)
            {
                if (!bgmAudioSource.isPlaying)
                    bgmAudioSource.Play();
                return;
            }
            currentBGM = bgm;
            bgmAudioSource.clip = GetBGMAudioClip(bgm);
            bgmAudioSource.Play();
        }

        public void PlayGameOver()
        {
            bgmAudioSource.Stop();
            bgmAudioSource.PlayOneShot(GetBGMAudioClip(BGMType.GAMEOVER));
        }

        public void SetVolume(float volume)
        {
            bgmAudioSource.volume = volume;
        }
    }
}