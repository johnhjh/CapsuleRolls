using UnityEngine;
using UnityEngine.UI;

public class UpScaleSprite : MonoBehaviour
{
    private Image upScaleImage;
    private RectTransform rectTransform;
    private float time = 0f;
    public float durationTime = 1f;
    public float startScale = 0f;
    public float endScale = 1f;
    private float curScale;

    private void Awake()
    {
        upScaleImage = GetComponent<Image>();
        rectTransform = upScaleImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        time = 0f;
        curScale = startScale;
        rectTransform.localScale = new Vector3(startScale, startScale, 1f);
    }

    private void Update()
    {
        if (curScale == endScale) return;
        curScale = time / durationTime;
        if (curScale > endScale) curScale = endScale;
        rectTransform.localScale = new Vector3(curScale, curScale, 1f);
        time += Time.deltaTime;
    }
}
