using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Capsule.Game.UI
{
    public class ActionButton2Ctrl : GameUIHover, IPointerClickHandler
    {
        private static ActionButton2Ctrl actionButton;
        public static ActionButton2Ctrl Instance
        {
            get
            {
                if (actionButton == null)
                    actionButton = GameObject.FindObjectOfType<ActionButton2Ctrl>();
                return actionButton;
            }
        }

        private bool isActivated = true;
        public bool IsActivated
        {
            get { return isActivated; }
            set
            {
                isActivated = value;
                this.GetComponent<Button>().interactable = value;
            }
        }

        private void Awake()
        {
            if (actionButton == null)
                actionButton = this;
            else if (actionButton != this)
                Destroy(this.gameObject);
        }

        public event Action OnClickActionButton2;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsActivated)
                OnClickActionButton2?.Invoke();
        }
    }
}
