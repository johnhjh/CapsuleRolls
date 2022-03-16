using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Game.Base;

namespace Capsule.Game.Player
{
    public class PlayerInput : MoveInput
    {
        private readonly string AXIS_NAME_HORIZONTAL = "Horizontal";
        private readonly string AXIS_NAME_VERTICAL = "Vertical";
        private readonly string AXIS_NAME_MOUSE_X = "Mouse X";

        public bool isMine = true;
        public bool Action1 { get; private set; }
        public bool Action2 { get; private set; }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver)
            {
                horizontal = 0f;
                vertical = 0f;
                rotate = 0f;
                Action1 = false;
                Action2 = false;
                return;
            }
            if (!isMine) return;
            horizontal = Input.GetAxis(AXIS_NAME_HORIZONTAL);
            vertical = Input.GetAxis(AXIS_NAME_VERTICAL);
            rotate = Input.GetAxis(AXIS_NAME_MOUSE_X);
            Action1 = Input.GetMouseButtonDown(0);
            Action2 = Input.GetMouseButtonDown(1);
        }

        public float GetInputMovePower()
        {
            return Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        }
    }
}
