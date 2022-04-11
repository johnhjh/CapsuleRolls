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

        private TutorialType currentTutorial = TutorialType.MENU;
        public TutorialType CurrentTutorial
        {
            get { return currentTutorial; }
            set
            {
                currentTutorial = value;
                CanvasGroupOnOff(tutorialMenuCG, false);
                CanvasGroupOnOff(gameKindTutorialCG, false);
                CanvasGroupOnOff(gameModeTutorialCG, false);
                CanvasGroupOnOff(gameInterfaceTutorialCG, false);
                CanvasGroupOnOff(gameControlTutorialCG, false);
                CanvasGroupOnOff(skipTutorialButtonCG, false);
                switch (value)
                {
                    case TutorialType.MENU:
                        CanvasGroupOnOff(tutorialMenuCG, true);
                        break;
                    case TutorialType.KIND:
                        CanvasGroupOnOff(skipTutorialButtonCG, true);
                        CanvasGroupOnOff(gameKindTutorialCG, true);
                        CurrentTextPanel = initKindTextPanel;
                        break;
                    case TutorialType.MODE:
                        CanvasGroupOnOff(skipTutorialButtonCG, true);
                        CanvasGroupOnOff(gameModeTutorialCG, true);
                        CurrentTextPanel = initModeTextPanel;
                        break;
                    case TutorialType.INTERFACE:
                        CanvasGroupOnOff(skipTutorialButtonCG, true);
                        CanvasGroupOnOff(gameInterfaceTutorialCG, true);
                        CurrentTextPanel = initInterfaceTextPanel;
                        break;
                    case TutorialType.CONTROL:
                        CanvasGroupOnOff(skipTutorialButtonCG, true);
                        CanvasGroupOnOff(gameControlTutorialCG, true);
                        CurrentTextPanel = initControlTextPanel;
                        break;
                    default:
                        CanvasGroupOnOff(tutorialMenuCG, true);
                        break;
                }
            }
        }
        private CanvasGroup tutorialDescCanvasGroup = null;
        private CanvasGroup tutorialMenuCG = null;
        private CanvasGroup gameKindTutorialCG = null;
        private CanvasGroup gameModeTutorialCG = null;
        private CanvasGroup gameInterfaceTutorialCG = null;
        private CanvasGroup gameControlTutorialCG = null;
        private CanvasGroup skipTutorialButtonCG = null;

        private TutorialTextPanelCtrl currentTextPanel = null;
        public TutorialTextPanelCtrl CurrentTextPanel
        {
            get { return currentTextPanel; }
            set
            {
                if (currentTextPanel != null && currentTextPanel.gameObject.activeSelf)
                    currentTextPanel.gameObject.SetActive(false);
                currentTextPanel = value;
                if (value != null)
                    value.gameObject.SetActive(true);
            }
        }
        private TutorialTextPanelCtrl initKindTextPanel = null;
        private TutorialTextPanelCtrl initModeTextPanel = null;
        private TutorialTextPanelCtrl initInterfaceTextPanel = null;
        private TutorialTextPanelCtrl initControlTextPanel = null;

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

        private bool wasTutorialMenuOpen = false;

        private void Start()
        {
            SetComponents();
            SetTutorialDatas();
        }

        private void SetComponents()
        {
            tutorialDescCanvasGroup = GameObject.Find("TutorialUIDesc").GetComponent<CanvasGroup>();
            tutorialMenuCG = GameObject.Find("TutorialMainMenus").GetComponent<CanvasGroup>();
            gameKindTutorialCG = GameObject.Find("GameKindTutorial").GetComponent<CanvasGroup>();
            gameModeTutorialCG = GameObject.Find("GameModeTutorial").GetComponent<CanvasGroup>();
            gameInterfaceTutorialCG = GameObject.Find("GameInterfaceTutorial").GetComponent<CanvasGroup>();
            gameControlTutorialCG = GameObject.Find("GameControlTutorial").GetComponent<CanvasGroup>();
            skipTutorialButtonCG = GameObject.Find("Button_Skip_Tutorial").GetComponent<CanvasGroup>();

            GameObject showTutorialButton = GameObject.Find("Button_Show_Tutorial");
            if (showTutorialButton != null)
            {
                showTutorialButton.GetComponent<Button>().onClick.AddListener(delegate
                {
                    CurrentTutorial = TutorialType.MENU;
                    IsTutorialPopup = true;
                });
            }
            CurrentTextPanel = null;
            initKindTextPanel = GameObject.Find("GameKindTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
            initModeTextPanel = GameObject.Find("GameModeTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
            initInterfaceTextPanel = GameObject.Find("GameInterfaceTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
            initControlTextPanel = GameObject.Find("GameControlTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
        }

        private void SetTutorialDatas()
        {
            if (DataManager.Instance != null)
            {
                switch (DataManager.Instance.CurrentGameData.Kind)
                {
                    case GameKind.GOAL_IN:
                    case GameKind.BATTLE_ROYAL:
                    case GameKind.GOLDEN_BALL:
                    case GameKind.NEXT_TARGET:
                        break;
                    default:
                        break;
                }
                switch (DataManager.Instance.CurrentGameData.Mode)
                {
                    case GameMode.ARCADE:
                        bool isFirstPlayArcade = PlayerPrefs.GetInt("IsFirstPlayArcade", 0) == 0;
                        break;
                    case GameMode.STAGE:
                        bool isFirstPlayStage = PlayerPrefs.GetInt("IsFirstPlayStage", 0) == 0;
                        switch (DataManager.Instance.CurrentGameData.Stage)
                        {
                            case GameStage.TUTORIAL_1:
                                break;
                            case GameStage.TUTORIAL_2:
                                break;
                            case GameStage.TUTORIAL_3:
                                break;
                        }
                        break;
                    case GameMode.PRACTICE:
                        bool isFirstPlayPractice = PlayerPrefs.GetInt("IsFirstPlayPractice", 0) == 0;
                        break;
                    case GameMode.BOT:
                        bool isFirstPlayBot = PlayerPrefs.GetInt("IsFirstPlayBot", 0) == 0;
                        break;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (!IsTutorialPopup)
                {
                    CurrentTutorial = TutorialType.MENU;
                    IsTutorialPopup = true;
                    return;
                }
                else if (CurrentTutorial == TutorialType.MENU)
                {
                    IsTutorialPopup = false;
                    return;
                }
            }
            if (!IsTutorialPopup || CurrentTextPanel == null) return;
            if (CurrentTutorial != TutorialType.MENU)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    if (CurrentTextPanel.HasPrevTextPanel())
                        CurrentTextPanel.ShowPrevTextPanel();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || 
                    Input.GetKeyDown(KeyCode.D) || 
                    Input.GetKeyDown(KeyCode.Space))
                {
                    if (CurrentTextPanel.HasNextTextPanel())
                        CurrentTextPanel.ShowNextTextPanel();
                    else if (CurrentTextPanel.IsLastTextPanel)
                        DoneTutorial();
                }
            }
        }

        public void OnExitButtonClick()
        {
            IsTutorialPopup = false;
        }

        public void OnSkipButtonClick()
        {
            DoneTutorial();
        }

        public void OnGameKindTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            wasTutorialMenuOpen = true;
            CurrentTutorial = TutorialType.KIND;
        }

        public void OnGameModeTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            wasTutorialMenuOpen = true;
            CurrentTutorial = TutorialType.MODE;
        }

        public void OnGameInterfaceTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            wasTutorialMenuOpen = true;
            CurrentTutorial = TutorialType.INTERFACE;
        }

        public void OnGameControlTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            wasTutorialMenuOpen = true;
            CurrentTutorial = TutorialType.CONTROL;
        }

        public void DoneTutorial()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            if (wasTutorialMenuOpen)
                CurrentTutorial = TutorialType.MENU;
            else
            {
                isTutorialPopup = false;
                PopupTutorial(false);
            }
        }

        public bool PopupTutorial(bool isOn)
        {
            if (isOn && 
                GameUIManager.Instance != null && 
                (GameUIManager.Instance.IsLoading || 
                GameUIManager.Instance.IsPopupActive)) return false;
            if (CurrentTutorial != TutorialType.MENU)
            {
                wasTutorialMenuOpen = false;
            }
            if (GameManager.Instance != null && GameManager.Instance.CheckSoloGame())
                Time.timeScale = isOn ? 0f : 1f;

            CanvasGroupOnOff(tutorialDescCanvasGroup, isOn);
            return isOn;
        }

        public void CanvasGroupOnOff(CanvasGroup cg, bool isOn)
        {
            cg.alpha = isOn ? 1f : 0f;
            cg.blocksRaycasts = isOn;
            cg.interactable = isOn;
        }
    }
}

