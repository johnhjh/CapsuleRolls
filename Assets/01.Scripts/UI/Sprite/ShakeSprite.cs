using UnityEngine;

namespace Capsule.UI
{
    public class ShakeSprite : MonoBehaviour
    {
        private float time = 0f;
        public float delayTime = 0f;
        public float rotAngle = 0.1f;
        private float rotTime = 0.5f;

        private RectTransform rectTransform;

        private void Awake()
        {
            time = 0f;
            rectTransform = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            RotateImageByTime();
        }

        private void RotateImageByTime()
        {
            if (time <= rotTime)
                rectTransform.Rotate(0f, 0f, rotAngle);
            else if (time <= rotTime * 3)
                rectTransform.Rotate(0f, 0f, -rotAngle);
            else if (time <= rotTime * 4)
                rectTransform.Rotate(0f, 0f, rotAngle);
            else if (time <= rotTime * 4 + delayTime)
                rectTransform.localRotation = Quaternion.identity;
            else
                time = 0f;
            time += Time.deltaTime;
        }
    }
}
