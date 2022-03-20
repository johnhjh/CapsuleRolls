using Capsule.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Lobby
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class AbstractSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [HideInInspector]
        public CanvasGroup canvasGroup;

        // Bool
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
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
        private readonly float MIN_ALPHA = 0.6f;
        private readonly float MAX_ALPHA = 1.0f;
        private float curAlpha;
        private Coroutine hoverCoroutine = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            curAlpha = MIN_ALPHA;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!isSelected)
            {
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
                SelectSlot();
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!isSelected)
            {
                SFXManager.Instance.PlayOneShot(MenuSFX.HOVER);
                if (hoverCoroutine != null)
                    StopCoroutine(hoverCoroutine);
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

