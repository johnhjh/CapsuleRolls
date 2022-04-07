using Capsule.Game.UI;
using Capsule.Util;
using UnityEngine;

namespace Capsule.Game.Player
{
    [RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerInput), typeof(AudioSource))]
    public abstract class PlayerMove : MonoBehaviour
    {
        // Components
        protected Animator playerAnimator;
        protected Rigidbody playerRigidbody;
        protected AudioSource playerAudioSource;
        protected CapsuleCollider playerCollider;
        protected PlayerInput playerInput;
        protected RagdollController ragdollController;

        private float timeBet = 0f;
        public float turnAnimateTime = 0.3f;
        protected bool isMovingByInput = true;
        public bool isMine = true;
        public bool IsDead { get; set; }

        private void Awake()
        {
            playerAnimator = GetComponent<Animator>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerCollider = GetComponent<CapsuleCollider>();
            playerInput = GetComponent<PlayerInput>();
            playerAudioSource = GetComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            ActionButton1Ctrl actionButton1 = FindObjectOfType<ActionButton1Ctrl>();
            if (actionButton1 != null)
                actionButton1.OnClickActionButton1 -= Action1;
            ActionButton2Ctrl actionButton2 = FindObjectOfType<ActionButton2Ctrl>();
            if (actionButton2 != null)
                actionButton2.OnClickActionButton2 -= Action2;
        }

        protected virtual void Start()
        {
            ragdollController = transform.parent.GetChild(1).GetComponent<RagdollController>();
            if (!isMine) return;
            ragdollController.OnChangeRagdoll += () =>
            {
                GameCameraManager.Instance.Target = new Tuple<Transform, bool>(ragdollController.spine.transform, true);
            };
            ActionButton1Ctrl actionButton1 = FindObjectOfType<ActionButton1Ctrl>();
            if (actionButton1 != null)
                actionButton1.OnClickActionButton1 += Action1;
            ActionButton2Ctrl actionButton2 = FindObjectOfType<ActionButton2Ctrl>();
            if (actionButton2 != null)
                actionButton2.OnClickActionButton2 += Action2;
        }

        protected virtual void Update()
        {
            if (IsDead || !isMine) return;
            if (isMovingByInput)
            {
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_HORIZONTAL, playerInput.horizontal);
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_VERTICAL, playerInput.vertical);
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_MOVE_SPEED, Mathf.Clamp(playerInput.GetInputMovePower(), 0f, 1f));
            }
            if (playerInput.rotate != 0f)
            {
                playerAnimator.SetBool(GameManager.Instance.animData.HASH_IS_TURNING, true);
                if (playerInput.rotate < 0)
                    playerAnimator.SetInteger(GameManager.Instance.animData.HASH_ROTATE, -1);
                else if (playerInput.rotate > 0)
                    playerAnimator.SetInteger(GameManager.Instance.animData.HASH_ROTATE, 1);
                timeBet = Time.time + turnAnimateTime;
            }
            if (Time.time > timeBet)
            {
                playerAnimator.SetInteger(GameManager.Instance.animData.HASH_ROTATE, 0);
                playerAnimator.SetBool(GameManager.Instance.animData.HASH_IS_TURNING, false);
            }
            if (playerInput.Action1)
                Action1();
            if (playerInput.Action2)
                Action2();
        }

        protected virtual void OnDisable()
        {
            if (!IsDead)
                IsDead = true;
        }

        protected abstract void Action1();

        protected abstract void Action2();
    }
}
