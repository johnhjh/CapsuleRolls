using UnityEngine;

namespace Capsule.Audio
{
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

        private void Awake()
        {
            if (audioListenerMgr == null)
            {
                audioListenerMgr = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (audioListenerMgr != this)
                Destroy(this.gameObject);
        }
    }
}
