using UnityEngine;
using Capsule.Audio;

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

        public bool IsLastTextPanel = false;

        private void Start()
        {
            if (PrevTextPanel != null)
                this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (EnableTogether != null)
                EnableTogether.SetActive(true);
        }

        private void OnDisable()
        {
            if (DisableTogether != null)
                DisableTogether.SetActive(false);
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
        }

        public void ShowPrevTextPanel()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.OK);
            if (PrevTextPanel != null)
                PrevTextPanel.gameObject.SetActive(true);
            if (GameTutorialManager.Instance != null)
                GameTutorialManager.Instance.CurrentTextPanel = PrevTextPanel;
            this.gameObject.SetActive(false);
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