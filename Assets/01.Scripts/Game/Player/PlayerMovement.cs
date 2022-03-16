using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        protected Animator playerAnimator;
        protected Rigidbody playerRigidbody;
        protected CapsuleCollider playerCollider;
        protected PlayerInput playerInput;
        protected RagdollController ragdollController;

        private float timeBet = 0f;
        public float turnAnimateTime = 0.3f;
        protected bool isMovingByInput = true;

        private void Awake()
        {
            playerAnimator = GetComponent<Animator>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerCollider = GetComponent<CapsuleCollider>();
            playerInput = GetComponent<PlayerInput>();
        }

        protected virtual void Start()
        {
            ragdollController = transform.parent.GetChild(2).GetComponent<RagdollController>();
        }

        protected virtual void Update()
        {
            if (isMovingByInput)
            {
                playerAnimator.SetFloat("Horizontal", playerInput.horizontal);
                playerAnimator.SetFloat("Vertical", playerInput.vertical);
                playerAnimator.SetFloat("MoveSpeed", Mathf.Clamp(playerInput.GetInputMovePower(), 0f, 1f));
            }
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
        }
    }
}

