using UnityEngine;
using UnityEngine.UI;

namespace Capsule.UI
{
    public class ChangeTextByPlatform : MonoBehaviour
    {
        [TextArea]
        public string MobileText;

        [TextArea]
        public string PCText;

        private void Start()
        {
            if (CheckStringExists(PCText) &&
                Application.platform != RuntimePlatform.Android &&
                Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (TryGetComponent(out Text _text))
                    _text.text = PCText;
            }
            else if (CheckStringExists(MobileText) &&
                (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer))
            {
                if (TryGetComponent(out Text _text))
                    _text.text = MobileText;
            }
        }

        private static bool CheckStringExists(string str)
        {
            return !string.IsNullOrEmpty(str) && 
                str.Length > 0;
        }
    }
}
