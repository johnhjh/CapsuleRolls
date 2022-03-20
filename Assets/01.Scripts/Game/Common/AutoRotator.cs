using UnityEngine;

namespace Capsule.Game
{
    public class AutoRotator : MonoBehaviour
    {
        public float rotationSpeed = 20f;
        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
        }
    }
}

