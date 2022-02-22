using UnityEngine;
using UnityEngine.UI;

public class BlinkText : MonoBehaviour
{
    private Text textObj;
    private float time;

    private void Awake()
    {
        textObj = GetComponent<Text>();
        time = 0f;
    }

    private void Update()
    {
        if (time < 0.5f)
            textObj.color = new Color(1, 1, 1, 1 - time);
        else
        {
            textObj.color = new Color(1, 1, 1, time);
            if (time > 1f)
                time = 0f;
        }
        time += Time.deltaTime;
    }
}
