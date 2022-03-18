using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Lobby
{
    public class OpenCloseUserInfo : MonoBehaviour, IPointerClickHandler
    {
        public bool isOpen = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            UserInfoManager.Instance.OpenCloseUserInfoPopup(isOpen);
        }
    }
}
