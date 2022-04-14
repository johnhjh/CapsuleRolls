using Capsule.Audio;
using UnityEngine;

namespace Capsule.Game.UI
{
    public class TutorialTextPanelCtrl : MonoBehaviour
    {
        [SerializeField]
        public TutorialTextPanelCtrl NextTextPanel;
        [SerializeField]
        public TutorialTextPanelCtrl PrevTextPanel;
        [SerializeField]
        public GameObject EnableTogether = null;
        [SerializeField]
        public GameObject DisableTogether = null;
        private GameObject enableWhenPrev = null;
        private GameObject disableWhenPrev = null;

        public bool IsLastTextPanel = false;
        private bool isShowingPrev = false;

        private void Start()
        {
            if (EnableTogether != null && DisableTogether == null)
                disableWhenPrev = EnableTogether;
            if (DisableTogether != null && EnableTogether == null)
                enableWhenPrev = DisableTogether;
            if (PrevTextPanel != null)
                this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (EnableTogether != null)
                EnableTogether.SetActive(true);
            if (enableWhenPrev != null && !enableWhenPrev.activeSelf)
                enableWhenPrev.SetActive(true);
            isShowingPrev = false;
        }

        private void OnDisable()
        {
            if (DisableTogether != null)
            {
                if (isShowingPrev && PrevTextPanel != null && PrevTextPanel.EnableTogether == DisableTogether)
                    DisableTogether.SetActive(true);
                else
                    DisableTogether.SetActive(false);
            }
        }

        public bool HasNextTextPanel()
        {
            return NextTextPanel != null;
        }

        public bool HasPrevTextPanel()
        {
            return PrevTextPanel != null;
        }

        public void ShowNextTextPanel()
        {
            if (NextTextPanel != null)
            {
                if (SFXManager.Instance != null)
                    SFXManager.Instance.PlayOneShot(MenuSFX.OK);
                NextTextPanel.gameObject.SetActive(true);
                if (GameTutorialManager.Instance != null)
                    GameTutorialManager.Instance.CurrentTextPanel = NextTextPanel;
                this.gameObject.SetActive(false);
            }
            else
            {
                if (SFXManager.Instance != null)
                    SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            }
        }

        public void ShowPrevTextPanel()
        {
            if (PrevTextPanel != null)
            {
                if (SFXManager.Instance != null)
                    SFXManager.Instance.PlayOneShot(MenuSFX.OK);
                if (disableWhenPrev != null && disableWhenPrev.activeSelf)
                    disableWhenPrev.SetActive(false);
                PrevTextPanel.gameObject.SetActive(true);
                if (GameTutorialManager.Instance != null)
                    GameTutorialManager.Instance.CurrentTextPanel = PrevTextPanel;
                isShowingPrev = true;
                this.gameObject.SetActive(false);
            }
            else
            {
                if (SFXManager.Instance != null)
                    SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            }
        }

        public void DoneTutorial()
        {
            if (GameTutorialManager.Instance != null)
                GameTutorialManager.Instance.DoneTutorial();
            else
                this.gameObject.SetActive(false);
        }
    }
}