using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Capsule.Audio;

namespace Capsule.Lobby
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class AbstractSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private CanvasGroup canvasGroup;

        // Bool
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
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

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!isSelected)
            {
                SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT);
                SelectSlot();
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!isSelected)
            {
                SFXManager.Instance.PlayOneShotSFX(SFXType.HOVER);
                hoverCoroutine = StartCoroutine(HoverSlot(MAX_ALPHA));
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!isSelected)
            {
                if (hoverCoroutine != null)
                    StopCoroutine(hoverCoroutine);
                hoverCoroutine = StartCoroutine(HoverSlot(MIN_ALPHA));
            }
        }

        public abstract void SelectSlot();

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

