using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Game.Base;

namespace Capsule.Game.Player
{
    public class PlayerInput : MoveInput
    {
        void Update()
        {
            if (GameManager.Instance.IsGameOver)
            {
                horizontal = 0f;
                vertical = 0f;
                rotate = 0f;
                return;
            }
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            rotate = Input.GetAxis("Mouse X");
        }

        public float GetInputMovePower()
        {
            return Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        }
    }
}
