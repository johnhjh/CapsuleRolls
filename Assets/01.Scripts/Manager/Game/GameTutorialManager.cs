using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.Game
{

    public class GameTutorialManager : MonoBehaviour
    {
        private static GameTutorialManager tutorialMgr;
        public static GameTutorialManager Instance
        {
            get
            {
                if (tutorialMgr == null)
                    tutorialMgr = FindObjectOfType<GameTutorialManager>();
                return tutorialMgr;
            }
        }

        private void Awake()
        {
            if (tutorialMgr == null)
                tutorialMgr = this;
            else if (tutorialMgr != this)
                Destroy(this.gameObject);
        }

        private CanvasGroup tutorialDescCanvasGroup = null;
        private Button showTutorialButton = null;
        [HideInInspector]
        public TutorialTextPanelCtrl CurrentTextPanel = null;
        [SerializeField]
        public TutorialTextPanelCtrl InitTextPanel;

        private bool isTutorialPopup = false;
        public bool IsTutorialPopup
        {
            get
            {
                return isTutorialPopup;
            }
            set
            {
                isTutorialPopup = PopupTutorial(value);
                if (SFXManager.Instance != null)
                    SFXManager.Instance.PlayOneShot(isTutorialPopup ? MenuSFX.LOAD_DONE : MenuSFX.BACK);
            }
        }

        private void Start()
        {
            SetComponents();
            SetTutorialDatas();
        }

        private void SetComponents()
        {
            tutorialDescCanvasGroup = GameObject.Find("TutorialUIDesc").GetComponent<CanvasGroup>();
            showTutorialButton = GameObject.Find("Button_Show_Tutorial").GetComponent<Button>();
            showTutorialButton.onClick.AddListener(delegate { IsTutorialPopup = true; });
            CurrentTextPanel = InitTextPanel;
        }

        private void SetTutorialDatas()
        {
            if (DataManager.Instance != null)
            {
                switch (DataManager.Instance.CurrentGameData.Kind)
                {
                    case GameKind.GOAL_IN:
                        //tutorialData = new StageTutorialData();
                        break;
                    case GameKind.BATTLE_ROYAL:
                    case GameKind.GOLDEN_BALL:
                        //tutorialData = new TutorialData();
                        break;
                    default:
                        //tutorialData = new StageTutorialData();
                        break;
                }
                switch (DataManager.Instance.CurrentGameData.Mode)
                {
                    case GameMode.ARCADE:
                        PlayerPrefs.GetInt("IsFirstPlayArcade", 0);
                        break;
                    case GameMode.STAGE:
                        switch (DataManager.Instance.CurrentGameData.Stage)
                        {
                            case GameStage.TUTORIAL_1:
                                PlayerPrefs.GetInt("IsFirstPlayStage", 0);
                                break;
                            case GameStage.TUTORIAL_2:
                                break;
                            case GameStage.TUTORIAL_3:
                                break;
                        }
                        break;
                    case GameMode.PRACTICE:
                        break;
                    case GameMode.BOT:
                        break;
                }
            }
        }

        private void Update()
        {
            if (!IsTutorialPopup || CurrentTextPanel == null) return;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (CurrentTextPanel.HasPrevTextPanel())
                    CurrentTextPanel.ShowPrevTextPanel();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (CurrentTextPanel.HasNextTextPanel())
                    CurrentTextPanel.ShowNextTextPanel();
                else if (CurrentTextPanel.IsLastTextPanel)
                    DoneTutorial();
            }
        }

        public void OnSkipButtonClick()
        {
            IsTutorialPopup = false;
        }

        public void DoneTutorial()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            isTutorialPopup = false;
            PopupTutorial(false);
        }

        public bool PopupTutorial(bool isOn)
        {
            if (isOn && GameUIManager.Instance != null && (GameUIManager.Instance.IsLoading || GameUIManager.Instance.IsPopupActive)) return false;
            if (!isOn && CurrentTextPanel != null)
            {
                CurrentTextPanel.gameObject.SetActive(false);
                CurrentTextPanel = InitTextPanel;
            }
            else if (isOn)
            {
                if (InitTextPanel != null)
                {
                    if (CurrentTextPanel == null)
                        CurrentTextPanel = InitTextPanel;
                    else if (CurrentTextPanel != InitTextPanel)
                    {
                        CurrentTextPanel.gameObject.SetActive(false);
                        CurrentTextPanel = InitTextPanel;
                    }
                    if (!CurrentTextPanel.gameObject.activeSelf)
                        CurrentTextPanel.gameObject.SetActive(true);
                }
            }
            if (GameManager.Instance != null && GameManager.Instance.CheckSoloGame())
                Time.timeScale = isOn ? 0f : 1f;
            tutorialDescCanvasGroup.alpha = isOn ? 1f : 0f;
            tutorialDescCanvasGroup.blocksRaycasts = isOn;
            tutorialDescCanvasGroup.interactable = isOn;
            return isOn;
        }
    }
}

