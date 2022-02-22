using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.UI.Common.Sprite
{
    public class FadeSprite : MonoBehaviour
    {
        private Image fadeInImage;

        [Range(0.5f, 2.0f)]
        public float fadeDuration = 1f;
        [Range(0f, 1f)]
        public float startAlpha = 0f;
        [Range(0f, 1f)]
        public float finalAlpha = 1f;
        private float curAlpha;

        private void Awake()
        {
            fadeInImage = GetComponent<Image>();
        }

        private void Start()
        {
            curAlpha = startAlpha;
            fadeInImage.color = new Color(1, 1, 1, curAlpha);
            StartCoroutine(FadeInOut());
        }

        private IEnumerator FadeInOut()
        {
            float fadeSpeed = Mathf.Abs(startAlpha - finalAlpha) / fadeDuration;
            while (!Mathf.Approximately(curAlpha, finalAlpha))
            {
                curAlpha = Mathf.MoveTowards(curAlpha, finalAlpha, fadeSpeed * Time.deltaTime);
                fadeInImage.color = new Color(1, 1, 1, curAlpha);
                yield return null;
            }
        }
    }
}