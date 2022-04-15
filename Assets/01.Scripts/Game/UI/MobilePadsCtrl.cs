using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.UI
{
    public class MobilePadsCtrl : MonoBehaviour
    {
        private static MobilePadsCtrl mobilePad;
        public static MobilePadsCtrl Instance
        {
            get
            {
                if (mobilePad == null)
                    mobilePad = FindObjectOfType<MobilePadsCtrl>();
                return mobilePad;
            }
        }

        private CanvasGroup cg = null;

        private void Awake()
        {
            if (mobilePad == null)
            {
                mobilePad = this;
                cg = GetComponent<CanvasGroup>();
            }
            else if (mobilePad != this)
            {
                Destroy(mobilePad.gameObject);
                mobilePad = this;
                cg = GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            SetCanvasGroupOnOff(PlayerPrefs.GetInt("UsingJoyStick", 1) == 1);
        }

        private void SetCanvasGroupOnOff(bool isOn)
        {
            if (cg == null)
                cg = GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = isOn ? 1f : 0f;
                cg.blocksRaycasts = isOn;
                cg.interactable = isOn;
            }
        }

        public void UseMobilePads(bool isUsing)
        {
            SetCanvasGroupOnOff(isUsing);
        }
    }
}
