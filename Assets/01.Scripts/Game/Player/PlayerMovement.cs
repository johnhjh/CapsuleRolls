using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Animator playerAnimator;
        private Rigidbody playerRigidbody;
        private CapsuleCollider playerCollider;
        private PlayerInput playerInput;
        private RagdollController ragdollController;
        private Rigidbody ballRigidbody;

        private float timeBet = 0f;
        public float turnAnimateTime = 0.3f;
        public float diveForce = 10f;
        public float jumpForce = 10f;
        private bool isDiving;
        public bool IsTryJumping { get; private set; }
        private bool isLanded = true;
        public bool IsLanded 
        {
            get { return isLanded; }
            private set
            {
                isLanded = value;
                playerCollider.isTrigger = value;
                playerRigidbody.isKinematic = value;
                if (value)
                    playerAnimator.SetTrigger("TrigStopJumping");
                IsTryJumping = !value;
            } 
        }

        private void Awake()
        {
            playerAnimator = GetComponent<Animator>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerCollider = GetComponent<CapsuleCollider>();
            playerInput = GetComponent<PlayerInput>();
            ballRigidbody = transform.parent.GetComponent<Rigidbody>();
        }

        void Start()
        {
            ragdollController = transform.parent.GetChild(2).GetComponent<RagdollController>();
            isLanded = true;
            isDiving = false;
        }

        void Update()
        {
            playerAnimator.SetFloat("Horizontal", playerInput.horizontal);
            playerAnimator.SetFloat("Vertical", playerInput.vertical);
            playerAnimator.SetFloat("MoveSpeed", Mathf.Clamp(playerInput.GetInputMovePower(), 0f, 1f));
            if (playerInput.rotate != 0f)
            {
                playerAnimator.SetBool("IsTurning", true);
                if (playerInput.rotate < 0)
                    playerAnimator.SetInteger("Rotate", -1);
                else if (playerInput.rotate > 0)
                    playerAnimator.SetInteger("Rotate", 1);
                timeBet = Time.time + turnAnimateTime;
            }
            if (Time.time > timeBet)
            {
                playerAnimator.SetInteger("Rotate", 0);
                playerAnimator.SetBool("IsTurning", false);
            }
            if (playerInput.GetInputMovePower() > 0f)
                playerAnimator.speed = 1f + ballRigidbody.velocity.magnitude / 10f;
            else
                playerAnimator.speed = 1f;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerAnimator.SetTrigger("TrigVictory");
                playerAnimator.SetInteger("VictoryAnim", Random.Range(0, 3));
            }
            if (Input.GetMouseButtonDown(0) && !IsTryJumping)
            {
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

        private IEnumerator Jumping()
        {
            yield return new WaitForSeconds(1.0f);
            IsTryJumping = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                RagdollController otherRagdoll = other.transform.parent.GetChild(2).GetComponent<RagdollController>();
                otherRagdoll.forceVector = playerRigidbody.velocity;
                otherRagdoll.ChangeRagdoll(true);
                ragdollController.forceVector = playerRigidbody.velocity;
                ragdollController.ChangeRagdoll(true);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Stage"))
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

