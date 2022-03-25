using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.Effect;
using Capsule.Game.Player;
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
        public float MAX_BALL_SPEED = 300f;
        public float ballPushForce = 30f;
        public float playerMoveSpeed = 5f;
        public float playerRotateSpeed = 80.0f;
        public float explodePower = 10f;
        public float popVolume = 7f;
        public bool isMine = true;
        private bool isDead = false;
        public bool IsDead
        {
            get { return isDead; }
            private set
            {
                isDead = value;
                transform.GetChild(0).GetComponent<PlayerRollTheBallMove>().IsDead = true;
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

            transform.GetChild(1).GetComponent<RagdollController>().OnChangeRagdoll += () =>
            {
                isDead = true;
                ballRigidbody.freezeRotation = false;
            };
        }

        private void Start()
        {
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

        private void Update()
        {
            if (isDead) return;
            if (!isMine) return;

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
                    //PlayerAudioPlay(SFXManager.Instance.GetAudioClip(GameSFX.MOVE), Random.Range(0.3f, 0.4f));
                    SFXManager.Instance.PlaySFX(GameSFX.MOVE, playerAudioSource, Random.Range(0.3f, 0.4f));
                }
                if (ballRigidbody.velocity.sqrMagnitude <= MAX_BALL_SPEED)
                    ballRigidbody.velocity += ballMoveSpeed * Time.deltaTime * moveDir;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
            {
                GameCameraManager.Instance.CameraShake();
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
                SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                EffectQueueManager.Instance.ShowCollisionEffect(collision,
                    Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                ballRigidbody.AddForce(collision.collider.GetComponent<Rigidbody>().velocity);
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER)
                || collision.collider.CompareTag(GameManager.Instance.tagData.TAG_DEAD_ZONE))
            {
                IsDead = true;
                PlayerAudioStop();
                //PlayerAudioPlayOneShot(SFXManager.Instance.GetAudioClip(GameSFX.POP), popVolume);
                //PlayerAudioPlayOneShot(SFXManager.Instance.GetAudioClip(GameSFX.FALLING));
                SFXManager.Instance.PlayOneShot(GameSFX.POP, playerAudioSource, popVolume);
                SFXManager.Instance.PlayOneShot(GameSFX.FALLING, playerAudioSource);

                Transform ballTransform = transform.GetChild(2);
                EffectQueueManager.Instance.ShowExplosionEffect(ballTransform.position);
                ballTransform.gameObject.SetActive(false);

                playerTransform.GetComponent<Animator>().SetTrigger(GameManager.Instance.animData.HASH_TRIG_FALLING);
                playerTransform.GetComponent<CapsuleCollider>().isTrigger = false;
                Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();
                playerRigidbody.isKinematic = false;
                playerRigidbody.mass = 1f;
                playerRigidbody.AddForce(explodePower * Vector3.up, ForceMode.Impulse);
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_GOAL_POST))
            {
                GameCameraManager.Instance.CameraShake();
                //BallAudioPlayOneShot(SFXManager.Instance.GetAudioClip(GameSFX.BOUNCE));
                SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                SFXManager.Instance.PlaySFX(Crowds.GROAN);
                EffectQueueManager.Instance.ShowCollisionEffect(collision,
                    Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
            }
        }

        private void PlayerAudioStop()
        {
            playerAudioSource.Stop();
        }
        /*
        private void BallAudioPlayOneShot(AudioClip clip)
        {
            if (clip != null)
                ballAudioSource.PlayOneShot(clip);
        }

        private void PlayerAudioPlay(AudioClip clip, float delay)
        {
            if (clip != null)
            {
                if (!playerAudioSource.isPlaying)
                {
                    playerAudioSource.clip = clip;
                    playerAudioSource.PlayDelayed(delay);
                }
            }
        }

        private void PlayerAudioPlayOneShot(AudioClip clip)
        {
            if (clip != null)
                playerAudioSource.PlayOneShot(clip);
        }

        private void PlayerAudioPlayOneShot(AudioClip clip, float volume)
        {
            if (clip != null)
                playerAudioSource.PlayOneShot(clip, volume);
        }
        */
    }
}
