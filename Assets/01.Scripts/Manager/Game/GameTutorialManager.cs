using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Capsule.Game
{
    public enum TutorialType
    {
        MENU,
        MODE,
        OBSTACLE,
        INTERFACE,
        CONTROL,
    }

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
                if (initVideoPlayer.isPlaying)
                    initVideoPlayer.Stop();
                currentTutorial = value;
                CanvasGroupOnOff(tutorialMenuCG, false);
                CanvasGroupOnOff(gameObstacleTutorialCG, false);
                CanvasGroupOnOff(gameModeTutorialCG, false);
                CanvasGroupOnOff(gameInterfaceTutorialCG, false);
                CanvasGroupOnOff(gameControlTutorialCG, false);
                CanvasGroupOnOff(skipTutorialButtonCG, false);
                switch (value)
                {
                    case TutorialType.MENU:
                        CanvasGroupOnOff(tutorialMenuCG, true);
                        break;
                    case TutorialType.OBSTACLE:
                        CanvasGroupOnOff(skipTutorialButtonCG, true);
                        CanvasGroupOnOff(gameObstacleTutorialCG, true);
                        CurrentTextPanel = initObstacleTextPanel;
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
                        initVideoPlayer.Play();
                        break;
                    default:
                        CanvasGroupOnOff(tutorialMenuCG, true);
                        break;
                }
            }
        }
        private CanvasGroup tutorialDescCanvasGroup = null;
        private CanvasGroup tutorialMenuCG = null;
        private CanvasGroup gameModeTutorialCG = null;
        private CanvasGroup gameObstacleTutorialCG = null;
        private CanvasGroup gameInterfaceTutorialCG = null;
        private CanvasGroup gameControlTutorialCG = null;
        private CanvasGroup skipTutorialButtonCG = null;

        private TutorialTextPanelCtrl currentTextPanel = null;
        public TutorialTextPanelCtrl CurrentTextPanel
        {
            get { return currentTextPanel; }
            set
            {
                if (value == null)
                {
                    if (currentTextPanel != null && currentTextPanel.gameObject.activeSelf)
                        currentTextPanel.gameObject.SetActive(false);
                }
                if (value != null)
                    value.gameObject.SetActive(true);
                currentTextPanel = value;
            }
        }
        private TutorialTextPanelCtrl initModeTextPanel = null;
        private TutorialTextPanelCtrl initObstacleTextPanel = null;
        private TutorialTextPanelCtrl initInterfaceTextPanel = null;
        private TutorialTextPanelCtrl initControlTextPanel = null;
        private VideoPlayer initVideoPlayer = null;

        private bool isTutorialPopup = false;
        public bool IsTutorialPopup
        {
            get
            {
                return isTutorialPopup;
            }
            set
            {
                if (value)
                    Cursor.visible = true;
                else if (GameSettingManager.Instance != null && !GameSettingManager.Instance.UsingCursor)
                    Cursor.visible = false;
                isTutorialPopup = PopupTutorial(value);
                if (SFXManager.Instance != null)
                    SFXManager.Instance.PlayOneShot(isTutorialPopup ? MenuSFX.LOAD_DONE : MenuSFX.BACK);
            }
        }

        private void Start()
        {
            SetComponents();
        }

        private void SetComponents()
        {
            tutorialDescCanvasGroup = GameObject.Find("TutorialUIDesc").GetComponent<CanvasGroup>();
            tutorialMenuCG = GameObject.Find("TutorialMainMenus").GetComponent<CanvasGroup>();
            gameModeTutorialCG = GameObject.Find("GameModeTutorial").GetComponent<CanvasGroup>();
            gameObstacleTutorialCG = GameObject.Find("GameObstacleTutorial").GetComponent<CanvasGroup>();
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
            initModeTextPanel = GameObject.Find("GameModeTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
            initObstacleTextPanel = GameObject.Find("GameObstacleTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
            initInterfaceTextPanel = GameObject.Find("GameInterfaceTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
            initControlTextPanel = GameObject.Find("GameControlTutorial").GetComponentsInChildren<TutorialTextPanelCtrl>()[0];
            initVideoPlayer = initControlTextPanel.EnableTogether.transform.GetComponentsInChildren<VideoPlayer>()[0];
            initVideoPlayer.Stop();
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

        public void OnGameModeTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            CurrentTutorial = TutorialType.MODE;
        }

        public void OnGameObstacleTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            CurrentTutorial = TutorialType.OBSTACLE;
        }

        public void OnGameInterfaceTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            CurrentTutorial = TutorialType.INTERFACE;
        }

        public void OnGameControlTutorialClick()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT);
            CurrentTutorial = TutorialType.CONTROL;
        }

        public void DoneTutorial()
        {
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            CurrentTextPanel = null;
            CurrentTutorial = TutorialType.MENU;
            if (initVideoPlayer.isPlaying)
                initVideoPlayer.Stop();
        }

        public bool PopupTutorial(bool isOn)
        {
            if (isOn &&
                GameUIManager.Instance != null &&
                (GameUIManager.Instance.IsLoading ||
                GameUIManager.Instance.IsPopupActive)) return false;
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

