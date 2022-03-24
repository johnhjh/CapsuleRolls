using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class ActionButton1Ctrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action OnClickActionButton1;

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
            OnClickActionButton1?.Invoke();
            if (OnClickActionButton1 != null)
            {
                Debug.Log("Hi");
            }
        }
    }
}
