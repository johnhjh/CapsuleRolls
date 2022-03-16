using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Game.Player;

namespace Capsule.Game.RollTheBall
{
    public class RollingBallRotate : MonoBehaviour
    {
        public float radius = 1.5f;
        public float rotateSpeed = 20f;
        private PlayerInput playerInput;
        private PlayerRollTheBallMove playerMovement;
        private Transform playerTransform;
        private Rigidbody ballRigidbody;
        private Vector3 savedDirection = Vector3.zero;
        public Vector3 SavedDirection
        {
            set 
            {
                if (savedDirection == Vector3.zero)
                    savedDirection = value;
            }
        }

        private void Start()
        {
            playerTransform = transform.parent.GetChild(0).GetComponent<Transform>();
            playerInput = playerTransform.GetComponent<PlayerInput>();
            playerMovement = playerTransform.GetComponent<PlayerRollTheBallMove>();
            ballRigidbody = transform.parent.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 currentDirection;
            if (playerMovement.IsLanded)
            {
                Vector3 direction = (-playerTransform.forward * playerInput.horizontal +
                    playerTransform.right * playerInput.vertical);
                direction = direction.normalized;
                if (playerInput.GetInputMovePower() <= 0.2f)
                    direction = savedDirection;
                savedDirection = direction;
                currentDirection = direction;
            }
            else
                currentDirection = savedDirection;
            currentDirection *= Time.deltaTime * rotateSpeed / radius;
            currentDirection *= ballRigidbody.velocity.magnitude;
            if (ballRigidbody.velocity.magnitude < 0.1f)
                savedDirection = Vector3.zero;
            transform.Rotate(currentDirection, Space.World);
        }
    }
}

