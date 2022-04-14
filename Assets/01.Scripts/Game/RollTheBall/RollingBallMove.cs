using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.Effect;
using Capsule.Game.Player;
using Capsule.Game.UI;
using Capsule.Game.Util;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    [RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
    public class RollingBallMove : MonoBehaviour
    {
        // Components
        private Transform playerTransform;
        private AudioSource playerAudioSource;
        private AudioSource ballAudioSource;
        private Rigidbody ballRigidbody;
        private PlayerInput playerInput;
        private PlayerRollTheBallMove playerMovement;
        public float ballMoveSpeed = 11f;
        public float MAX_BALL_SPEED = 500f;
        private readonly float ballPushForce = 30f;
        [HideInInspector]
        public float playerMoveSpeed = 5f;
        [HideInInspector]
        public float playerRotateSpeed = 60.0f;
        private readonly float explodePower = 10f;
        private readonly float popVolume = 7f;
        public bool isMine = true;
        private bool isDead = false;
        public bool IsDead
        {
            get { return isDead; }
            set
            {
                isDead = value;
                playerMovement.IsDead = value;
                playerInput.IsDead = value;
                ballRigidbody.freezeRotation = !value;
            }
        }

        private void Awake()
        {
            playerTransform = transform.GetChild(0).GetComponent<Transform>();
            playerInput = playerTransform.GetComponent<PlayerInput>();
            playerMovement = playerTransform.GetComponent<PlayerRollTheBallMove>();
            playerAudioSource = playerTransform.GetComponent<AudioSource>();
            ballRigidbody = GetComponent<Rigidbody>();
            ballAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (!isMine) return;
            if (GameSettingManager.Instance != null)
                GameSettingManager.Instance.ballMove = this;
            playerRotateSpeed = PlayerPrefs.GetFloat("PlayerRotSpeed", 65f);
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.OnPauseGame += PlayerAudioStop;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver += PlayerAudioStop;
                if (GameManager.Instance.CurrentGameData.Mode == GameMode.STAGE)
                {
                    switch (GameManager.Instance.CurrentGameData.Stage)
                    {
                        case GameStage.TUTORIAL_1:
                        case GameStage.STAGE_1:
                            playerInput.usingAction1 = false;
                            playerInput.usingAction2 = false;
                            break;
                        case GameStage.TUTORIAL_2:
                        case GameStage.STAGE_2:
                            playerInput.usingAction1 = true;
                            playerInput.usingAction2 = false;
                            break;
                        default:
                            playerInput.usingAction1 = true;
                            playerInput.usingAction2 = true;
                            break;
                    }
                }
            }
        }

        private void Update()
        {
            if (isDead) return;
            if (GameManager.Instance != null && (!GameManager.Instance.IsGameReady || GameManager.Instance.IsGameOver)) return;
            if (playerMovement.IsAdjusting) return;
            playerTransform.Rotate(playerInput.rotate * playerRotateSpeed * Time.deltaTime * Vector3.up);
            Vector3 moveDir = (playerTransform.right * playerInput.horizontal) +
                (playerTransform.forward * playerInput.vertical);
            if (playerInput.GetInputMovePower() > 1f)
                moveDir = moveDir.normalized;

            if (!playerMovement.IsLanded)
                playerTransform.Translate(playerMoveSpeed * Time.deltaTime * moveDir, Space.World);
            else
            {
                if (playerInput.GetInputMovePower() > 0.2f && ballRigidbody.velocity.sqrMagnitude > 0.2f)
                {
                    if (Time.timeScale != 0f)
                        SFXManager.Instance.PlaySFX(GameSFX.MOVE, playerAudioSource, Random.Range(0.3f, 0.4f));
                }
                if (ballRigidbody.velocity.sqrMagnitude <= MAX_BALL_SPEED)
                    ballRigidbody.velocity += ballMoveSpeed * Time.deltaTime * moveDir;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (GameManager.Instance != null)
            {
                if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
                {
                    GameCameraManager.Instance.CameraShake();
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(300);
                    if (ballAudioSource != null)
                        SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                    EffectQueueManager.Instance.ShowCollisionEffect(collision, Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                    if (collision.collider.transform.parent.TryGetComponent<Rigidbody>(out Rigidbody collRigidbody))
                    {
                        collRigidbody.AddForce(
                            ballRigidbody.velocity * ballPushForce, ForceMode.Impulse);
                    }
                }
                else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SWIPER))
                {
                    GameCameraManager.Instance.CameraShake();
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(500);
                    if (ballAudioSource != null)
                        SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                    EffectQueueManager.Instance.ShowCollisionEffect(collision,
                        Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                    ballRigidbody.AddForce(collision.collider.GetComponent<Rigidbody>().velocity);
                }
                else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER)
                    || collision.collider.CompareTag(GameManager.Instance.tagData.TAG_DEAD_ZONE))
                {
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(1000);
                    if (ballAudioSource != null)
                        SFXManager.Instance.PlayOneShot(GameSFX.POP, ballAudioSource, popVolume);
                    Transform ballTransform = transform.GetChild(2);
                    if (ballTransform.gameObject.activeSelf)
                    {
                        EffectQueueManager.Instance.ShowExplosionEffect(ballTransform.position);
                        ballTransform.gameObject.SetActive(false);
                        if (!isDead && !playerMovement.IsDead && playerMovement.IsLanded)
                        {
                            isDead = true;
                            playerInput.IsDead = true;
                            playerMovement.IsLanded = false;
                            PlayerAudioStop();
                            if (playerAudioSource != null)
                                SFXManager.Instance.PlayOneShot(GameSFX.FALLING, playerAudioSource);
                            playerTransform.GetComponent<Animator>().SetTrigger(GameManager.Instance.animData.HASH_TRIG_FALLING);
                            Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();
                            playerRigidbody.mass = 1f;
                            playerRigidbody.AddForce(explodePower * Vector3.up, ForceMode.Impulse);
                        }
                    }
                }
                else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_GOAL_POST))
                {
                    GameCameraManager.Instance.CameraShake();
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(300);
                    if (ballAudioSource != null)
                        SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                    if (EffectQueueManager.Instance != null)
                        EffectQueueManager.Instance.ShowCollisionEffect(collision,
                        Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));

                    if (GameManager.Instance.CurrentGameData.Mode == GameMode.BOT ||
                        GameManager.Instance.CurrentGameData.Mode == GameMode.MULTI ||
                        GameManager.Instance.CurrentGameData.Mode == GameMode.CUSTOM ||
                        GameManager.Instance.CurrentGameData.Mode == GameMode.RANK)
                        SFXManager.Instance.PlaySFX(Crowds.GROAN);
                }
                else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_PLAYER))
                {
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(100);
                    if (ballAudioSource != null)
                        SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                    if (EffectQueueManager.Instance != null)
                        EffectQueueManager.Instance.ShowCollisionEffect(collision, Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                }
            }
        }

        private void PlayerAudioStop()
        {
            playerAudioSource.Stop();
        }
    }
}
