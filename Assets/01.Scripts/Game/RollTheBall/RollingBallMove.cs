using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Audio;
using Capsule.Game.Effect;
using Capsule.Game.Player;

namespace Capsule.Game.RollTheBall
{
    public class RollingBallMove : MonoBehaviour
    {
        private Transform playerTransform;
        private PlayerInput playerInput;
        private PlayerRollTheBallMove playerMovement;
        private Rigidbody ballRigidbody;
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
            ballRigidbody = GetComponent<Rigidbody>();

            transform.GetChild(2).GetComponent<RagdollController>().OnChangeRagdoll += () => {
                isDead = true;
                ballRigidbody.freezeRotation = false;
            };
        }

        void Update()
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
                if (playerInput.GetInputMovePower() > 0.2f)
                    SFXManager.Instance.PlaySFX(GameSFX.MOVE, Random.Range(0.3f, 0.4f));
                if (ballRigidbody.velocity.sqrMagnitude <= MAX_BALL_SPEED)
                    ballRigidbody.velocity += ballMoveSpeed * Time.deltaTime * moveDir;

            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
            {
                SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE);
                EffectQueueManager.Instance.ShowCollisionEffect(collision, Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                collision.collider.transform.parent.GetComponent<Rigidbody>().AddForce(
                    ballRigidbody.velocity * ballPushForce, ForceMode.Impulse);
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SWIPER))
            {
                SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE);
                EffectQueueManager.Instance.ShowCollisionEffect(collision, Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                ballRigidbody.AddForce(collision.collider.GetComponent<Rigidbody>().velocity);
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER))
            {
                IsDead = true;
                SFXManager.Instance.StopSFX();
                SFXManager.Instance.PlayOneShot(GameSFX.POP, popVolume);
                SFXManager.Instance.PlayOneShot(GameSFX.FALLING);

                Transform ballTransform = transform.GetChild(1);
                EffectQueueManager.Instance.ShowExplosionEffect(ballTransform.position);
                ballTransform.gameObject.SetActive(false);

                playerTransform.GetComponent<Animator>().SetTrigger(
                    GameManager.Instance.animData.HASH_TRIG_FALLING);
                playerTransform.GetComponent<CapsuleCollider>().isTrigger = false;
                Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();
                playerRigidbody.isKinematic = false;
                playerRigidbody.mass = 1f;
                playerRigidbody.AddForce(explodePower * Vector3.up, ForceMode.Impulse);
            }
        }
    }
}
