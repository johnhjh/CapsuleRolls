using Capsule.UI;
using UnityEngine;

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
