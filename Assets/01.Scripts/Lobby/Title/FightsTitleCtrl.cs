using Capsule.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.Lobby.Title
{
    public class FightsTitleCtrl : MonoBehaviour
    {
        private Image fightsTitle;
        private RectTransform rectTransform;
        private WaitForSeconds ws001 = new WaitForSeconds(0.01f);
        public float downPosY = 40f;
        public float finalPosY = -110f;
        public float speed = 7f;
        private Vector2 finalPos;

        private void Start()
        {
            TitleManager.Instance.OnOpeningDone += () => ActivateAction();
            fightsTitle = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();

            fightsTitle.color = new Color(1f, 1f, 1f, 0f);
            finalPos = new Vector2(0, finalPosY);
            rectTransform.localPosition = finalPos - Vector2.up * downPosY;
            rectTransform.localScale = Vector3.one * 3f;

            StartCoroutine(TitleAppear());
        }

        IEnumerator TitleAppear()
        {
            yield return new WaitForSeconds(2.0f);
            float value = 0;
            while (!TitleManager.Instance.IsOpeningDone)
            {
                yield return ws001;
                value += speed;

                float newColor = 0.005f * value;
                if (newColor <= 1f)
                    fightsTitle.color = new Color(1f, 1f, 1f, newColor);

                float newPosition = finalPosY - downPosY + 0.2f * value;
                if (newPosition <= finalPosY)
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
            fightsTitle.color = new Color(1f, 1f, 1f, 1f);
            rectTransform.localPosition = finalPos;
            rectTransform.localScale = Vector3.one;

            GetComponent<ShakeSprite>().enabled = true;
            GetComponent<BounceSprite>().enabled = true;
        }
    }
}
