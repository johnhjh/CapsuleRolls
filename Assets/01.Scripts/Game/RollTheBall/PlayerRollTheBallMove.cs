using System.Collections;
using UnityEngine;
using Capsule.Game.Player;

namespace Capsule.Game.RollTheBall
{
    public class PlayerRollTheBallMove : PlayerMove
    {
        private Rigidbody ballRigidbody;
        public float jumpForce = 300f;
        public float diveForce = 700f;
        private bool isDiving;
        private bool jumpEnabled = true;
        public bool IsTryJumping { get; private set; }
        private bool isLanded = true;
        public bool IsLanded
        {
            get { return isLanded; }
            private set
            {
                if (value)
                {
                    playerAnimator.ResetTrigger("TrigJump");
                    playerAnimator.SetTrigger("TrigStopJumping");
                    StartCoroutine(EnableJump());
                }
                isLanded = value;
                playerCollider.isTrigger = value;
                playerRigidbody.isKinematic = value;
                IsTryJumping = !value;
            }
        }

        protected override void Start()
        {
            base.Start();
            base.isMovingByInput = false;
            isLanded = true;
            isDiving = false;
            ballRigidbody = transform.parent.GetComponent<Rigidbody>();
        }

        protected override void Update()
        {
            Vector3 velocity = transform.InverseTransformDirection(ballRigidbody.velocity);
            float magnitude = Mathf.Clamp(velocity.magnitude / 10f, 0f, 1f);
            playerAnimator.SetFloat("Horizontal", velocity.x);
            playerAnimator.SetFloat("Vertical", velocity.z);
            playerAnimator.SetFloat("MoveSpeed", magnitude);
            base.Update();
            if (!isMine) return;
            if (playerInput.GetInputMovePower() > 0f)
                playerAnimator.speed = 1f + ballRigidbody.velocity.magnitude / 10f;
            else
                playerAnimator.speed = 1f;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerAnimator.SetTrigger("TrigVictory");
                playerAnimator.SetInteger("VictoryAnim", Random.Range(0, 3));
            }
            if (Input.GetMouseButtonDown(0) && !IsTryJumping && jumpEnabled)
            {
                jumpEnabled = false;
                IsLanded = false;
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                playerAnimator.SetTrigger("TrigJump");
                StartCoroutine(Jumping());
            }
            if (Input.GetMouseButtonDown(1) && IsLanded)
            {
                IsLanded = false;
                playerRigidbody.AddForce(transform.forward * diveForce, ForceMode.Impulse);
                playerAnimator.SetTrigger("TrigDive");
                isDiving = true;
            }
        }

        private IEnumerator EnableJump()
        {
            yield return new WaitForSeconds(0.05f);
            jumpEnabled = true;
        }

        private IEnumerator Jumping()
        {
            yield return new WaitForSeconds(1.0f);
            IsTryJumping = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Swiper"))
            {
                ragdollController.ChangeRagdoll(true);
            }
            else if (other.CompareTag("Player"))
            {
                RagdollController otherRagdoll = other.transform.parent.GetChild(2).GetComponent<RagdollController>();
                otherRagdoll.forceVector = playerRigidbody.velocity;
                otherRagdoll.ChangeRagdoll(true);
                ragdollController.forceVector = playerRigidbody.velocity;
                ragdollController.ChangeRagdoll(true);
            }
            else if (other.CompareTag("RollingBall"))
            {
                if (other.transform != transform.parent.GetChild(1))
                    ragdollController.ChangeRagdoll(true);
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Stage") || collision.collider.CompareTag("Swiper"))
                ragdollController.ChangeRagdoll(true);
            else if (collision.collider.CompareTag("RollingBall") && !IsTryJumping)
            {
                if (collision.collider.transform == transform.parent.GetChild(1) && !isDiving)
                    IsLanded = true;
                else
                    ragdollController.ChangeRagdoll(true);
            }
        }
    }
}