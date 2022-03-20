using Capsule.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Lobby
{
    public class UserInfoCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private CanvasGroup canvasGroup;
        [Range(0.1f, 2.0f)]
        public float hoverDurationTime = 0.6f;
        private readonly float MIN_ALPHA = 0f;
        private readonly float MAX_ALPHA = 1f;
        private float curAlpha;
        private Coroutine hoverCoroutine = null;

        private void Awake()
        {
            canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
            curAlpha = MIN_ALPHA;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.HOVER);
            if (hoverCoroutine != null)
                StopCoroutine(hoverCoroutine);
            hoverCoroutine = StartCoroutine(HoverCanvasGroup(MAX_ALPHA));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverCoroutine != null)
                StopCoroutine(hoverCoroutine);
            hoverCoroutine = StartCoroutine(HoverCanvasGroup(MIN_ALPHA));
        }

        private IEnumerator HoverCanvasGroup(float finalAlpha)
        {
            while (!Mathf.Approximately(curAlpha, finalAlpha))
            {
                curAlpha = Mathf.MoveTowards(curAlpha,
                    finalAlpha,
                    Time.deltaTime / hoverDurationTime);
                canvasGroup.alpha = curAlpha;
                yield return null;
            }
            hoverCoroutine = null;
            yield return null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UserInfoManager.Instance.OpenCloseUserInfoPopup(true);
        }
    }
}
