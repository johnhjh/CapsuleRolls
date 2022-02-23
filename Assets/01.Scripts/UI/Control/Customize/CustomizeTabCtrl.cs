using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Capsule.Customize
{
    public class CustomizeTabCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // Components
        private RectTransform tabTransform;
        private Text tabText;
        public CustomizeType customizeType;

        // Bool
        private bool isFocused;
        public bool IsFocused
        {
            set { isFocused = value; }
        }

        // Floats
        [Range(0.1f, 1.0f)]
        public float sizingDurationTime = 0.2f;
        private float curFontSize = 63f;
        private const float MIN_FONT_SIZE = 63f;
        private const float MAX_FONT_SIZE = 70f;

        private Coroutine hoverCoroutine = null;

        private void Awake()
        {
            tabTransform = GetComponent<RectTransform>();
            tabText = GetComponent<Text>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isFocused) return;
            if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
            SFXManager.Instance.PlaySFX(SFXEnum.HOVER);
            hoverCoroutine = StartCoroutine(TabHover(true));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isFocused) return;
            if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
            hoverCoroutine = StartCoroutine(TabHover(false));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isFocused) return;
            CustomizeManager.Instance.ChangeFocusTab(tabTransform, customizeType);
            isFocused = true;
        }

        private IEnumerator TabHover(bool hover)
        {
            float finalFontSize = hover ? MAX_FONT_SIZE : MIN_FONT_SIZE;
            float sizingSpeed = Mathf.Abs(curFontSize - finalFontSize) / sizingDurationTime;
            while (!Mathf.Approximately(curFontSize, finalFontSize))
            {
                curFontSize = Mathf.MoveTowards(curFontSize, finalFontSize, sizingSpeed * Time.deltaTime);
                tabText.fontSize = Mathf.RoundToInt(curFontSize);
                yield return null;
            }
            hoverCoroutine = null;
        }
    }
}
