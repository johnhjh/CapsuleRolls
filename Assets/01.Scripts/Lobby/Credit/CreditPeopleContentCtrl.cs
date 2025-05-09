﻿using UnityEngine;

namespace Capsule.Lobby.Credit
{
    public class CreditPeopleContentCtrl : MonoBehaviour
    {
        private RectTransform rectTransform;
        private bool isPeopleEnded = false;

        private void Awake()
        {
            isPeopleEnded = false;
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (isPeopleEnded) return;
            if (rectTransform.localPosition.y <= 3500f)
                rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y + 120f * Time.deltaTime);
            else
            {
                isPeopleEnded = true;
                CreditManager.Instance.ShowThankYouPanel();
            }
        }
    }
}
