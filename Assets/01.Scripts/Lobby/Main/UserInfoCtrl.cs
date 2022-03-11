using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Capsule.Audio;

public class UserInfoCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;
    [Range(0.1f, 2.0f)]
    public float hoverDurationTime = 0.6f;
    private const float MIN_ALPHA = 0f;
    private const float MAX_ALPHA = 1f;
    private float curAlpha = MIN_ALPHA;
    private Coroutine hoverCoroutine = null;

    private void Awake()
    {
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        SFXManager.Instance.PlayOneShotSFX(SFXType.HOVER);
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
}
