using UnityEngine;

public class BlinkSprite : MonoBehaviour
{
    private SpriteRenderer spriteObj;
    private float time;

    private void Awake()
    {
        spriteObj = GetComponent<SpriteRenderer>();
        time = 0f;
    }

    private void Update()
    {
        if (time < 0.5f)
            spriteObj.color = new Color(1, 1, 1, 1 - time);
        else
        {
            spriteObj.color = new Color(1, 1, 1, time);
            if (time > 1f)
                time = 0f;
        }
        time += Time.deltaTime;
    }
}
