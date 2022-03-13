using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Player.Game
{
    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector]
        public float h;
        [HideInInspector]
        public float v;

        void Start()
        {

        }

        void Update()
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }
    }
}
