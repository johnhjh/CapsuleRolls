using UnityEngine;

public class BounceSprite : MonoBehaviour
{
    private float time = 0;
    public float delayTime = 0f;
    public float upSize = 0.1f;
    private float upSizeTime = 0.5f;

    private RectTransform rectTransform;

    private void Awake()
    {
        time = 0f;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (time <= upSizeTime)
            rectTransform.localScale = Vector3.one * (1 + upSize * time);
        else if (time <= upSizeTime * 2)
            rectTransform.localScale = Vector3.one * (1 + 2 * upSizeTime * upSize - time * upSize);
        else if (time <= upSizeTime * 2 + delayTime)
            rectTransform.localScale = Vector3.one;
        else
            time = 0f;
        time += Time.deltaTime;
    }
}
