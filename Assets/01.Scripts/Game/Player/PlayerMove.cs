﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.Player
{
    public class PlayerMove : MonoBehaviour
    {
        protected Animator playerAnimator;
        protected Rigidbody playerRigidbody;
        protected CapsuleCollider playerCollider;
        protected PlayerInput playerInput;
        protected RagdollController ragdollController;

        private float timeBet = 0f;
        public float turnAnimateTime = 0.3f;
        protected bool isMovingByInput = true;
        public bool isMine = true;

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
            if (!isMine) return;
            ragdollController.OnChangeRagdoll += () => {
                Camera.main.GetComponent<CameraFollow>().targetTransform = ragdollController.spine.transform;
                Camera.main.GetComponent<CameraFollow>().camView = CameraView.QUATER;
            };
        }

        protected virtual void Update()
        {
            if (!isMine) return;
            if (isMovingByInput)
            {
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_HORIZONTAL, playerInput.horizontal);
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_VERTICAL, playerInput.vertical);
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_MOVE_SPEED, Mathf.Clamp(playerInput.GetInputMovePower(), 0f, 1f));
            }
            if (playerInput.rotate != 0f)
            {
                playerAnimator.SetBool(GameManager.Instance.animData.HASH_IS_TURNING, true);
                if (playerInput.rotate < 0)
                    playerAnimator.SetInteger(GameManager.Instance.animData.HASH_ROTATE, -1);
                else if (playerInput.rotate > 0)
                    playerAnimator.SetInteger(GameManager.Instance.animData.HASH_ROTATE, 1);
                timeBet = Time.time + turnAnimateTime;
            }
            if (Time.time > timeBet)
            {
                playerAnimator.SetInteger(GameManager.Instance.animData.HASH_ROTATE, 0);
                playerAnimator.SetBool(GameManager.Instance.animData.HASH_IS_TURNING, false);
            }
        }
    }
}
