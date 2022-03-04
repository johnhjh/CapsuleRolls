using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Capsule.Audio;
using Capsule.Entity;

namespace Capsule.Customize
{
    [RequireComponent (typeof (CanvasGroup))]
    public abstract class CustomizeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public CustomizingData data;
        private CanvasGroup canvasGroup;

        // Bool
        [SerializeField]
        private bool isSelected;
        public bool IsSelected
        {
            set
            {
                isSelected = value;
                if (hoverCoroutine != null)
                    StopCoroutine(hoverCoroutine);
                if (value)
                {
                    curAlpha = MAX_ALPHA;
                    canvasGroup.alpha = MAX_ALPHA;
                }
                else
                    hoverCoroutine = StartCoroutine(HoverSlot(MIN_ALPHA));
            }
        }

        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set { isLocked = value; }
        }

        // Floats
        [Range(0.1f, 2.0f)]
        public float hoverDurationTime = 0.6f;
        private const float MIN_ALPHA = 0.6f;
        private const float MAX_ALPHA = 1.0f;
        private float curAlpha = MIN_ALPHA;

        private Coroutine hoverCoroutine = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isLocked) return;
            if (!isSelected)
            {
                SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT);
                Equip();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isLocked) return;
            if (!isSelected)
            {
                SFXManager.Instance.PlayOneShotSFX(SFXType.HOVER);
                hoverCoroutine = StartCoroutine(HoverSlot(MAX_ALPHA));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isLocked) return;
            if (!isSelected)
            {
                if (hoverCoroutine != null)
                    StopCoroutine(hoverCoroutine);
                hoverCoroutine = StartCoroutine(HoverSlot(MIN_ALPHA));
            }
        }

        public abstract void Equip();

        private IEnumerator HoverSlot(float finalAlpha)
        {
            while (!Mathf.Approximately(curAlpha, finalAlpha))
            {
                if (isSelected) break;
                curAlpha = Mathf.MoveTowards(curAlpha,
                    finalAlpha,
                    Time.deltaTime / hoverDurationTime);
                canvasGroup.alpha = curAlpha;
                yield return null;
            }
            hoverCoroutine = null;
            yield return null;
        }
    }
}
