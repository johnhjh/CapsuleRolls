using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.UI;
using System;
using System.Collections;
using UnityEngine;

namespace Capsule.Game
{
    public class GameNameData
    {
        public readonly string NAME_GAME_OBJECTS = "GameObjects";
        public readonly string NAME_ROLLING_BALL = "RollingBall";
    }

    public class GameTagData
    {
        public readonly string TAG_PLAYER = "Player";
        public readonly string TAG_STAGE = "Stage";
        public readonly string TAG_ROLLING_BALL = "RollingBall";
        public readonly string TAG_SPIKE_ROLLER = "SpikeRoller";
        public readonly string TAG_SWIPER = "Swiper";
        public readonly string TAG_GOAL_POST = "GoalPost";
        public readonly string TAG_GOAL_IN = "GoalInTrigger";
        public readonly string TAG_BARRICADE = "Barricade";
        public readonly string TAG_DEAD_ZONE = "DeadZone";
        public readonly string TAG_WAY_POINT = "WayPoint";
        public readonly string TAG_TUTORIAL_1 = "Tutorial1";
        public readonly string TAG_TUTORIAL_2 = "Tutorial2";
        public readonly string TAG_TUTORIAL_3 = "Tutorial3";
        public readonly string TAG_TUTORIAL_4 = "Tutorial4";
        public readonly string TAG_TUTORIAL_5 = "Tutorial5";
        public readonly string TAG_TUTORIAL_6 = "Tutorial6";
        public readonly string TAG_TUTORIAL_7 = "Tutorial7";
        public readonly string TAG_TUTORIAL_8 = "Tutorial8";
        public readonly string TAG_TUTORIAL_9 = "Tutorial9";
    }
    public class GameAnimData
    {
        public readonly int HASH_MOVE_SPEED = Animator.StringToHash("MoveSpeed");
        public readonly int HASH_HORIZONTAL = Animator.StringToHash("Horizontal");
        public readonly int HASH_VERTICAL = Animator.StringToHash("Vertical");
        public readonly int HASH_ROTATE = Animator.StringToHash("Rotate");
        public readonly int HASH_IS_TURNING = Animator.StringToHash("IsTurning");
        public readonly int HASH_TRIG_JUMP = Animator.StringToHash("TrigJump");
        public readonly int HASH_TRIG_STOP_JUMPING = Animator.StringToHash("TrigStopJumping");
        public readonly int HASH_TRIG_DIVE = Animator.StringToHash("TrigDive");
        public readonly int HASH_TRIG_FALLING = Animator.StringToHash("TrigFalling");
        public readonly int HASH_TRIG_VICTORY = Animator.StringToHash("TrigVictory");
        public readonly int HASH_VICTORY_ANIM = Animator.StringToHash("VictoryAnim");
        public readonly int HASH_TRIG_STAGE_CLEAR = Animator.StringToHash("TrigStageClear");
        public readonly int HASH_TRIG_STAGE_FAILURE = Animator.StringToHash("TrigStageFailure");
    }

    public class GameManager : MonoBehaviour
    {
        private static GameManager gameMgr;
        public static GameManager Instance
        {
            get
            {
                if (gameMgr == null)
                    gameMgr = FindObjectOfType<GameManager>();
                return gameMgr;
            }
        }

        private GameData currentGameData = null;
        public GameData CurrentGameData
        {
            get
            {
                if (currentGameData == null)
                {
                    if (DataManager.Instance != null)
                        currentGameData = DataManager.Instance.CurrentGameData;
                    if (currentGameData == null)
                        currentGameData = new GameData
                        {
                            Mode = GameMode.PRACTICE,
                            Kind = GameKind.ROLL_THE_BALL,
                        };
                }
                return currentGameData;
            }
        }

        public GameTagData tagData = new GameTagData();
        public GameAnimData animData = new GameAnimData();
        public GameNameData nameData = new GameNameData();

        public bool IsGameOver { get; private set; }
        private int teamScoreA = 0;
        public int TeamScoreA
        {
            get { return teamScoreA; }
        }
        private int teamScoreB = 0;
        public int TeamScoreB
        {
            get { return teamScoreB; }
        }

        public event Action OnAddScoreTeamA;
        public event Action OnAddScoreTeamB;
        public event Action OnStageClear;
        public event Action OnStageFailure;
        public event Action OnStartGame;
        public event Action OnTimeEnded;

