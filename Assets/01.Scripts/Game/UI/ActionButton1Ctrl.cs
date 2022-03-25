using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Capsule.Game.UI
{
    public class ActionButton1Ctrl : GameUIHover, IPointerClickHandler
    {
        private static ActionButton1Ctrl actionButton;
        public static ActionButton1Ctrl Instance
        {
            get
            {
                if (actionButton == null)
                    actionButton = GameObject.FindObjectOfType<ActionButton1Ctrl>();
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

        public event Action OnClickActionButton1;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsActivated)
                OnClickActionButton1?.Invoke();
        }
    }
}
