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
        [HideInInspector]
        public bool usingAction1 = true;
        [HideInInspector]
        public bool usingAction2 = true;
        [HideInInspector]
        public bool usingHorizontal = false;

        private JoyStickCtrl joyStick = null;

        private void Start()
        {
            usingHorizontal = true;
            if (isMine)
            {
                joyStick = FindObjectOfType<JoyStickCtrl>();
                if (joyStick != null)
                    joyStick.InputPlayer = this;
            }
        }

        private void Update()
        {
            if (IsDead || GameManager.Instance != null && GameManager.Instance.IsGameOver)
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
            if (GameTutorialManager.Instance != null && GameTutorialManager.Instance.IsTutorialPopup)
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
                //rotate = joyStick.rotate;
                if (MobileRotateDragCtrl.Instace != null)
                    rotate = MobileRotateDragCtrl.Instace.rotate;
            }
            else
            {
                if (Application.platform != RuntimePlatform.Android &&
                    Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    if (usingHorizontal)
                    {
                        horizontal = Input.GetAxis(AXIS_NAME_HORIZONTAL);
                        rotate = Input.GetAxis(AXIS_NAME_MOUSE_X);
                    }
                    else
                    {
                        horizontal = 0f;
                        rotate = Input.GetAxis(AXIS_NAME_HORIZONTAL);
                    }
                    vertical = Input.GetAxis(AXIS_NAME_VERTICAL);
                    if (GameUIManager.Instance != null && GameUIManager.Instance.IsUIHover)
                    {
                        Action1 = false;
                        Action2 = false;
                        if (MobileRotateDragCtrl.Instace != null)
                            rotate = MobileRotateDragCtrl.Instace.rotate;
                        return;
                    }
                    if (usingAction1)
                        Action1 = Input.GetMouseButtonDown(0);
                    if (usingAction2)
                        Action2 = Input.GetMouseButtonDown(1);
                }
                else
                {
                    horizontal = 0f;
                    vertical = 0f;
                    if (MobileRotateDragCtrl.Instace != null)
                        rotate = MobileRotateDragCtrl.Instace.rotate;
                }
            }
        }

        public float GetInputMovePower()
        {
            return Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        }
    }
}
