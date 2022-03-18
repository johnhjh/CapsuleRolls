using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Lobby
{
    public class OpenCloseChangeName : MonoBehaviour, IPointerClickHandler
    {
        public bool isOpen = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            UserInfoManager.Instance.OpenCloseChangeNamePopup(isOpen);
        }
    }
}
