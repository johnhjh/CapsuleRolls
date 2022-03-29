using Capsule.Entity;
using System.Collections;
using System.Collections.Generic;
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
        private GameObject actionGlow1 = null;
        private GameObject actionGlow2 = null;
        private Button showTutorialButton = null;
        private TutorialData tutorialData = null;

        private bool isTutorialPopup = false;
        public bool IsTutorialPopup
        {
            get
            {
                return isTutorialPopup;
            }
            set
            {
                isTutorialPopup = value;
                if (!value)
                    OnSkipButtonClick();
            }
        }

        private void Start()
        {
            SetComponents();
            SetTutorialDatas();
        }

        private void SetComponents()
        {
            SetComponents();
            tutorialDescCanvasGroup = GameObject.Find("TutorialUIDesc").GetComponent<CanvasGroup>();
            // Action Glows
            actionGlow1 = GameObject.Find("Glow_Action1");
            actionGlow2 = GameObject.Find("Glow_Action1");

            showTutorialButton = GameObject.Find("Button_Show_Tutorial").GetComponent<Button>();
        }

        private void SetTutorialDatas()
        {
            actionGlow1.SetActive(false);
            actionGlow2.SetActive(false);
            if (DataManager.Instance != null)
            {
                switch (DataManager.Instance.CurrentGameData.Kind)
                {
                    case GameKind.ROLL_THE_BALL:
                        tutorialData = new RollTheBallTutorialData();
                        break;
                    case GameKind.THROWING_FEEDER:
                    case GameKind.ATTACK_INVADER:
                        tutorialData = new TutorialData();
                        break;
                    default:
                        tutorialData = new RollTheBallTutorialData();
                        break;
                }
                switch(DataManager.Instance.CurrentGameData.Mode)
                {
                    case GameMode.ARCADE:
                        PlayerPrefs.GetInt("IsFirstPlayArcade", 0);
                        break;
                    case GameMode.STAGE:
                        switch(DataManager.Instance.CurrentGameData.Stage)
                        {
                            case GameStage.TUTORIAL_1:
                                PlayerPrefs.GetInt("IsFirstPlayStage", 0);
                                break;
                            case GameStage.TUTORIAL_2:
                                actionGlow1.SetActive(true);

                                break;
                            case GameStage.TUTORIAL_3:
                                actionGlow2.SetActive(true);

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

            PopupTutorial(false);
        }

        public void OnPrevDescButtonClick()
        {

        }

        public void OnNextDescButtonClick()
        {

        }

        public void PopupTutorial(bool isOn)
        {
            tutorialDescCanvasGroup.alpha = isOn ? 1f : 0f;
            tutorialDescCanvasGroup.blocksRaycasts = isOn;
            tutorialDescCanvasGroup.interactable = isOn;
        }
    }
}

