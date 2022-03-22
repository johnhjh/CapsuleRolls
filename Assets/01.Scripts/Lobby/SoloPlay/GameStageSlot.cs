using Capsule.Entity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Capsule.Lobby.SoloPlay
{
    public class GameStageSlot : AbstractSlot
    {
        public GameStage stage;
        [HideInInspector]
        public GameStageData data;
        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                isLocked = value;
                Transform childTransform = transform.GetChild(1).GetChild(0).GetChild(0);
                if (value)
                {
                    slotImage.sprite = SoloPlayManager.Instance.lockedStageSlot;
                    if (childTransform.TryGetComponent<Text>(out Text stageNumText))
                        stageNumText.color = SoloPlayManager.Instance.LockedStageTextColor;
                    else
                    {
                        if (childTransform.TryGetComponent<Image>(out Image stageImage))
                            stageImage.color = new Color(0f, 0f, 0f, 1f);
                    }
                }
                else
                {
                    slotImage.sprite = SoloPlayManager.Instance.unlockedStageSlot;
                    if (childTransform.TryGetComponent<Text>(out Text stageNumText))
                        stageNumText.color = new Color(1f, 1f, 1f, 1f);
                    else
                    {
                        if (childTransform.TryGetComponent<Image>(out Image stageImage))
                            stageImage.color = new Color(1f, 1f, 1f, 1f);
                    }
                }
            }
        }
        [HideInInspector]
        public Image slotImage;
        private Image focusImage;

        protected override void Awake()
        {
            base.Awake();
            slotImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
            focusImage = transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>();
            focusImage.color = new Color(1f, 1f, 1f, 0f);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsLocked || IsSelected) return;
            base.OnPointerClick(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            //if (IsLocked || IsSelected) return;
            SoloPlayManager.Instance.CurrentHoverStageSlot = this;
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            //if (IsLocked || IsSelected) return;
            SoloPlayManager.Instance.CurrentHoverStageSlot = null;
            base.OnPointerExit(eventData);
        }

        public override void SelectSlot()
        {
            SoloPlayManager.Instance.CurrentStageSlot = this;
            this.IsSelected = true;
            slotImage.sprite = SoloPlayManager.Instance.focusedStageSlot;
            focusImage.color = new Color(1f, 1f, 1f, 1f);
        }

        public void LockSlot()
        {

        }

        public void CancelSelect()
        {
            slotImage.sprite = SoloPlayManager.Instance.unlockedStageSlot;
            focusImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}