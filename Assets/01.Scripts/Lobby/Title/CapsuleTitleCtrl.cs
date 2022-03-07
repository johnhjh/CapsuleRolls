using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Capsule.UI;

namespace Capsule.Lobby.Title
{
    public class CapsuleTitleCtrl : MonoBehaviour
    {
        private Image capsuleTitle;
        private RectTransform rectTransform;
        private WaitForSeconds ws001 = new WaitForSeconds(0.01f);
        public float upPosY = 40f;
        public float finalPosY = 300f;
        public float speed = 7f;
        private Vector2 finalPos;

        private void Start()
        {
            TitleManager.Instance.OnOpeningDone += () => ActivateAction();
            capsuleTitle = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();

            capsuleTitle.color = new Color(1f, 1f, 1f, 0f);
            finalPos = new Vector2(0, finalPosY);
            rectTransform.localPosition = finalPos + Vector2.up * upPosY;
            rectTransform.localScale = Vector3.one * 3f;

            StartCoroutine(TitleAppear());
        }

        IEnumerator TitleAppear()
        {
            yield return new WaitForSeconds(1.0f);
            float value = 0;
            while (!TitleManager.Instance.IsOpeningDone)
            {
                yield return ws001;
                value += speed;

                float newColor = 0.005f * value;
                if (newColor <= 1f)
                    capsuleTitle.color = new Color(1f, 1f, 1f, newColor);

                float newPosition = finalPosY + upPosY - 0.2f * value;
                if (newPosition >= finalPosY)
                    rectTransform.localPosition = new Vector2(0, newPosition);

                float newScale = 3f - 0.01f * value;
                if (newScale >= 1f)
                    rectTransform.localScale = Vector3.one * newScale;
            }
            yield return null;
        }


        private void ActivateAction()
        {
            StopCoroutine(TitleAppear());

            capsuleTitle.color = new Color(1f, 1f, 1f, 1f);
            rectTransform.localPosition = finalPos;
            rectTransform.localScale = Vector3.one;

            GetComponent<ShakeSprite>().enabled = true;
            GetComponent<BounceSprite>().enabled = true;
        }
    }
}
