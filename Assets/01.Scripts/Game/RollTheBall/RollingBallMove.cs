using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Game.Player;
using Capsule.Audio;

namespace Capsule.Game.RollTheBall
{
    public class RollingBallMove : MonoBehaviour
    {
        private Transform playerTransform;
        private PlayerInput playerInput;
        private PlayerRollTheBallMove playerMovement;
        private Rigidbody ballRigidbody;
        public float ballMoveSpeed = 11f;
        public float MAX_BALL_SPEED = 100f;
        public float ballForce = 10f;
        public float playerMoveSpeed = 5f;
        public float playerRotateSpeed = 80.0f;
        public bool isMine = true;
        private bool isDead = false;
        public bool IsDead { get { return isDead; } }        

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
            if (collision.collider.CompareTag("RollingBall"))
            {
                collision.collider.transform.GetComponent<RollingBallRotate>().SavedDirection = 
                    playerTransform.InverseTransformDirection(ballRigidbody.velocity);
                collision.collider.transform.parent.GetComponent<Rigidbody>().AddForce(
                    ballRigidbody.velocity * ballForce, ForceMode.Impulse);
            }
        }
    }
}
