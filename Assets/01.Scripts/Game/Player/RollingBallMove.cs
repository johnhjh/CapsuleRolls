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
        private PlayerMovement playerMovement;
        private Rigidbody ballRigidbody;
        public float ballMoveSpeed = 10f;
        public float ballForce = 10f;
        public float playerMoveSpeed = 5f;
        public float playerRotateSpeed = 80.0f;

        private void Awake()
        {
            playerTransform = transform.GetChild(0).GetComponent<Transform>();
            playerInput = playerTransform.GetComponent<PlayerInput>();
            playerMovement = playerTransform.GetComponent<PlayerMovement>();
            ballRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            playerTransform.Rotate(Vector3.up * playerRotateSpeed * Time.deltaTime * playerInput.rotate);
            Vector3 moveDir = (playerTransform.right * playerInput.horizontal) + (playerTransform.forward * playerInput.vertical);
            if (playerInput.GetInputMovePower() > 1f)
                moveDir = moveDir.normalized;

            if (!playerMovement.IsLanded)
                playerTransform.Translate(moveDir * playerMoveSpeed * Time.deltaTime, Space.Self);
            else
                ballRigidbody.velocity += moveDir * ballMoveSpeed * Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "RollingBall")
            {
                Debug.Log("Hello");
                collision.collider.transform.parent.GetComponent<Rigidbody>().AddForce(ballRigidbody.velocity * ballForce, ForceMode.Impulse);
            }
        }
    }
}
