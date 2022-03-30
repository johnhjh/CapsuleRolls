using Capsule.Entity;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Capsule.Lobby.SoloPlay
{
    public class GameKindSlot : AbstractSlot
    {
        public GameKind kind;
        public bool IsLocked;
        public bool IsNew;
        private Image hoverImage;
        private Image selectImage;

        protected override void Awake()
        {
            base.Awake();
            hoverImage = transform.GetChild(3).GetComponent<Image>();
            selectImage = transform.GetChild(4).GetComponent<Image>();
            hoverImage.color = new Color(1f, 1f, 1f, 0f);
            selectImage.color = new Color(1f, 1f, 1f, 0f);
        }

        private void Start()
        {
            if (!IsNew)
                transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            if (!IsLocked)
                transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            else
                canvasGroup.interactable = false;
            StartCoroutine(SetData());
        }

        private IEnumerator SetData()
        {
            yield return new WaitForSeconds(1.0f);
            GameKindData data = DataManager.Instance.GameKindDatas[(int)kind];
            transform.GetChild(2).GetComponent<Text>().text = data.name;
            if (data.preview != null)
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = data.preview;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsLocked || IsSelected) return;
            base.OnPointerClick(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (IsLocked || IsSelected) return;
            hoverImage.color = new Color(1f, 1f, 1f, 1f);
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (IsLocked || IsSelected) return;
            base.OnPointerExit(eventData);
            hoverImage.color = new Color(1f, 1f, 1f, 0f);
        }

        public override void SelectSlot()
        {
            SoloPlayManager.Instance.CurrentKindSlot = this;
            this.IsSelected = true;
            hoverImage.color = new Color(1f, 1f, 1f, 0f);
            selectImage.color = new Color(1f, 1f, 1f, 1f);
        }

        public void CancelSelect()
        {
            selectImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}