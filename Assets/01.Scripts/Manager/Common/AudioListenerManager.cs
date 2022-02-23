using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    private static AudioListenerManager audioListenerMgr;
    public static AudioListenerManager Instance
    {
        get
        {
            if (audioListenerMgr == null)
                audioListenerMgr = GameObject.FindObjectOfType<AudioListenerManager>();
            return audioListenerMgr;
        }
    }
    private AudioListener audioListener;

    private void Awake()
    {
        if (audioListenerMgr == null)
        {
            audioListenerMgr = this;
            audioListener = GetComponent<AudioListener>();
            DontDestroyOnLoad(this.gameObject);
        }
        else if (audioListenerMgr != this)
            Destroy(this.gameObject);
    }
}