        private void Awake()
        {
            if (AudioListenerManager.Instance != null)
                Destroy(AudioListenerManager.Instance.gameObject);
            if (gameMgr == null)
                gameMgr = this;
            else if (gameMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            IsGameOver = false;
            currentGameData = DataManager.Instance.CurrentGameData;
            if (CurrentGameData != null)
            {
                if (CurrentGameData.Mode == GameMode.ARCADE)
                    BGMManager.Instance.ChangeBGM(BGMType.ARCADE);
                else if (CurrentGameData.Mode == GameMode.STAGE)
                    BGMManager.Instance.ChangeBGM(BGMType.BATTLE);
            }
            SFXManager.Instance.PlaySFX(Announcements.READY, 1f);
            SFXManager.Instance.PlaySFX(Announcements.GO, 3f);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.IsLoading = false;
            OnStartGame?.Invoke();
        }

        public bool CheckSoloGame()
        {
            switch (CurrentGameData.Mode)
            {
                case GameMode.ARCADE:
                case GameMode.STAGE:
                case GameMode.PRACTICE:
                case GameMode.BOT:
                    return true;
            }
            return false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameUIManager.Instance != null)
                {
                    if (CheckSoloGame())
                        GameUIManager.Instance.PauseGame();
                    else
                        GameUIManager.Instance.ShowGameSetting();
                }
            }
        }

        public GameObject GetNewGameObj(bool usingRigidbody = false, bool usingAudioSource = false)
        {
            GameObject newGameObj = new GameObject { name = nameData.NAME_GAME_OBJECTS };
            if (usingRigidbody)
            {
                Rigidbody newRigidbody = newGameObj.AddComponent<Rigidbody>();
                newRigidbody.mass = 60f;
                newRigidbody.drag = 0.05f;
                newRigidbody.angularDrag = 0.05f;
                newRigidbody.useGravity = true;
            }
            if (usingAudioSource)
            {
                AudioSource newAudioSource = newGameObj.AddComponent<AudioSource>();
                newAudioSource.playOnAwake = false;
                newAudioSource.minDistance = 1f;
                newAudioSource.maxDistance = 5f;
            }
            return newGameObj;
        }

        public void TimeEnded()
        {
            if (IsGameOver) return;
            OnTimeEnded?.Invoke();
            switch (CurrentGameData.Mode)
            {
                case GameMode.ARCADE:
                    // 점수 계산 후 데이터 갱신 할 자리
                    break;
                case GameMode.STAGE:
                    StageFailure();
                    break;
                case GameMode.BOT:
                    // 점수 계산 후 승자를 가리는 자리
                    break;
            }

        }

        public void StageClear()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            SFXManager.Instance.PlayOneShot(Crowds.APPLOUSE);
            SFXManager.Instance.PlayOneShot(Announcements.CLEAR);
            OnStageClear?.Invoke();
            DataManager.Instance.CurrentPlayerStageClearData.StageClear(CurrentGameData.Stage);
            DataManager.Instance.CurrentPlayerGameData.PlayerStagePlayed((int)CurrentGameData.Stage, true);
            StartCoroutine(PopupClearUI());
        }

        public void StageFailure()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            OnStageFailure?.Invoke();
            DataManager.Instance.CurrentPlayerGameData.PlayerStagePlayed((int)CurrentGameData.Stage, false);
            StartCoroutine(PopupFailureUI());
        }

        private IEnumerator PopupClearUI()
        {
            yield return new WaitForSeconds(3.0f);
            SFXManager.Instance.PlayOneShot(Announcements.CONGRAT);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.OnStageClear();
        }

        private IEnumerator PopupFailureUI()
        {
            yield return new WaitForSeconds(3.0f);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.OnStageFailure();
        }

        public void AddScore(bool isTeamA, int newScore)
        {
            if (!IsGameOver)
            {
                if (isTeamA)
                {
                    teamScoreA += newScore;
                    OnAddScoreTeamA?.Invoke();
                }
                else
                {
                    teamScoreB += newScore;
                    OnAddScoreTeamB?.Invoke();
                }
            }
        }

        public void EndGame()
        {
            IsGameOver = true;
        }
    }
}
