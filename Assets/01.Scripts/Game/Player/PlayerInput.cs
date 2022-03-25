using Capsule.Game.Base;
using Capsule.Game.UI;
using UnityEngine;

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
        //[HideInInspector]
        public bool usingActiion1 = true;
        //[HideInInspector]
        public bool usingAction2 = true;

        private JoyStickCtrl joyStick = null;

        private void Start()
        {
            if (isMine)
            {
                joyStick = GameObject.FindObjectOfType<JoyStickCtrl>();
                if (joyStick != null)
                    joyStick.InputPlayer = this;
            }
        }

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
            if (GameUIManager.Instance != null && GameUIManager.Instance.CheckUIActive())
            {
                horizontal = 0f;
                vertical = 0f;
                rotate = 0f;
                Action1 = false;
                Action2 = false;
                return;
            }
            if (joyStick != null && joyStick.JoyStickPressed)
            {
                horizontal = joyStick.horizontal;
                vertical = joyStick.vertical;
                rotate = joyStick.rotate;
            }
            else
            {
                if (Application.platform != RuntimePlatform.Android &&
                    Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    horizontal = Input.GetAxis(AXIS_NAME_HORIZONTAL);
                    vertical = Input.GetAxis(AXIS_NAME_VERTICAL);
                    rotate = Input.GetAxis(AXIS_NAME_MOUSE_X);
                }
            }
            if (GameUIManager.Instance != null && GameUIManager.Instance.IsUIHover)
            {
                Action1 = false;
                Action2 = false;
                return;
            }
            if (usingActiion1)
                Action1 = Input.GetMouseButtonDown(0);
            if (usingAction2)
                Action2 = Input.GetMouseButtonDown(1);
        }

        public float GetInputMovePower()
        {
            return Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        }
    }
}
