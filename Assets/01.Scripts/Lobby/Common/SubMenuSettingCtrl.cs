using UnityEngine.EventSystems;

namespace Capsule.Lobby
{
    public class SubMenuSettingCtrl : SubMenuCtrl
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (LobbySettingManager.Instance != null)
                LobbySettingManager.Instance.PopUpSetting(true);
        }
    }
}