using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class ActionButton2Ctrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action OnClickActionButton2;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.IsUIHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.IsUIHover = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickActionButton2?.Invoke();
            if (OnClickActionButton2 != null)
            {
                Debug.Log("Hi");
            }
        }
    }
}
