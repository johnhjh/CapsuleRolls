using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class GameUIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.IsUIHover = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.IsUIHover = false;
        }
    }
}
