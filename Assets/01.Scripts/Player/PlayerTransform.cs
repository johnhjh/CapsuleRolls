using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Player
{
    public class PlayerTransform : MonoBehaviour
    {
        private static PlayerTransform playerTransform;
        public static PlayerTransform Instance
        {
            get
            {
                if (playerTransform == null)
                    playerTransform = FindObjectOfType<PlayerTransform>();
                return playerTransform;
            }
        }

        private void Awake()
        {
            if (playerTransform == null)
            {
                playerTransform = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (playerTransform != this)
                Destroy(this.gameObject);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.localRotation = rotation;
        }

        public void Rotate(Vector3 vector, float amount)
        {
            transform.Rotate(vector * amount);
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }

    }
}
