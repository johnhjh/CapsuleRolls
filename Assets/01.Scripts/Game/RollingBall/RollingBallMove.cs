using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Game.Player;

namespace Capsule.Game.RollingBall
{
    public class RollingBallMove : MonoBehaviour
    {
        private Transform playerTransform;
        private PlayerInput playerInput;
        private PlayerRollingBallMovement playerMovement;
        private Rigidbody ballRigidbody;
        public float ballMoveSpeed = 11f;
        public float ballForce = 10f;
        public float playerMoveSpeed = 5f;
        public float playerRotateSpeed = 80.0f;
        public bool isMine = true;

        private void Awake()
        {
            playerTransform = transform.GetChild(0).GetComponent<Transform>();
            playerInput = playerTransform.GetComponent<PlayerInput>();
            playerMovement = playerTransform.GetComponent<PlayerRollingBallMovement>();
            ballRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (!isMine) return;
            playerTransform.Rotate(playerInput.rotate * playerRotateSpeed * Time.deltaTime * Vector3.up);
            Vector3 moveDir = (playerTransform.right * playerInput.horizontal) + (playerTransform.forward * playerInput.vertical);
            if (playerInput.GetInputMovePower() > 1f)
                moveDir = moveDir.normalized;

            if (!playerMovement.IsLanded)
                playerTransform.Translate(playerMoveSpeed * Time.deltaTime * moveDir, Space.Self);
            else
                ballRigidbody.velocity += ballMoveSpeed * Time.deltaTime * moveDir;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("RollingBall"))
            {
                collision.collider.transform.GetComponent<RollingBallRotate>().SavedDirection = playerTransform.InverseTransformDirection(ballRigidbody.velocity);
                collision.collider.transform.parent.GetComponent<Rigidbody>().AddForce(ballRigidbody.velocity * ballForce, ForceMode.Impulse);
            }
        }
    }
}
