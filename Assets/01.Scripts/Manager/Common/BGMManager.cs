using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.BGM
{
    public enum BGMType
    {
        MAIN = 0,
        CUSTOMIZE,
        BATTLE,
        CREDIT,
    }

    public class BGMManager : MonoBehaviour
    {
        public BGMType currentBGM = BGMType.MAIN;

        public AudioClip mainThemeMusic;
        public AudioClip customizeMusic;
        public AudioClip battleMusic;
        public AudioClip creditMusic;

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
                default:
                    return mainThemeMusic;
            }
        }

        public void ChangeBGM(BGMType bgm)
        {
            if (currentBGM == bgm) return;
            currentBGM = bgm;
            bgmAudioSource.clip = GetBGMAudioClip(bgm);
            bgmAudioSource.Play();
        }
    }
}