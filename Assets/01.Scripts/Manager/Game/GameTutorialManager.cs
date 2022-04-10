using Capsule.Entity;
using UnityEngine;
using UnityEngine.UI;
using Capsule.Audio;
using Capsule.Game.UI;

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
            if (!IsTutorialPopup) return;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {

            }
        }

        public void OnSkipButtonClick()
        {
            IsTutorialPopup = false;
        }

        public void OnPrevDescButtonClick()
        {

        }

        public void OnNextDescButtonClick()
        {

        }

        public bool PopupTutorial(bool isOn)
        {
            if (isOn && GameUIManager.Instance != null && (GameUIManager.Instance.IsLoading || GameUIManager.Instance.IsPopupActive)) return false;
            if (GameManager.Instance != null && GameManager.Instance.CheckSoloGame())
                Time.timeScale = isOn ? 0f : 1f;
            tutorialDescCanvasGroup.alpha = isOn ? 1f : 0f;
            tutorialDescCanvasGroup.blocksRaycasts = isOn;
            tutorialDescCanvasGroup.interactable = isOn;
            return isOn;
        }
    }
}

