using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class RollingBallRotate : MonoBehaviour
    {
        public bool isTeamA = false;
        public float radius = 1.5f;
        public float ballRotateSpeed = 40f;

        private Rigidbody ballRigidbody = null;
        private Transform ballParent;
        public Transform BallParent
        {
            get { return ballParent; }
            set
            {
                ballParent = value;
                transform.parent = value;
                if (value.TryGetComponent(out Rigidbody rigidbody))
                {
                    ballRigidbody = rigidbody;
                    rigidbody.freezeRotation = true;
                }
                else
                    ballRigidbody = null;
            }
        }
        /*
        private PlayerInput playerInput;
        private PlayerRollTheBallMove playerMovement;
        private Transform playerTransform;
        private Vector3 savedDirection = Vector3.zero;
        public Vector3 SavedDirection
        {
            set 
            {
                if (savedDirection == Vector3.zero)
                    savedDirection = value;
            }
        }
        */

        private void Start()
        {
            //playerTransform = transform.parent.GetChild(0).GetComponent<Transform>();
            //playerInput = playerTransform.GetComponent<PlayerInput>();
            //playerMovement = playerTransform.GetComponent<PlayerRollTheBallMove>();
            BallParent = transform.parent;
        }

        private void FixedUpdate()
        {
            if (ballRigidbody == null) return;
            RotateByVelocity();
        }

        private void RotateByVelocity()
        {
            Vector3 ballDirection = Time.deltaTime * ballRotateSpeed / radius * ballRigidbody.velocity;
            ballDirection = -Vector3.forward * ballDirection.x + Vector3.right * ballDirection.z;
            transform.Rotate(ballDirection, Space.World);
        }
        /*
        private void PastRotate()
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
            currentDirection *= Time.deltaTime * ballRotateSpeed / radius;
            currentDirection *= ballRigidbody.velocity.magnitude;
            if (ballRigidbody.velocity.magnitude < 0.1f)
                savedDirection = Vector3.zero;
            transform.Rotate(currentDirection, Space.World);
        }
        */
    }
}

