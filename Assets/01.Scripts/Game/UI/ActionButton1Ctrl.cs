using System;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class ActionButton1Ctrl : GameUIHover, IPointerClickHandler
    {
        public event Action OnClickActionButton1;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickActionButton1?.Invoke();
        }
    }
}
