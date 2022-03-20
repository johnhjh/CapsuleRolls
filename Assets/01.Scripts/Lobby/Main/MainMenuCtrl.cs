using Capsule.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Capsule.Lobby
{
    [RequireComponent(typeof(Image))]
    public class MainMenuCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // Components
        private Image backgroundImage;
        private Image iconImage;
        private Text innerText;

        // Bool
        private bool isSelected;
        public bool IsSelected
        {
            set { isSelected = value; }
        }

        // Floats
        [Range(0.1f, 2.0f)]
        public float hoverDurationTime = 0.6f;

        private const float MIN_FONT_SIZE = 90f;
        private const float MAX_FONT_SIZE = 105f;

        [HideInInspector]
        public float finalAlpha = 0f;
        [HideInInspector]
        public float finalFontSize = MIN_FONT_SIZE;

        private float curAlpha = 0f;
        private float curFontSize = MIN_FONT_SIZE;
        private float fontSizing;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            iconImage = transform.GetChild(0).GetComponent<Image>();
            innerText = transform.GetChild(1).GetComponent<Text>();
            fontSizing = (MAX_FONT_SIZE - MIN_FONT_SIZE) / hoverDurationTime;
        }

        void Update()
        {
            MainMenuHover();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isSelected) return;
            finalAlpha = 1f;
            finalFontSize = MAX_FONT_SIZE;
            SFXManager.Instance.PlayOneShot(MenuSFX.HOVER);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isSelected) return;
            finalAlpha = 0f;
            finalFontSize = MIN_FONT_SIZE;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var mainMenus = GameObject.Find("MainMenu").GetComponentsInChildren<MainMenuCtrl>();
            if (mainMenus != null)
            {
                foreach (var mainMenu in mainMenus)
                {
                    mainMenu.IsSelected = false;
                    mainMenu.finalAlpha = 0f;
                    mainMenu.finalFontSize = MIN_FONT_SIZE;
                }
            }
            this.isSelected = true;
            this.finalAlpha = 1f;
            this.finalFontSize = MAX_FONT_SIZE;
        }

        private void MainMenuHover()
        {
            if (Mathf.Approximately(curAlpha, finalAlpha)) return;

            curAlpha = Mathf.MoveTowards(curAlpha,
                finalAlpha,
                Time.deltaTime / hoverDurationTime);

            curFontSize = Mathf.MoveTowards(curFontSize,
                finalFontSize,
                Time.deltaTime * fontSizing);

            backgroundImage.color = new Color(1, 1, 1, curAlpha);
            iconImage.color = new Color(1, 1, 1, curAlpha);
            innerText.fontSize = Mathf.RoundToInt(curFontSize);
        }
    }
}
