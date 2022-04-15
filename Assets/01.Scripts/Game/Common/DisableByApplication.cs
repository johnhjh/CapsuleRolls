using UnityEngine;

namespace Capsule.Game
{
    public class DisableByApplication : MonoBehaviour
    {
        public bool DisableWhenMobile = false;

        private void Start()
        {
            if (DisableWhenMobile &&
                (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer))
                gameObject.SetActive(false);
            else if (!DisableWhenMobile &&
                (Application.platform != RuntimePlatform.Android &&
                Application.platform != RuntimePlatform.IPhonePlayer))
                gameObject.SetActive(false);
        }
    }
}
