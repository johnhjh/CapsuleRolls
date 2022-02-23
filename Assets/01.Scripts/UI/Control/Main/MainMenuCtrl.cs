using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Capsule.Audio;

public class MainMenuCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Components
    private Image backgroundImage;
    private Image iconImage;
    private Text innerText;

    // Bool
    private bool isHovering;

    // Floats
    [Range(0.1f, 1.0f)]
    public float hoverDurationTime = 0.2f;
    private float curAlpha = 0f;
    private float curFontSize = 90f;
    private const float MIN_FONT_SIZE = 90f;
    private const float MAX_FONT_SIZE = 115f;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        iconImage = transform.GetChild(0).GetComponent<Image>();
        innerText = transform.GetChild(1).GetComponent<Text>();
    }

    void Start()
    {
        isHovering = false;
    }

    void Update()
    {
        MainMenuHover();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        SFXManager.Instance.PlaySFX(SFXEnum.HOVER);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    private void MainMenuHover()
    {
        if (isHovering)
        {
            if (curAlpha != 1f)
            {
                curAlpha += Time.deltaTime / hoverDurationTime;
                curFontSize += Time.deltaTime * 5 / hoverDurationTime;
                if (curFontSize > MAX_FONT_SIZE) curFontSize = MAX_FONT_SIZE;
                if (curAlpha > 1f) curAlpha = 1f;

                backgroundImage.color = new Color(1, 1, 1, curAlpha);
                iconImage.color = new Color(1, 1, 1, curAlpha);
                innerText.fontSize = Mathf.RoundToInt(curFontSize);
            }
        }
        else
        {
            if (curAlpha != 0f)
            {
                curAlpha -= Time.deltaTime / hoverDurationTime;
                curFontSize -= Time.deltaTime * 5 / hoverDurationTime;
                if (curFontSize < MIN_FONT_SIZE) curFontSize = MIN_FONT_SIZE;
                if (curAlpha < 0f) curAlpha = 0f;

                backgroundImage.color = new Color(1, 1, 1, curAlpha);
                iconImage.color = new Color(1, 1, 1, curAlpha);
                innerText.fontSize = Mathf.RoundToInt(curFontSize);
            }
        }
    }
}
