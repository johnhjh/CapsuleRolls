using System;
using UnityEngine.EventSystems;

namespace Capsule.Game.UI
{
    public class ActionButton2Ctrl : GameUIHover, IPointerClickHandler
    {
        public event Action OnClickActionButton2;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickActionButton2?.Invoke();
        }
    }
}
