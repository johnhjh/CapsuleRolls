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
                transform.gameObject.layer = value.gameObject.layer;
                if (value.TryGetComponent(out Rigidbody rigidbody))
                {
                    ballRigidbody = rigidbody;
                    rigidbody.freezeRotation = true;
                }
                else
                    ballRigidbody = null;
            }
        }

        private void Start()
        {
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
    }
}

