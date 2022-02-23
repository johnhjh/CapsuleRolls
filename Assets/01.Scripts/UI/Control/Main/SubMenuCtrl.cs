using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Capsule.Audio;

public class SubMenuCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Components
    [SerializeField]
    private RectTransform parentTransform;
    [SerializeField]
    private RectTransform iconTransform;
    [SerializeField]
    private RectTransform textTransform;
    [SerializeField]
    private Text textFont;

    // Bool
    private bool isHovering;

    // Floats
    public float durationTime = 0.2f;

    private float curParentSize = 153f;
    private const float MIN_PARENT_SIZE = 153f;
    private const float MAX_PARENT_SIZE = 170f;
    private float parentSizing;

    private float curIconSize = 100f;
    private const float MIN_ICON_SIZE = 100f;
    private const float MAX_ICON_SIZE = 110f;
    private float iconSizing;

    private float curIconPos = 10f;
    private const float MIN_ICON_POS = 10f;
    private const float MAX_ICON_POS = 20f;
    private float iconPosing;

    private float curTextSize = 32f;
    private const float MIN_TEXT_SIZE = 32f;
    private const float MAX_TEXT_SIZE = 50f;
    private float textSizing;

    private float curFontSize = 24f;
    private const float MIN_FONT_SIZE = 24f;
    private const float MAX_FONT_SIZE = 30f;
    private float fontSizing;

    private void Awake()
    {
        parentTransform = GetComponent<RectTransform>();
        iconTransform = transform.GetChild(0).GetComponent<RectTransform>();
        textTransform = transform.GetChild(1).GetComponent<RectTransform>();
        textFont = transform.GetChild(1).GetComponent<Text>();
    }

    void Start()
    {
        isHovering = false;
        curParentSize = MIN_PARENT_SIZE;
        curIconSize = MIN_ICON_SIZE;
        curIconPos = MIN_ICON_POS;
        curTextSize = MIN_TEXT_SIZE;
        curFontSize = MIN_FONT_SIZE;

        parentSizing = (MAX_PARENT_SIZE - MIN_PARENT_SIZE) / durationTime;
        iconSizing = (MAX_ICON_SIZE - MIN_ICON_SIZE) / durationTime;
        iconPosing = (MAX_ICON_POS - MIN_ICON_POS) / durationTime;
        textSizing = (MAX_TEXT_SIZE - MIN_TEXT_SIZE) / durationTime;
        fontSizing = (MAX_FONT_SIZE - MIN_FONT_SIZE) / durationTime;
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

    private void Update()
    {
        SubMenuHover();
    }

    private void SubMenuHover()
    {
        if (isHovering)
        {
            if (curParentSize != MAX_PARENT_SIZE)
            {
                curParentSize += Time.deltaTime * parentSizing;
                if (curParentSize > MAX_PARENT_SIZE) curParentSize = MAX_PARENT_SIZE;

                curIconSize += Time.deltaTime * iconSizing;
                if (curIconSize > MAX_ICON_SIZE) curIconSize = MAX_ICON_SIZE;

                curIconPos += Time.deltaTime * iconPosing;
                if (curIconPos > MAX_ICON_POS) curIconPos = MAX_ICON_POS;

                curTextSize += Time.deltaTime * textSizing;
                if (curTextSize > MAX_TEXT_SIZE) curTextSize = MAX_TEXT_SIZE;

                curFontSize += Time.deltaTime * fontSizing;
                if (curFontSize > MAX_FONT_SIZE) curFontSize = MAX_FONT_SIZE;

                ResizeUI();
            }
        }
        else
        {
            if (curParentSize != MIN_PARENT_SIZE)
            {
                curParentSize -= Time.deltaTime * parentSizing;
                if (curParentSize < MIN_PARENT_SIZE) curParentSize = MIN_PARENT_SIZE;

                curIconSize -= Time.deltaTime * iconSizing;
                if (curIconSize < MIN_ICON_SIZE) curIconSize = MIN_ICON_SIZE;

                curIconPos -= Time.deltaTime * iconPosing;
                if (curIconPos < MIN_ICON_POS) curIconPos = MIN_ICON_POS;

                curTextSize -= Time.deltaTime * textSizing;
                if (curTextSize < MIN_TEXT_SIZE) curTextSize = MIN_TEXT_SIZE;

                curFontSize -= Time.deltaTime * fontSizing;
                if (curFontSize < MIN_FONT_SIZE) curFontSize = MIN_FONT_SIZE;

                ResizeUI();
            }
        }
    }

    private void ResizeUI()
    {
        parentTransform.sizeDelta = Vector2.one * curParentSize;
        iconTransform.sizeDelta = Vector2.one * curIconSize;
        iconTransform.localPosition = new Vector3(0f, curIconPos, 0f);
        textTransform.sizeDelta = Vector2.one * curTextSize;
        textFont.fontSize = Mathf.RoundToInt(curFontSize);
    }
}
