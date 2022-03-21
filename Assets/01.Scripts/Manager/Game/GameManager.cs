using Capsule.Audio;
using Capsule.Entity;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Capsule.Game
{
    public class GameNameData
    {
        public readonly string NAME_GAME_OBJECTS = "GameObjects";
        public readonly string NAEM_ROLLING_BALL = "RollingBall";
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

        public GameTagData tagData = new GameTagData();
        public GameAnimData animData = new GameAnimData();
        public GameNameData nameData = new GameNameData();

        public bool IsGameOver { get; private set; }
        private int teamScoreA = 0;
        private int teamScoreB = 0;

        public event Action OnAddScoreTeamA;
        public event Action OnAddScoreTeamB;

        private void Awake()
        {
            if (gameMgr == null)
                gameMgr = this;
            else if (gameMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            // 플레이어 생성 위치 들어갈 자리
            BGMManager.Instance.ChangeBGM(BGMType.ARCADE);
            SFXManager.Instance.PlayOneShot(Announcements.READY);
            SFXManager.Instance.PlaySFX(Announcements.GO, 2f);
            currentGameData = DataManager.Instance.CurrentGameData;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 게임 종료 들어갈 자리
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
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

        public void AddScore(bool isTeamA, int newScore)
        {
            if (!IsGameOver)
            {
                if (isTeamA)
                {
                    if (OnAddScoreTeamA != null)
                        OnAddScoreTeamA();
                    teamScoreA += newScore;
                }
                else
                {
                    if (OnAddScoreTeamB != null)
                        OnAddScoreTeamB();
                    teamScoreB += newScore;
                }
            }
        }

        public void EndGame()
        {
            IsGameOver = true;
        }
    }
}
