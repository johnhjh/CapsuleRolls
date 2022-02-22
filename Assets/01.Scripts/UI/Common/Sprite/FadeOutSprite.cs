using UnityEngine;
using UnityEngine.UI;

public class FadeOutSprite : MonoBehaviour
{
    private Image fadeOutImage;
    private float time = 0f;
    public float durationTime = 1f;
    private float curAlpha = 1f;

    private void Awake()
    {
        fadeOutImage = GetComponent<Image>();
    }

    private void Start()
    {
        time = 0f;
        curAlpha = 1f;
        fadeOutImage.color = new Color(1, 1, 1, 1);
    }

    void Update()
    {
        if (curAlpha == 0f) return;
        curAlpha = 1- time / durationTime;
        if (curAlpha < 0f) curAlpha = 0f;
        fadeOutImage.color = new Color(1, 1, 1, curAlpha);
        time += Time.deltaTime;
    }
}
