using UnityEngine;
using UnityEngine.UI;

namespace Capsule.UI
{
    public class FadeInSprite : MonoBehaviour
    {
        private Image fadeInImage;
        private float time = 0f;
        public float durationTime = 1f;
        private float curAlpha = 0f;

        private void Awake()
        {
            fadeInImage = GetComponent<Image>();
        }

        private void Start()
        {
            time = 0f;
            curAlpha = 0f;
            fadeInImage.color = new Color(1, 1, 1, 0);
        }

        void Update()
        {
            if (curAlpha == 1f) return;
            curAlpha = time / durationTime;
            if (curAlpha > 1f) curAlpha = 1f;
            fadeInImage.color = new Color(1, 1, 1, curAlpha);
            time += Time.deltaTime;
        }
    }
}
