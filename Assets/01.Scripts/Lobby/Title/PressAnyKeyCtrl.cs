using UnityEngine;
using Capsule.UI;

namespace Capsule.Lobby.Title
{
    public class PressAnyKeyCtrl : MonoBehaviour
    {
        public void Start()
        {
            TitleManager.Instance.OnReadyToStart += () => GetComponent<BlinkText>().enabled = true;
        }
    }
}
