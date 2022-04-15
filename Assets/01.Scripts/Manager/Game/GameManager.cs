using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.Enemy;
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
                            Kind = GameKind.GOAL_IN,
                        };
                }
                return currentGameData;
            }
        }

        public GameTagData tagData = new GameTagData();
        public GameAnimData animData = new GameAnimData();
        public GameNameData nameData = new GameNameData();

        private bool isGameOver = false;
        public bool IsGameOver
        {
            get
            {
                return isGameOver;
            }
            private set
            {
                isGameOver = value;
                if (value && OnGameOver != null)
                    OnGameOver();
            }
        }

        private bool isGameReady = false;
        public bool IsGameReady { get { return isGameReady; } }

        private int teamScoreA = 0;
        public int TeamScoreA
        {
            get { return teamScoreA; }
            private set { teamScoreA = value; }
        }

        private int teamScoreB = 0;
        public int TeamScoreB
        {
            get { return teamScoreB; }
            private set { teamScoreB = value; }
        }

        private int arcadeScore = 0;
        public int ArcadeScore
        {
            get { return arcadeScore; }
            private set { arcadeScore = value; }
        }

        private int enemyCount = 0;
        public int EnemyCount
        {
            get { return enemyCount; }
            private set { enemyCount = value; }
        }

        private int currentWave = 1;
        public int CurrentWave
        {
            get { return currentWave; }
            private set { currentWave = value; }
        }

        public event Action OnPlayerDeath;
        public event Action OnAddScoreTeamA;
        public event Action OnAddScoreTeamB;
        public event Action OnAddArcadeScore;
        public event Action OnAddEnemyCount;
        public event Action OnArcadeFinish;
        public event Action OnStageClear;
        public event Action OnStageFailure;
        public event Action OnStartGame;
        public event Action OnGameOver;
        public event Action OnTimeEnded;
        public event Action OnWaveClear;

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
            isGameReady = false;
            Cursor.visible = PlayerPrefs.GetInt("UsingCursor", 1) == 1;
            currentGameData = DataManager.Instance.CurrentGameData;
            if (CurrentGameData != null)
            {
                if (CurrentGameData.Mode == GameMode.ARCADE)
                {
                    BGMManager.Instance.ChangeBGM(BGMType.ARCADE);
                    OnAddArcadeScore += GameUIManager.Instance.UpdateScoreText;
                    OnAddEnemyCount += GameUIManager.Instance.UpdateRemainedEnemeyText;
                    OnAddEnemyCount += CheckWaveCleared;
                }
                else if (CurrentGameData.Mode == GameMode.STAGE)
                    BGMManager.Instance.ChangeBGM(BGMType.STAGE);
                else if (CurrentGameData.Mode == GameMode.PRACTICE)
                    BGMManager.Instance.ChangeBGM(BGMType.MAIN);
                else if (CurrentGameData.Mode == GameMode.BOT)
                    BGMManager.Instance.ChangeBGM(BGMType.MAIN);
            }
            SFXManager.Instance.PlaySFX(Announcements.READY, 1f);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.IsLoading = false;
            OnStartGame?.Invoke();
            StartCoroutine(SetGameReady());
        }

        private IEnumerator SetGameReady()
        {
            yield return new WaitForSeconds(3.0f);
            SFXManager.Instance.PlayOneShot(Announcements.GO);
            isGameReady = true;
            if (GameTutorialManager.Instance != null)
            {
                switch (CurrentGameData.Mode)
                {
                    case GameMode.ARCADE:
                        bool isFirstPlayArcade = PlayerPrefs.GetInt("ArcadePlayCount", 0) == 0;
                        if (isFirstPlayArcade)
                        {
                            GameTutorialManager.Instance.CurrentTutorial = TutorialType.MODE;
                            GameTutorialManager.Instance.IsTutorialPopup = true;
                        }
                        break;
                    case GameMode.STAGE:
                        bool isFirstPlayStage = PlayerPrefs.GetInt("StagePlayCount", 0) == 0;
                        if (isFirstPlayStage)
                        {
                            GameTutorialManager.Instance.CurrentTutorial = TutorialType.MODE;
                            GameTutorialManager.Instance.IsTutorialPopup = true;
                        }
                        break;
                    case GameMode.PRACTICE:
                        bool isFirstPlayPractice = PlayerPrefs.GetInt("PracticePlayCount", 0) == 0;
                        if (isFirstPlayPractice)
                        {
                            GameTutorialManager.Instance.CurrentTutorial = TutorialType.MODE;
                            GameTutorialManager.Instance.IsTutorialPopup = true;
                        }
                        break;
                    case GameMode.BOT:
                        bool isFirstPlayBot = PlayerPrefs.GetInt("BotPlayCount", 0) == 0;
                        if (isFirstPlayBot)
                        {
                            GameTutorialManager.Instance.CurrentTutorial = TutorialType.MODE;
                            GameTutorialManager.Instance.IsTutorialPopup = true;
                        }
                        break;
                }
            }
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
                if (GameTutorialManager.Instance != null)
                {
                    if (GameTutorialManager.Instance.IsTutorialPopup)
                    {
                        GameTutorialManager.Instance.IsTutorialPopup = false;
                        return;
                    }
                }
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
                    ArcadeFinish();
                    break;
                case GameMode.STAGE:
                    StageFailure();
                    break;
                case GameMode.BOT:
                    // 점수 계산 후 승자를 가리는 자리
                    break;
            }
        }

        public void CheckWaveCleared()
        {
            if (EnemyCount == 0)
                WaveCleared();
        }

        public void WaveCleared()
        {
            OnWaveClear?.Invoke();
            CurrentWave++;
            SFXManager.Instance.PlayOneShot(Crowds.APPLOUSE);
            SFXManager.Instance.PlayOneShot(Announcements.CLEAR);
            GameUIManager.Instance.UpdateWaveText();
            EnemySpawnManager.Instance.NextSpkieRollers(CurrentWave);
            StartCoroutine(NextWave());
        }

        private IEnumerator NextWave()
        {
            GameUIManager.Instance.AddTime(5);
            yield return new WaitForSeconds(1f);
            if (isGameOver)
                yield break;
            SFXManager.Instance.PlayOneShot(Announcements.READY);
            yield return new WaitForSeconds(3f);
            if (isGameOver)
                yield break;
            SFXManager.Instance.PlayOneShot(Announcements.GO);
            if (EnemySpawnManager.Instance != null)
                EnemySpawnManager.Instance.SpawnWave(CurrentWave);
        }

        private int CalculateArcadeCoin()
        {
            return GameUIManager.Instance.CurrentPassedTime +
                Mathf.RoundToInt(ArcadeScore / 10);
        }

        private int CalculateArcadeExp()
        {
            return Mathf.RoundToInt(
                (GameUIManager.Instance.CurrentPassedTime +
                ArcadeScore / 10) / 10);
        }

        public void ArcadeFinish()
        {
            IsGameOver = true;
            OnArcadeFinish?.Invoke();
            DataManager.Instance.CurrentPlayerGameData.PlayerArcadePlayed();
            DataManager.Instance.CurrentPlayerData.EarnCoin(CalculateArcadeCoin());
            DataManager.Instance.CurrentPlayerData.AddExp(CalculateArcadeExp());
            StartCoroutine(PopupArcadeFinishUI());
        }

        public void StageClear()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            SFXManager.Instance.PlayOneShot(Crowds.APPLOUSE);
            SFXManager.Instance.PlayOneShot(Announcements.CLEAR);
            OnStageClear?.Invoke();
            foreach (RewardData reward in DataManager.Instance.GameStageDatas[(int)currentGameData.Stage].rewards)
            {
                if (reward.onlyOnce)
                {
                    //Debug.Log("원래 처음 한번만 주는거라구~");
                    if (DataManager.Instance.CurrentPlayerStageClearData
                        .ClearData[(int)currentGameData.Stage])
                        continue;
                }
                if (GameUIManager.Instance != null)
                    GameUIManager.Instance.SetRewardData(reward);
                switch (reward.kind)
                {
                    case RewardKind.COIN:
                        DataManager.Instance.CurrentPlayerData.EarnCoin(reward.amount);
                        //Debug.Log("코인 " + reward.amount.ToString() + " 얻음");
                        break;
                    case RewardKind.EXP:
                        DataManager.Instance.CurrentPlayerData.AddExp(reward.amount);
                        //Debug.Log("경험치 " + reward.amount.ToString() + " 얻음");
                        break;
                    case RewardKind.CUSTOMIZING_BODY:
                        DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(reward.amount, (int)CustomizingType.BODY);
                        DataManager.Instance.BodyOpenData.Add((CustomizingBody)reward.amount);
                        //Debug.Log("바디 " + reward.amount.ToString() + " 얻음");
                        break;
                    case RewardKind.CUSTOMIZING_HEAD:
                        DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(reward.amount, (int)CustomizingType.HEAD);
                        DataManager.Instance.HeadOpenData.Add((CustomizingHead)reward.amount);
                        //Debug.Log("헤드 " + reward.amount.ToString() + " 얻음");
                        break;
                    case RewardKind.CUSTOMIZING_FACE:
                        DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(reward.amount, (int)CustomizingType.FACE);
                        DataManager.Instance.FaceOpenData.Add((CustomizingFace)reward.amount);
                        //Debug.Log("얼굴 " + reward.amount.ToString() + " 얻음");
                        break;
                    case RewardKind.CUSTOMIZING_GLOVE:
                        DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(reward.amount, (int)CustomizingType.GLOVE);
                        DataManager.Instance.GloveOpenData.Add((CustomizingGlove)reward.amount);
                        //Debug.Log("장갑 " + reward.amount.ToString() + " 얻음");
                        break;
                    case RewardKind.CUSTOMIZING_CLOTH:
                        DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(reward.amount, (int)CustomizingType.CLOTH);
                        DataManager.Instance.ClothOpenData.Add((CustomizingCloth)reward.amount);
                        //Debug.Log("옷 " + reward.amount.ToString() + " 얻음");
                        break;
                    case RewardKind.GAME_MODE:
                        if (reward.amount == 0)
                            DataManager.Instance.OpenArcadeMode();
                        else if (reward.amount == 1)
                            DataManager.Instance.OpenPracticeMode();
                        else if (reward.amount == 2)
                            DataManager.Instance.OpenBotMode();
                        break;
                }
            }
            DataManager.Instance.CurrentPlayerStageClearData.StageClear(CurrentGameData.Stage);
            DataManager.Instance.CurrentPlayerGameData.PlayerStagePlayed((int)CurrentGameData.Stage, true);
            StartCoroutine(PopupClearUI());
        }

        public void StageFailure()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            OnStageFailure?.Invoke();
            DataManager.Instance.CurrentPlayerData.AddExp((int)CurrentGameData.Stage);
            DataManager.Instance.CurrentPlayerGameData.PlayerStagePlayed((int)CurrentGameData.Stage, false);
            StartCoroutine(PopupFailureUI());
        }

        private IEnumerator PopupArcadeFinishUI()
        {
            yield return new WaitForSeconds(3.0f);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.OnArcadeFinished();
            if (DataManager.Instance.CurrentPlayerGameData.PlayerScored(arcadeScore))
            {
                SFXManager.Instance.PlayOneShot(Announcements.NEW_RECORD);
                if (GameUIManager.Instance != null)
                    GameUIManager.Instance.OnArcadeNewRecord();
            }
        }

        private IEnumerator PopupClearUI()
        {
            if (GameUIManager.Instance != null)
                yield return StartCoroutine(GameUIManager.Instance.ClearTextEffect());
            else
                yield return new WaitForSeconds(2.0f);
            SFXManager.Instance.PlayOneShot(Announcements.CONGRAT);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.OnStageClear();
        }

        private IEnumerator PopupFailureUI()
        {
            yield return new WaitForSeconds(2.0f);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.OnStageFailure();
        }

        public void AddEnemyCount(int count)
        {
            EnemyCount += count;
            OnAddEnemyCount?.Invoke();
        }

        public void AddScore(int newScore)
        {
            arcadeScore += newScore;
            OnAddArcadeScore?.Invoke();
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

        public void PlayerDied()
        {
            OnPlayerDeath?.Invoke();
        }

        public void EndGame()
        {
            IsGameOver = true;
        }
    }
}
